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
        private readonly string _configString;

        private PerfScenario[] _tests;
        private Stopwatch _sw = new Stopwatch();

        public PerfTestRunner(
            ResultWriter writer,
            int timeSeconds,
            Client.TransportType transportType,
            int messageSizeBytes,
            int maximumParallelOperations,
            int scenarioInstances,
            string authType,
            string scenario,
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
             _tests = new PerfScenario[_n];

            TelemetryMetrics.SetStaticConfigParameters(_timeSeconds, _transportType, _messageSizeBytes, _parallelOperations, _n, _authType, scenario);

            Console.WriteLine($"Running {_timeSeconds}s test. ({authType})");
            Console.WriteLine($"  {_n} operations ({_parallelOperations} parallel) with {_messageSizeBytes}B/message.");
        }

        public async Task RunTestAsync()
        {
            try
            {
                await SetupAllAsync().ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("FAILED (timeout)");
            }

            _sw.Restart();

            try
            {
                await LoopAsync().ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"Test ended ({_sw.Elapsed}).");
            }

            _sw.Restart();

            await TeardownAllAsync().ConfigureAwait(false);
        }

        private async Task LoopAsync()
        {
            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(_timeSeconds)))
            {
                int actualParallel = Math.Min(_parallelOperations, _n);
                int currentInstance = 0;

                // Intermediate status update
                ulong statInterimCompleted = 0;
                ulong statTotalCompleted = 0;
                Stopwatch statInterimSw = new Stopwatch();
                statInterimSw.Start();

                var tasks = new List<Task>(actualParallel);

                for (; currentInstance < actualParallel; currentInstance++)
                {
                    tasks.Add(_tests[currentInstance].RunTestAsync(cts.Token));
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
                        double totalSeconds = _sw.Elapsed.TotalSeconds;
                        double totalRequestsPerSec = statTotalCompleted / totalSeconds;
                        double totalTransferPerSec = (statTotalCompleted * (ulong)_messageSizeBytes) / totalSeconds;

                        // Interim:
                        double interimSeconds = statInterimSw.Elapsed.TotalSeconds;
                        double requestsPerSec = statInterimCompleted / interimSeconds;
                        double transferPerSec = (statInterimCompleted * (ulong)_messageSizeBytes) / interimSeconds;

                        Console.Write(
                            $"[{_sw.Elapsed}] " +
                            $"{requestsPerSec:       0.00} RPS" +
                            $"{GetHumanReadableBytesPerSecond(transferPerSec)}" +
                            $" TOTAL: " +
                            $"{totalRequestsPerSec:       0.00} RPS" +
                            $"{GetHumanReadableBytesPerSecond(totalTransferPerSec)}" +
                            $"                 \r");

                        statInterimSw.Restart();
                    }

                    if (currentInstance >= _n) currentInstance = 0;
                    tasks.Add(_tests[currentInstance].RunTestAsync(cts.Token));

                    currentInstance++;
                }
            }
        }

        private async Task SetupAllAsync()
        {
            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(MaximumInitializationTimeSeconds)))
            {
                var semaphore = new SemaphoreSlim(_parallelOperations);
                Stopwatch statInterimSw = new Stopwatch();
                statInterimSw.Start();

                PerfScenarioConfig c = new PerfScenarioConfig()
                {
                    Id = 0,
                    SizeBytes = _messageSizeBytes,
                    Writer = _log,
                    AuthType = _authType,
                    Transport = _transportType,
                };

                for (int i = 0; i < _tests.Length; i++)
                {
                    c.Id = i;
                    _tests[i] = _scenarioFactory(c);
                }

                for (int i = 0; i < _tests.Length; i++)
                {
                    await semaphore.WaitAsync(cts.Token).ConfigureAwait(false);
                    await _tests[i].SetupAsync(cts.Token).ConfigureAwait(false);
                    semaphore.Release();

                    if (statInterimSw.Elapsed.TotalMilliseconds > StatUpdateIntervalMilliseconds)
                    {
                        statInterimSw.Restart();
                        int p_completed = (int)(((float)i / _n) * 100);
                        Console.Write($"Initializing tests (timeout={MaximumInitializationTimeSeconds}s) ... {_sw.Elapsed} {p_completed}% \r");
                    }
                }

                Console.WriteLine($"Initializing tests (timeout={MaximumInitializationTimeSeconds}s) ... {_sw.Elapsed}        ");
            }
        }

        private async Task TeardownAllAsync()
        {
            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(MaximumInitializationTimeSeconds)))
            {
                Stopwatch statInterimSw = new Stopwatch();
                statInterimSw.Start();

                int count = 0;
                foreach (PerfScenario test in _tests)
                {
                    try
                    {
                        await test.TeardownAsync(cts.Token).ConfigureAwait(false);

                        if (statInterimSw.Elapsed.TotalMilliseconds > StatUpdateIntervalMilliseconds)
                        {
                            statInterimSw.Restart();
                            int p_done = (int)(((float)count / _n) * 100);
                            Console.Write($"Teardown: ... [{_sw.Elapsed}] {p_done}% \r");
                        }

                        count++;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed: {ex}");
                    }
                }

                Console.WriteLine($"Teardown: ... [{_sw.Elapsed}]     ");
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
