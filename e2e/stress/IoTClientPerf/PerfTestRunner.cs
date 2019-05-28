// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Devices.E2ETests
{
    public class PerfTestRunner
    {
        private const int MaximumInitializationTimeSeconds = 2 * 60;

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
            _sw.Restart();
            await _log.WriteHeaderAsync().ConfigureAwait(false);

            try
            {
                await SetupAllAsync().ConfigureAwait(false);
                Console.WriteLine($"Setup completed (time:{_sw.Elapsed})");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"Setup FAILED (timeout:{_sw.Elapsed})");
            }

            _sw.Restart();
            Console.WriteLine();

            try
            {
                await LoopAsync().ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"Test ended ({_sw.Elapsed}).");
            }

            _sw.Restart();
            Console.WriteLine();
            
            await TeardownAllAsync().ConfigureAwait(false);
            Console.WriteLine("Done.                                    ");
        }

        private async Task LoopAsync()
        {
            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(_timeSeconds)))
            {
                ulong statTotalCompleted = 0;
                ulong statTotalFaulted = 0;
                ulong statTotalCancelled = 0;
                double statTotalSeconds = 0.0;
                List<double> statRps = new List<double>();

                var runner = new ParallelRun(
                    _tests,
                    _parallelOperations,
                    (test) => test.RunTestAsync(CancellationToken.None),
                    (statInterimCompleted, statInterimFaulted, statInterimCancelled, statInterimTimeSec) =>
                    {
                        statTotalCompleted += statInterimCompleted;
                        statTotalFaulted += statInterimFaulted;
                        statTotalCancelled += statInterimCancelled;

                        statTotalSeconds += statInterimTimeSec;

                        // Interim:
                        double requestsPerSec = statInterimCompleted / statInterimTimeSec;
                        double transferPerSec = requestsPerSec * _messageSizeBytes;

                        statRps.Add(requestsPerSec);

                        // Totals:
                        double totalRequestsPerSec = statTotalCompleted / statTotalSeconds;
                        double totalTransferPerSec = totalRequestsPerSec * _messageSizeBytes;

                        (double avgRps, double stdDevRps) = CalculateAvgAndStDev(statRps);
                        double avgBps = avgRps * _messageSizeBytes;
                        double stdDevBps = stdDevRps * _messageSizeBytes;

                        Console.WriteLine($"[{_sw.Elapsed}] Loop Statistics:");
                        Console.WriteLine($"RPS       : {requestsPerSec,10:N2} R/s Avg: {avgRps,10:N2} R/s +/-StdDev: {stdDevRps,10:N2} R/s");
                        Console.WriteLine($"Throughput: {GetHumanReadableBytes(transferPerSec)}/s Avg: {GetHumanReadableBytes(avgBps)}/s +/-StdDev: {GetHumanReadableBytes(avgRps)}/s         ");
                        Console.WriteLine("----");
                        Console.WriteLine($"TOTALs: ");
                        Console.WriteLine($"Requests  : Completed: {statTotalCompleted,10:N0} Faulted: {statTotalFaulted,10:N0} Cancelled: {statTotalCancelled,10:N0}");
                        Console.WriteLine($"Data      :    {GetHumanReadableBytes(statTotalCompleted * (ulong)_messageSizeBytes)}             ");
                    });

                await runner.RunAsync(runOnce: false, cts.Token).ConfigureAwait(false);
            }
        }

        private async Task SetupAllAsync()
        {
            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(_timeSeconds)))
            {
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

                ulong statTotalCompleted = 0;
                ulong statTotalFaulted = 0;
                ulong statTotalCancelled = 0;
                double statTotalSeconds = 0.0;
                List<double> statRps = new List<double>();

                var runner = new ParallelRun(
                    _tests,
                    _parallelOperations,
                    (test) => test.SetupAsync(CancellationToken.None),
                    (statInterimCompleted, statInterimFaulted, statInterimCancelled, statInterimTimeSec) =>
                    {
                        statTotalCompleted += statInterimCompleted;
                        statTotalFaulted += statInterimFaulted;
                        statTotalCancelled += statInterimCancelled;

                        statTotalSeconds += statInterimTimeSec;

                        // Interim:
                        double requestsPerSec = statInterimCompleted / statInterimTimeSec;
                        statRps.Add(requestsPerSec);

                        // Totals:
                        double totalRequestsPerSec = statTotalCompleted / statTotalSeconds;

                        (double avgRps, double stdDevRps) = CalculateAvgAndStDev(statRps);
                        
                        Console.WriteLine($"[{_sw.Elapsed}] Setup Statistics:");
                        Console.WriteLine($"RPS       : {requestsPerSec,10:N2} R/s Avg: {avgRps,10:N2} R/s +/-StdDev: {stdDevRps,10:N2} R/s");
                        Console.WriteLine("----");
                        Console.WriteLine($"TOTALs: ");
                        Console.WriteLine($"Requests  : Completed: {statTotalCompleted,10:N0} Faulted: {statTotalFaulted,10:N0} Cancelled: {statTotalCancelled,10:N0}");
                    });

                await runner.RunAsync(runOnce: true, cts.Token).ConfigureAwait(false);
            }
        }

        private async Task TeardownAllAsync()
        {
            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(_timeSeconds)))
            {

                ulong statTotalCompleted = 0;
                ulong statTotalFaulted = 0;
                ulong statTotalCancelled = 0;
                double statTotalSeconds = 0.0;
                List<double> statRps = new List<double>();

                var runner = new ParallelRun(
                    _tests,
                    _parallelOperations,
                    (test) => test.TeardownAsync(CancellationToken.None),
                    (statInterimCompleted, statInterimFaulted, statInterimCancelled, statInterimTimeSec) =>
                    {
                        statTotalCompleted += statInterimCompleted;
                        statTotalFaulted += statInterimFaulted;
                        statTotalCancelled += statInterimCancelled;

                        statTotalSeconds += statInterimTimeSec;

                        // Interim:
                        double requestsPerSec = statInterimCompleted / statInterimTimeSec;
                        statRps.Add(requestsPerSec);

                        // Totals:
                        double totalRequestsPerSec = statTotalCompleted / statTotalSeconds;

                        (double avgRps, double stdDevRps) = CalculateAvgAndStDev(statRps);

                        Console.WriteLine($"[{_sw.Elapsed}] Teardown Statistics:");
                        Console.WriteLine($"RPS       : {requestsPerSec,10:N2} R/s Avg: {avgRps,10:N2} R/s +/-StdDev: {stdDevRps,10:N2} R/s");
                        Console.WriteLine("----");
                        Console.WriteLine($"TOTALs: ");
                        Console.WriteLine($"Requests  : Completed: {statTotalCompleted,10:N0} Faulted: {statTotalFaulted,10:N0} Cancelled: {statTotalCancelled,10:N0}");
                    });

                await runner.RunAsync(runOnce: true, cts.Token).ConfigureAwait(false);
            }
        }
        
        private static string GetHumanReadableBytes(double bytes)
        {
            if (bytes < 1024)
            {
                return $"{bytes,10:N2}B ";
            }
            else if (bytes < 1024 * 1024)
            {
                return $"{bytes / 1024,10:N2}kB";
            }
            else if (bytes < 1024 * 1024 * 1024)
            {
                return $"{bytes / (1024 * 1024),10:N2}MB";
            }
                
            return $"{bytes / (1024 * 1024 * 1024),10:N2}GB";
        }

        private static Tuple<double, double> CalculateAvgAndStDev(List<double> values)
        {
            double avg = values.Average();
            double stddev = Math.Sqrt(values.Average(v => ((v - avg) * (v - avg))));

            return new Tuple<double, double>(avg, stddev);
        }
    }
}
