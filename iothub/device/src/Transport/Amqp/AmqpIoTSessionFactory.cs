// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Azure.Devices.Client.Transport.AmqpIoT;
using System.Diagnostics;

namespace Microsoft.Azure.Devices.Client.Transport.Amqp
{
    internal class AmqpIoTSessionFactory
    {
        private const string PoolNameIndividualIdentityKey = "SAS";
        private const string PoolNameHubPolicyKey = "SASPolicy";
        private const string PoolNameX509Certificate = "X509";

        private static readonly AmqpIoTSessionFactory s_instance = new AmqpIoTSessionFactory();

        private Dictionary<string, AmqpIoTConnectionPool> _amqpConnectionPools = new Dictionary<string, AmqpIoTConnectionPool>();
        private readonly object _lock = new object();

        internal AmqpIoTSessionFactory()
        {
        }

        internal static AmqpIoTSessionFactory GetInstance()
        {
            return s_instance;
        }

        // TODO: this should return AmqpIoTSession.
        // TODO: UT
        public Task<AmqpIoTSession> GetAmqpIoTSessionAsync(
            DeviceIdentity deviceIdentity)
        {
            string connectionPoolName = GetConnectionPoolName(deviceIdentity);

            // 
            throw new NotImplementedException();
        }

        private string GetConnectionPoolName(DeviceIdentity deviceIdentity)
        {
            // TODO
            string poolName = deviceIdentity.AmqpTransportSettings.AmqpConnectionPoolSettings.PoolName;

            throw new NotImplementedException();
            
            //return $"{poolName}_{deviceIdentity.IotHubConnectionString.IotHubName}_";
        }
    }
}
