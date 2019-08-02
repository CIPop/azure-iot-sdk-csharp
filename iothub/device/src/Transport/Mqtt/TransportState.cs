// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Microsoft.Azure.Devices.Client.Transport.Mqtt
{
    /// <summary>MQTT transport state.</summary>
    [Flags]
    public enum TransportState
    {
        /// <summary>
        /// Transport not initialized.
        /// </summary>
        NotInitialized = 1,
        /// <summary>
        /// Transport opening.
        /// </summary>
        Opening = 2,
        /// <summary>
        /// Transport opened.
        /// </summary>
        Open = 4,
        /// <summary>
        /// Transport subscribing.
        /// </summary>
        Subscribing = Open | 8,
        /// <summary>
        /// Transport receiving.
        /// </summary>
        Receiving = Open | 16,
        /// <summary>
        /// Transport closed.
        /// </summary>
        Closed = 32,
        /// <summary>
        /// Transport faulted.
        /// </summary>
        Error = 64
    }
}
