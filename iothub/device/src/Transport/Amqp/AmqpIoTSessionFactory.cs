// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Azure.Devices.Client.Transport.AmqpIoT;

namespace Microsoft.Azure.Devices.Client.Transport.Amqp
{
    internal class AmqpIoTSessionFactory
    {
        private static readonly AmqpIoTSessionFactory s_instance = new AmqpIoTSessionFactory();

        private IDictionary<string, AmqpIoTConnectionPool> _amqpConnectionPools = new Dictionary<string, AmqpIoTConnectionPool>();
        private readonly object _lock = new object();
        private bool _disposed;

        internal AmqpIoTSessionFactory()
        {
        }

        internal static AmqpIoTSessionFactory GetInstance()
        {
            return s_instance;
        }

        // TODO: this should return AmqpIoTSession.
        public AmqpUnit GetAmqpIoTSession(
            DeviceIdentity deviceIdentity,
            Func<MethodRequestInternal, Task> methodHandler,
            Action<Twin, string, TwinCollection> twinMessageListener,
            Func<string, Message, Task> eventListener)
        {
            AmqpIoTConnectionPool amqpConnectionPool = ResolveConnectionPool(deviceIdentity.IotHubConnectionString.HostName);
            return amqpConnectionPool.CreateAmqpUnit(
                deviceIdentity,
                methodHandler,
                twinMessageListener,
                eventListener);
        }

        private AmqpUnit ResolveConnectionPool(string host)
        {
            lock (_lock)
            {
                _amqpConnectionPools.TryGetValue(host, out IAmqpUnitManager amqpConnectionPool);
                if (amqpConnectionPool == null)
                {
                    amqpConnectionPool = new AmqpIoTConnectionPool();
                    _amqpConnectionPools.Add(host, amqpConnectionPool);
                }

                if (Logging.IsEnabled) Logging.Associate(this, amqpConnectionPool, $"{nameof(ResolveConnectionPool)}");
                return amqpConnectionPool;
            }
        }
    }
}
