// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Azure.Devices.Client.Transport.AmqpIoT;

namespace Microsoft.Azure.Devices.Client.Transport.Amqp
{
    internal class AmqpIoTConnectionPool
    {
        private string _name;
        private int _maxNumberOfConnections;

        internal AmqpIoTConnectionPool()
        {
        }




#if false
        public AmqpUnit CreateAmqpUnit(
            DeviceIdentity deviceIdentity, 
            Func<MethodRequestInternal, Task> methodHandler, 
            Action<Twin, string, TwinCollection> twinMessageListener, 
            Func<string, Message, Task> eventListener)
        {
            if (Logging.IsEnabled) Logging.Enter(this, deviceIdentity, $"{nameof(CreateAmqpUnit)}");
            if (deviceIdentity.AuthenticationModel != AuthenticationModel.X509Certificate && (deviceIdentity.AmqpTransportSettings?.AmqpConnectionPoolSettings?.Pooling??false))
            {
                IAmqpConnectionHolder amqpConnectionHolder;
                lock (Lock)
                {
                    ISet<IAmqpConnectionHolder> amqpConnectionHolders = ResolveConnectionGroup(deviceIdentity, true);
                    if (amqpConnectionHolders.Count < deviceIdentity.AmqpTransportSettings.AmqpConnectionPoolSettings.MaxPoolSize)
                    {
                        amqpConnectionHolder = new AmqpIoTConnection(deviceIdentity);
                        amqpConnectionHolder.OnConnectionDisconnected += (o, args) => RemoveConnection(amqpConnectionHolders, o as IAmqpConnectionHolder);
                        amqpConnectionHolders.Add(amqpConnectionHolder);
                        if (Logging.IsEnabled) Logging.Associate(this, amqpConnectionHolder, "amqpConnectionHolders");
                    }
                    else
                    {
                        amqpConnectionHolder = GetLeastUsedConnection(amqpConnectionHolders);
                    }
                }
                if (Logging.IsEnabled) Logging.Exit(this, deviceIdentity, $"{nameof(CreateAmqpUnit)}");
                return amqpConnectionHolder.CreateAmqpUnit(deviceIdentity, methodHandler, twinMessageListener, eventListener);
            }
            else
            {
                if (Logging.IsEnabled) Logging.Exit(this, deviceIdentity, $"{nameof(CreateAmqpUnit)}");
                return new AmqpIoTConnection(deviceIdentity)
                    .CreateAmqpUnit(deviceIdentity, methodHandler, twinMessageListener, eventListener);
            }
        }

        private void RemoveConnection(ISet<IAmqpConnectionHolder> amqpConnectionHolders, IAmqpConnectionHolder amqpConnectionHolder)
        {
            lock (Lock)
            {
                
                bool removed = amqpConnectionHolder.GetNumberOfUnits() == 0 && amqpConnectionHolders.Remove(amqpConnectionHolder);
                if (Logging.IsEnabled) Logging.Info(this, $"Remove ConnectionHolder {amqpConnectionHolder}: {removed}");
            }
        }

        private ISet<IAmqpConnectionHolder> ResolveConnectionGroup(DeviceIdentity deviceIdentity, bool create)
        {
            if (deviceIdentity.AuthenticationModel == AuthenticationModel.SharedAccessKeyHubPolicy)
            {
                return AmqpSasIndividualPool;
            }
            else
            {
                string scope = deviceIdentity.IotHubConnectionString.SharedAccessKeyName;
                AmqpSasGroupedPool.TryGetValue(scope, out ISet<IAmqpConnectionHolder>  amqpConnectionHolders);
                if (create && amqpConnectionHolders == null)
                {
                    amqpConnectionHolders = new HashSet<IAmqpConnectionHolder>();
                    AmqpSasGroupedPool.Add(scope, amqpConnectionHolders);
                }
                return amqpConnectionHolders;
            }
        }
        
        private IAmqpConnectionHolder GetLeastUsedConnection(ISet<IAmqpConnectionHolder> amqpConnectionHolders)
        {
            if (Logging.IsEnabled) Logging.Enter(this, $"{nameof(GetLeastUsedConnection)}");

            int count = MaxSpan;

            IAmqpConnectionHolder amqpConnectionHolder = null;

            foreach (IAmqpConnectionHolder value in amqpConnectionHolders)
            {
                int clientCount = value.GetNumberOfUnits();
                if (clientCount < count)
                {
                    amqpConnectionHolder = value;
                    count = clientCount;
                    if (count == 0)
                    {
                        break;
                    }
                }
            }

            if (Logging.IsEnabled) Logging.Exit(this, $"{nameof(GetLeastUsedConnection)}");
            return amqpConnectionHolder;
        }
#endif
    }
}
