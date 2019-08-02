// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;

using Microsoft.Azure.Devices.Shared;

namespace Microsoft.Azure.Devices.Client
{
    /// <summary>
    /// contains Http1 transport-specific settings for DeviceClient
    /// </summary>
    public sealed class Http1TransportSettings : ITransportSettings
    {
        static readonly TimeSpan DefaultOperationTimeout = TimeSpan.FromSeconds(60);

        /// <summary>Initializes a new instance of the <see cref="Http1TransportSettings"/> class.</summary>
        public Http1TransportSettings()
        {
            this.Proxy = DefaultWebProxySettings.Instance;
        }

        /// <summary>Returns the transport type of the TransportSettings object.</summary>
        /// <returns>The TransportType</returns>
        public TransportType GetTransportType()
        {
            return TransportType.Http1;
        }

        /// <summary>Gets or sets the client certificate.</summary>
        /// <value>The client certificate.</value>
        public X509Certificate2 ClientCertificate { get; set; }

        /// <summary>The default receive timeout.</summary>
        public TimeSpan DefaultReceiveTimeout => DefaultOperationTimeout;

        /// <summary>Gets or sets the proxy.</summary>
        /// <value>The proxy.</value>
        public IWebProxy Proxy { get; set; }
    }
}
