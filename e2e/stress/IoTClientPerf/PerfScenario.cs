// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Devices.E2ETests
{
    internal abstract class PerfScenario
    {
        protected ResultWriter _writer;
        protected int _sizeBytes;

        public PerfScenario(ResultWriter writer, int sizeBytes)
        {
            _writer = writer;
            _sizeBytes = sizeBytes;
        }

        public abstract string Help();

        public abstract Task SetupAsync(CancellationToken ct);

        public abstract Task RunTestAsync(CancellationToken ct);

        public abstract Task TeardownAsync(CancellationToken ct);

        public abstract void OnTaskUpdate(TimeSpan delta);
    }
}
