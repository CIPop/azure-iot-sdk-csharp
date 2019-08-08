// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Text;

namespace Microsoft.Azure.Devices.Client
{
    /// <summary>The method request.</summary>
    public sealed class MethodRequest
    {
        /// <summary>Initializes a new instance of the <see cref="MethodRequest"/> class.</summary>
        /// <param name="name">The name.</param>
        public MethodRequest(string name) : this(name, null, null, null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="MethodRequest"/> class.</summary>
        /// <param name="name">The name.</param>
        /// <param name="data">The data.</param>
        public MethodRequest(string name, byte[] data) : this(name, data, null, null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="MethodRequest"/> class.</summary>
        /// <param name="name">The name.</param>
        /// <param name="responseTimeout">The response timeout.</param>
        /// <param name="connectionTimeout">The connection timeout.</param>
        public MethodRequest(string name, TimeSpan? responseTimeout, TimeSpan? connectionTimeout) : this(name, null, responseTimeout, connectionTimeout)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="MethodRequest"/> class.</summary>
        /// <param name="name">The name.</param>
        /// <param name="data">The data.</param>
        /// <param name="responseTimeout">The response timeout.</param>
        /// <param name="connectionTimeout">The connection timeout.</param>
        public MethodRequest(string name, byte[] data, TimeSpan? responseTimeout, TimeSpan? connectionTimeout)
        {
            this.Name = name;
            this.Data = data;
            this.ResponseTimeout = responseTimeout;
            this.ConnectionTimeout = connectionTimeout;
        }

        /// <summary>Gets the name.</summary>
        /// <value>The name.</value>
        public string Name { get; private set; }

        /// <summary>Gets the data.</summary>
        /// <value>The data.</value>
        public byte[] Data { get; private set; }

        /// <summary>Gets the response timeout.</summary>
        /// <value>The response timeout.</value>
        public TimeSpan? ResponseTimeout { get; private set; }

        /// <summary>Gets the connection timeout.</summary>
        /// <value>The connection timeout.</value>
        public TimeSpan? ConnectionTimeout { get; private set; }

        /// <summary>Gets the data as json.</summary>
        /// <value>The data as json.</value>
        public string DataAsJson
        {
            get { return (Data == null || Data.Length == 0) ? null : Encoding.UTF8.GetString(Data); }
        }
    }
}
