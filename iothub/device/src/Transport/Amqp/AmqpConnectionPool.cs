// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Azure.Devices.Client.Transport.AmqpIoT;

namespace Microsoft.Azure.Devices.Client.Transport.Amqp
{
    internal class AmqpConnectionPool : IAmqpUnitManager
    {
        private const int MaxSpan = int.MaxValue;
        private readonly List<IAmqpConnectionHolder> AmqpSasIndividualPool;
        private readonly IDictionary<string, List<IAmqpConnectionHolder>> AmqpSasGroupedPool;
        private readonly object Lock;

        internal AmqpConnectionPool()
        {
            AmqpSasIndividualPool = new List<IAmqpConnectionHolder>();
            AmqpSasGroupedPool = new Dictionary<string, List<IAmqpConnectionHolder>>();
            Lock = new object();
        }

        public AmqpUnit CreateAmqpUnit(
            DeviceIdentity deviceIdentity, 
            Func<MethodRequestInternal, Task> methodHandler, 
            Action<Twin, string, TwinCollection> twinMessageListener, 
            Func<string, Message, Task> eventListener)
        {
            if (Logging.IsEnabled) Logging.Enter(this, deviceIdentity, $"{nameof(CreateAmqpUnit)}");
            if (deviceIdentity.AuthenticationModel != AuthenticationModel.X509 && (deviceIdentity.AmqpTransportSettings?.AmqpConnectionPoolSettings?.Pooling??false))
            {
                IAmqpConnectionHolder amqpConnectionHolder;
                lock (Lock)
                {
                    List<IAmqpConnectionHolder> amqpConnectionHolders = ResolveConnectionGroup(deviceIdentity, true);
                    if (amqpConnectionHolders.Count < deviceIdentity.AmqpTransportSettings.AmqpConnectionPoolSettings.MaxPoolSize)
                    {
                        amqpConnectionHolder = new AmqpConnectionHolder(deviceIdentity);
                        amqpConnectionHolders.Add(amqpConnectionHolder);
                        if (Logging.IsEnabled) Logging.Associate(this, amqpConnectionHolder, "amqpConnectionHolders");
                    }
                    else
                    {
                        amqpConnectionHolder = GetConsistentHashConnection(amqpConnectionHolders, deviceIdentity);
                    }
                }
                if (Logging.IsEnabled) Logging.Exit(this, deviceIdentity, $"{nameof(CreateAmqpUnit)}");
                return amqpConnectionHolder.CreateAmqpUnit(deviceIdentity, methodHandler, twinMessageListener, eventListener);
            }
            else
            {
                if (Logging.IsEnabled) Logging.Exit(this, deviceIdentity, $"{nameof(CreateAmqpUnit)}");
                return new AmqpConnectionHolder(deviceIdentity)
                    .CreateAmqpUnit(deviceIdentity, methodHandler, twinMessageListener, eventListener);
            }
        }

        private List<IAmqpConnectionHolder> ResolveConnectionGroup(DeviceIdentity deviceIdentity, bool create)
        {
            if (deviceIdentity.AuthenticationModel == AuthenticationModel.SasIndividual)
            {
                return AmqpSasIndividualPool;
            }
            else
            {
                string scope = deviceIdentity.IotHubConnectionString.SharedAccessKeyName;
                AmqpSasGroupedPool.TryGetValue(scope, out List<IAmqpConnectionHolder>  amqpConnectionHolders);
                if (create && amqpConnectionHolders == null)
                {
                    amqpConnectionHolders = new List<IAmqpConnectionHolder>();
                    AmqpSasGroupedPool.Add(scope, amqpConnectionHolders);
                }
                return amqpConnectionHolders;
            }
        }
        
        private IAmqpConnectionHolder GetConsistentHashConnection(List<IAmqpConnectionHolder> pool, DeviceIdentity deviceIdentity)
        {
            if (Logging.IsEnabled) Logging.Enter(this, deviceIdentity, $"{nameof(GetConsistentHashConnection)}");

            int poolSize = pool.Count;
            int index = Math.Abs(deviceIdentity.GetHashCode()) % poolSize;

            if (pool[index] == null)
            {
                pool[index] = new AmqpConnectionHolder(deviceIdentity);
            }

            if (Logging.IsEnabled) Logging.Exit(this, deviceIdentity, $"{nameof(GetConsistentHashConnection)}");
            return pool[index];
        }
    }
}
