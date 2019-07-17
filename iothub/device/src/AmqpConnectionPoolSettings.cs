// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Microsoft.Azure.Devices.Client
{
    /// <summary>
    /// Configuration for the AMQP IoTHub Transport.
    /// </summary>
    public sealed class AmqpConnectionPoolSettings
    {
        private static readonly TimeSpan DefaultConnectionIdleTimeout = TimeSpan.FromMinutes(2);
        private static readonly TimeSpan MinConnectionIdleTimeout = TimeSpan.FromSeconds(5);
        private const uint DefaultPoolSize = 100;
        private const uint MaxNumberOfPools = ushort.MaxValue;

        private uint _maxPoolSize;
        private TimeSpan _connectionIdleTimeout;

        /// <summary>
        /// Initializes a new instance of the <see cref="AmqpConnectionPoolSettings"/> class.
        /// </summary>
        public AmqpConnectionPoolSettings()
        {
            this._maxPoolSize = DefaultPoolSize;
            this.Pooling = false;
            this._connectionIdleTimeout = DefaultConnectionIdleTimeout;
        }

        /// <summary>
        /// Gets or sets the maximum size of the pool.
        /// </summary>
        /// <value>
        /// The maximum size of the pool.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">value</exception>
        public uint MaxPoolSize
        {
            get { return this._maxPoolSize; }

            set
            {
                if (value > 0 && value <= MaxNumberOfPools)
                {
                    this._maxPoolSize = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
            }
        }

        public bool Pooling { get; set; }

        public TimeSpan ConnectionIdleTimeout
        {
            get { return this._connectionIdleTimeout; }

            set
            {
                if (value >= MinConnectionIdleTimeout)
                {
                    this._connectionIdleTimeout = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("value");
                }
            }
        }

        public string PoolName {
            get;
            set; // TODO: check upperCase/Lowercase only.
        }

        public bool Equals(AmqpConnectionPoolSettings other)
        {
            if (other == null)
            {
                return false;
            }

            if (object.ReferenceEquals(this, other))
            {
                return true;
            }

            return (this.Pooling == other.Pooling && this.MaxPoolSize == other.MaxPoolSize && this.ConnectionIdleTimeout == other.ConnectionIdleTimeout);
        }
    }
}