﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Azure.Devices.Client;
using System;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Devices.E2ETests
{
    public abstract class DeviceClientScenario : PerfScenario
    {
        private DeviceClient _dc;
        private TelemetryMetrics _m = new TelemetryMetrics();
        private Stopwatch _sw = new Stopwatch();
        private byte[] _messageBytes;

        private bool _pooled;
        private int _poolSize;

        public DeviceClientScenario(PerfScenarioConfig config, bool pooled = false, int poolSize = 400) : base(config)
        {
            _m.Id = _id;
            _messageBytes = new byte[_sizeBytes];

            _pooled = pooled;
            _poolSize = poolSize;

            BitConverter.TryWriteBytes(_messageBytes, _id);
        }

        protected async Task CreateDeviceAsync()
        {
            _sw.Restart();
            _m.OperationType = "create";

            ITransportSettings transportSettings = null;

            if (_pooled && ((_transport == Client.TransportType.Amqp_Tcp_Only) || (_transport == Client.TransportType.Amqp_WebSocket_Only)))
            {
                transportSettings = new AmqpTransportSettings(
                    _transport,
                    50,
                    new AmqpConnectionPoolSettings()
                    {
                        Pooling = true,
                        MaxPoolSize = (uint)_poolSize,
                    });
            }

            if (_authType == "sas")
            {
                if (transportSettings == null)
                {
                    _dc = DeviceClient.CreateFromConnectionString(Configuration.Stress.GetConnectionStringById(_id, _authType), _transport);
                }
                else
                {
                    _dc = DeviceClient.CreateFromConnectionString(Configuration.Stress.GetConnectionStringById(_id, _authType), new ITransportSettings[] { transportSettings });
                }
            }
            else if (_authType == "x509")
            {
                _dc = DeviceClient.Create(
                    Configuration.Stress.Endpoint,
                    new DeviceAuthenticationWithX509Certificate(
                        Configuration.Stress.GetDeviceNameById(_id, _authType),
                        Configuration.Stress.Certificate),
                    _transport);
            }
            else
            {
                throw new NotImplementedException($"Not implemented for authType {_authType}");
            }

            _m.ExecuteTime = _sw.ElapsedMilliseconds;
            _m.ScheduleTime = null; // sync operation
            await _writer.WriteAsync(_m).ConfigureAwait(false);
        }

        protected async Task OpenDeviceAsync(CancellationToken ct)
        {
            ExceptionDispatchInfo exInfo = null;
            _m.OperationType = "open";
            _m.ScheduleTime = null;
            _sw.Restart();
            try
            {
                Task t = _dc.OpenAsync(ct);
                _m.ScheduleTime = _sw.ElapsedMilliseconds;

                _sw.Restart();
                await t.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _m.ErrorMessage = ex.Message;
                exInfo = ExceptionDispatchInfo.Capture(ex);
            }

            _m.ExecuteTime = _sw.ElapsedMilliseconds;
            await _writer.WriteAsync(_m).ConfigureAwait(false);

            exInfo?.Throw();
        }

        protected async Task SendMessageAsync(CancellationToken ct)
        {
            ExceptionDispatchInfo exInfo = null;
            _m.OperationType = "send_d2c";
            _m.ScheduleTime = null;
            _sw.Restart();

            try
            {
                Client.Message message = new Client.Message(_messageBytes);
                Task t = _dc.SendEventAsync(message, ct);
                _m.ScheduleTime = _sw.ElapsedMilliseconds;

                _sw.Restart();
                await t.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _m.ErrorMessage = ex.Message;
                exInfo = ExceptionDispatchInfo.Capture(ex);
            }

            _m.ExecuteTime = _sw.ElapsedMilliseconds;
            await _writer.WriteAsync(_m).ConfigureAwait(false);
            exInfo?.Throw();
        }

        protected Task CloseAsync(CancellationToken ct)
        {
            return _dc.CloseAsync(ct);
        }
    }
}
