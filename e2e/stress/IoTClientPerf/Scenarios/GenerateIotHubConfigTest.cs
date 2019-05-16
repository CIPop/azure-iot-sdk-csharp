// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Devices.E2ETests
{
    public class GenerateIotHubConfigTest : PerfScenario
    {
        // Pattern: IotClientPerf_<auth>_id
        private readonly string NamePrefix = "IotClientPerf_";
        private static StreamWriter s_outputFile = new StreamWriter("iotclientperf_import.txt");

        public GenerateIotHubConfigTest(PerfScenarioConfig config) : base(config)
        {
        }
        
        public override async Task SetupAsync(CancellationToken ct)
        {
            if (_authType != "sas_device") throw new NotImplementedException();
            //await s_outputFile.WriteLineAsync($"{"id":"Device1","eTag":"MA==","status":"enabled","authentication":{"symmetricKey":{"primaryKey":"abc=","secondaryKey":"def="}}}
        }

        public override async Task TeardownAsync(CancellationToken ct)
        {
            try
            {
                await s_outputFile.FlushAsync().ConfigureAwait(false);
                s_outputFile.Dispose();
            }
            catch (ObjectDisposedException)
            { }
        }

        public override Task RunTestAsync(CancellationToken ct)
        {
            return Task.CompletedTask;
        }
    }
}
