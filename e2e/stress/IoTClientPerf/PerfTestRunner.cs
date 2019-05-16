// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Devices.E2ETests
{
    public class PerfTestRunner
    {
        private const int MaximumInitializationTimeSeconds = 2 * 60;
        private const int StatUpdateIntervalMilliseconds = 500;

        // Scenario information:
        private readonly ResultWriter _log;
        private readonly Client.TransportType _transportType;
        private readonly int _messageSizeBytes;
        private readonly string _authType;

        // Runner information:
        private readonly int _parallelOperations;
        private readonly int _n;
        private readonly int _timeSeconds;
        private readonly Func<PerfScenarioConfig, PerfScenario> _scenarioFactory;

        public PerfTestRunner(
            ResultWriter writer,
            int timeSeconds,
            Client.TransportType transportType,
            int messageSizeBytes,
            int maximumParallelOperations,
            int scenarioInstances,
            string authType,
            Func<PerfScenarioConfig, PerfScenario> scenarioFactory)
        {
            _log = writer;
            _timeSeconds = timeSeconds;
            _transportType = transportType;
            _messageSizeBytes = messageSizeBytes;
            _parallelOperations = maximumParallelOperations;
            _n = scenarioInstances;
            _authType = authType;
            _scenarioFactory = scenarioFactory;

            Console.WriteLine($"Running {_timeSeconds}s test. ({authType})");
            Console.WriteLine($"  {_n} operations ({_parallelOperations} parallel) with {_messageSizeBytes}B/message.");
        }

        public async Task RunTestAsync()
        {
            var tests = new PerfScenario[_n];
            Stopwatch sw = new Stopwatch();

            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(MaximumInitializationTimeSeconds)))
            {
                var semaphore = new SemaphoreSlim(_parallelOperations);
                Console.Write($"Initializing tests (timeout={MaximumInitializationTimeSeconds}s) ... ");

                PerfScenarioConfig c = new PerfScenarioConfig()
                {
                    Id = 0,
                    SizeBytes = _messageSizeBytes,
                    Writer = _log
                };

                for (int i = 0; i < tests.Length; i++)
                {
                    c.Id = i;
                    tests[i] = _scenarioFactory(c);
                }

                for (int i = 0; i < tests.Length; i++)
                {
                    await semaphore.WaitAsync(cts.Token).ConfigureAwait(false);
                    await tests[i].SetupAsync(cts.Token).ConfigureAwait(false);
                    semaphore.Release();
                }

                Console.WriteLine($"{sw.Elapsed}");
            }

            sw.Restart();

            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(_timeSeconds)))
            {
                int actualParallel = Math.Min(_parallelOperations, _n);
                int currentInstance = 0;

                // Intermediate status update
                int statInterimCompleted = 0;
                int statTotalCompleted = 0;
                Stopwatch statInterimSw = new Stopwatch();
                statInterimSw.Start();

                var tasks = new List<Task>(actualParallel);

                for (; currentInstance < actualParallel; currentInstance++)
                {
                    tasks.Add(tests[currentInstance].RunTestAsync(cts.Token));
                }

                while (true)
                {
                    Task finished = await Task.WhenAny(tasks).ConfigureAwait(false);
                    tasks.Remove(finished);
                    statInterimCompleted++;

                    if (statInterimSw.Elapsed.TotalMilliseconds > StatUpdateIntervalMilliseconds)
                    {
                        statInterimSw.Stop();
                        statTotalCompleted += statInterimCompleted;

                        // Totals:
                        double totalSeconds = sw.Elapsed.TotalSeconds;
                        double totalRequestsPerSec = statTotalCompleted / totalSeconds;
                        double totalTransferPerSec = (statTotalCompleted * _messageSizeBytes) / totalSeconds;

                        // Interim:
                        double interimSeconds = statInterimSw.Elapsed.TotalSeconds;
                        double requestsPerSec = statInterimCompleted / interimSeconds;
                        double transferPerSec = (statInterimCompleted * _messageSizeBytes) / interimSeconds;

                        Console.Write(
                            $"[{sw.Elapsed}] " +
                            $"{requestsPerSec:       0.00} RPS" +
                            $"{GetHumanReadableBytesPerSecond(transferPerSec)}" +
                            $"TOTAL: " + 
                            $"{totalRequestsPerSec:       0.00} RPS" +
                            $"{GetHumanReadableBytesPerSecond(totalTransferPerSec)}" +
                            $"\r");

                        statInterimSw.Restart();
                    }

                    currentInstance++;
                    if (currentInstance > _n) currentInstance = 0;

                    tasks.Add(tests[currentInstance].RunTestAsync(cts.Token));
                }
            }
        }

        public static string GetHumanReadableBytesPerSecond(double bytesPerSecond)
        {
            if (bytesPerSecond < 1024)
            {
                return $"{bytesPerSecond}B/s";
            }
            else if (bytesPerSecond < 1024 * 1024)
            {
                return $"{bytesPerSecond / 1024: 0.00}kB/s";
            }
            else if (bytesPerSecond < 1024 * 1024 * 1024)
            {
                return $"{bytesPerSecond / (1024 * 1024): 0.00}MB/s";
            }
                
            return $"{bytesPerSecond / (1024 * 1024 * 1024): 0.00}GB/s";
        }
    }
}
