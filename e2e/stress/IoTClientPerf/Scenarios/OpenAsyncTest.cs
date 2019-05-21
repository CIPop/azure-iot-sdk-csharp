// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Azure.Devices.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Devices.E2ETests.Scenarios
{
    public class OpenAsyncTest : PerfScenario
    {
        private DeviceClient _dc;

        public OpenAsyncTest(PerfScenarioConfig config) : base(config)
        {
            if (_authType == "sas")
            {
                _dc = DeviceClient.CreateFromConnectionString(Configuration.Stress.GetConnectionStringById(_id, _authType));
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public override Task RunTestAsync(CancellationToken ct)
        {
            return Task.CompletedTask;
        }

        public override Task SetupAsync(CancellationToken ct)
        {
            _dc.OpenAsync().ConfigureAwait(false);
        }

        public override Task TeardownAsync(CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
