// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using DotNetty.Codecs.Mqtt.Packets;

namespace Microsoft.Azure.Devices.Client.Transport.Mqtt
{
    /// <summary>MQTT Will Message</summary>
    /// <seealso cref="Microsoft.Azure.Devices.Client.Transport.Mqtt.WillMessage" />
    public interface IWillMessage
    {
        /// <summary>
        ///   <para>
        ///  Gets the message.
        /// </para>
        /// </summary>
        /// <value>The message.</value>
        Message Message { get; }

        /// <summary>Gets or sets the quality of service (QoS).</summary>
        /// <value>The quality of service.</value>
        QualityOfService QoS { get; set; }
    }
}
