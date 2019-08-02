// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using DotNetty.Codecs.Mqtt.Packets;

namespace Microsoft.Azure.Devices.Client.Transport.Mqtt
{
    /// <summary>MQTT Will Message</summary>
    /// <seealso cref="Microsoft.Azure.Devices.Client.Transport.Mqtt.IWillMessage" />
    public class WillMessage : IWillMessage
    {
        /// <summary>Gets the message.</summary>
        /// <value>The message.</value>
        public Message Message { get; private set; }

        /// <summary>Gets or sets the quality of service (QoS).</summary>
        /// <value>The quality of service.</value>
        public QualityOfService QoS { get; set; }

        /// <summary>Initializes a new instance of the <see cref="WillMessage"/> class.</summary>
        /// <param name="qos">The qos.</param>
        /// <param name="message">The message.</param>
        public WillMessage(QualityOfService qos, Message message)
        {
            QoS = qos;
            Message = message;
        }
    }
}
