// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Azure.Devices.Client.Extensions;
using Microsoft.Azure.Devices.Shared;
using System.Net;

namespace Microsoft.Azure.Devices.Client.Transport
{
    /// <summary>
    /// Device configurations
    /// Stores the common attributes
    /// - connection string 
    /// - transport settings 
    /// </summary>
    internal class DeviceIdentity
    {
        internal IotHubConnectionString IotHubConnectionString { get; }
        internal AmqpTransportSettings AmqpTransportSettings { get; }
        internal ProductInfo ProductInfo { get; }
        internal string Audience { get; }
        internal DeviceIdentity(IotHubConnectionString iotHubConnectionString, AmqpTransportSettings amqpTransportSettings, ProductInfo productInfo)
        {
        }
    }
}
