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

        private readonly ResultWriter _log;
        private readonly int _timeSeconds;
        private readonly Client.TransportType _transportType;
        private readonly int _messageSizeBytes;
        private readonly int _parallelOperations;
        private readonly int _n;
        private readonly string _authType;
        private readonly Type _scenarioClassType;

        public PerfTestRunner(
            ResultWriter writer,
            int timeSeconds,
            Client.TransportType transportType,
            int messageSizeBytes,
            int maximumParallelOperations,
            int scenarioInstances,
            string authType,
            Type scenarioClassType)
        {
            _log = writer;
            _timeSeconds = timeSeconds;
            _transportType = transportType;
            _messageSizeBytes = messageSizeBytes;
            _parallelOperations = maximumParallelOperations;
            _n = scenarioInstances;
            _authType = authType;
            _scenarioClassType = scenarioClassType;

            Console.WriteLine($"Running {_timeSeconds}s test.");
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

                for (int i = 0; i < tests.Length; i++)
                {
                    tests[i] = (PerfScenario)Activator.CreateInstance(_scenarioClassType);
                }

                for (int i = 0; i < tests.Length; i++)
                {
                    await semaphore.WaitAsync(cts.Token).ConfigureAwait(false);
                    await tests[i].SetupAsync(cts.Token).ConfigureAwait(false);
                    semaphore.Release();
                }

                Console.WriteLine($"{sw.Elapsed}");
            }

            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(_timeSeconds)))
            {
                int actualParallel = Math.Min(_parallelOperations, _n);
                int currentInstance = 0;
                int interimStatsCompleted = 0;
                var tasks = new List<Task>(actualParallel);

                for (; currentInstance < actualParallel; currentInstance++)
                {
                    tasks.Add(tests[currentInstance].RunTestAsync(cts.Token));
                }

                while (true)
                {
                    Task finished = await Task.WhenAny(tasks).ConfigureAwait(false);
                    tasks.Remove(finished);

                    currentInstance++;
                    if (currentInstance > _n) currentInstance = 0;

                    tasks.Add(tests[currentInstance].RunTestAsync(cts.Token));
                }




            }

        }

    }
}

