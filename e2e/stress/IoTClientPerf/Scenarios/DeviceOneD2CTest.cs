// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Azure.Devices.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Devices.E2ETests
{
    public class DeviceOneD2CTest : PerfScenario
    {
        private static DeviceClient s_dc;

        public DeviceOneD2CTest(PerfScenarioConfig config) : base(config)
        {
            if (_id == 0)
            {
                //s_dc = 
            }
        }

        public override Task SetupAsync(CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public override Task RunTestAsync(CancellationToken ct)
        {
            throw new NotImplementedException();

        }

        public override Task TeardownAsync(CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
