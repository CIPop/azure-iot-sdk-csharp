// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Azure.Devices.Client;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Devices.E2ETests
{
    public class DeviceD2C : PerfScenario
    {
        private DeviceClient _dc;
        private TelemetryMetrics _m = new TelemetryMetrics();
        private Stopwatch _sw = new Stopwatch();
        private byte[] _messageBytes;

        public DeviceD2C(PerfScenarioConfig config) : base(config)
        {
            _m.Id = _id;
            _messageBytes = new byte[_sizeBytes];
            BitConverter.TryWriteBytes(_messageBytes, _id);
        }

        public override async Task RunTestAsync(CancellationToken ct)
        {
            await CreateDeviceAsync().ConfigureAwait(false);
            await OpenDeviceAsync(ct).ConfigureAwait(false);

            await Task.Delay(10000).ConfigureAwait(false);
        }

        private async Task CreateDeviceAsync()
        {
            _sw.Restart();
            _m.OperationType = "create";

            if (_authType == "sas")
            {
                _dc = DeviceClient.CreateFromConnectionString(Configuration.Stress.GetConnectionStringById(_id, _authType));
            }
            else if (_authType == "x509")
            {
                _dc = DeviceClient.Create(
                    Configuration.Stress.Endpoint,
                    new DeviceAuthenticationWithX509Certificate(
                        Configuration.Stress.GetDeviceNameById(_id, _authType),
                        Configuration.Stress.Certificate));
            }
            else
            {
                throw new NotImplementedException($"Not implemented for authType {_authType}");
            }

            _m.ExecuteTime = _sw.ElapsedMilliseconds;
            _m.ScheduleTime = null; // sync operation
            await _writer.WriteAsync(_m).ConfigureAwait(false);
        }
        
        private async Task OpenDeviceAsync(CancellationToken ct)
        {
            _m.OperationType = "open";
            _sw.Restart();
            Task t = _dc.OpenAsync(ct);
            _m.ScheduleTime = _sw.ElapsedMilliseconds;

            _sw.Restart();
            try
            {
                await t.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _m.ErrorMessage = ex.Message;

            }

            _m.ExecuteTime = _sw.ElapsedMilliseconds;
            await _writer.WriteAsync(_m).ConfigureAwait(false);
        }

        private async Task SendMessageAsync(CancellationToken ct)
        {
            _m.OperationType = "send_d2c";

            _sw.Restart();
            Client.Message message = new Client.Message(_messageBytes);
            Task t = _dc.SendEventAsync(message, ct);
            _m.ScheduleTime = _sw.ElapsedMilliseconds;

            _sw.Restart();
            await t.ConfigureAwait(false);
            _m.ExecuteTime = _sw.ElapsedMilliseconds;
            await _writer.WriteAsync(_m).ConfigureAwait(false);
        }

        public override Task SetupAsync(CancellationToken ct)
        {
            return Task.CompletedTask;
        }

        public override Task TeardownAsync(CancellationToken ct)
        {
            return _dc.CloseAsync(ct);
        }
    }
}
