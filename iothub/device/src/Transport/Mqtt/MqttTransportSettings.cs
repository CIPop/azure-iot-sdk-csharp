// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using DotNetty.Codecs.Mqtt.Packets;
using Microsoft.Azure.Devices.Client.Extensions;
using Microsoft.Azure.Devices.Common;
using Microsoft.Azure.Devices.Shared;
using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Azure.Devices.Client.Transport.Mqtt
{
    using TransportType = Microsoft.Azure.Devices.Client.TransportType;

    /// <summary>The MQTT transport settings.</summary>
    /// <seealso cref="Microsoft.Azure.Devices.Client.ITransportSettings" />
    public class MqttTransportSettings : ITransportSettings
    {
        readonly TransportType transportType;
        const bool DefaultCleanSession = false;
        const bool DefaultDeviceReceiveAckCanTimeout = false;
        const bool DefaultHasWill = false;
        const bool DefaultMaxOutboundRetransmissionEnforced = false;
        const int DefaultKeepAliveInSeconds = 300;
        const int DefaultReceiveTimeoutInSeconds = 60;
        const int DefaultMaxPendingInboundMessages = 50;
        const QualityOfService DefaultPublishToServerQoS = QualityOfService.AtLeastOnce;
        const QualityOfService DefaultReceivingQoS = QualityOfService.AtLeastOnce;
        static readonly TimeSpan DefaultConnectArrivalTimeout = TimeSpan.FromSeconds(300);
        static readonly TimeSpan DefaultDeviceReceiveAckTimeout = TimeSpan.FromSeconds(300);

        /// <summary>Initializes a new instance of the <see cref="MqttTransportSettings"/> class.</summary>
        /// <param name="transportType">Type of the transport.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// transportType - Must specify Mqtt_WebSocket_Only or Mqtt_Tcp_Only
        /// or
        /// transportType - Unsupported Transport Type {0}".FormatInvariant(transportType)
        /// </exception>
        public MqttTransportSettings(TransportType transportType)
        {
            this.transportType = transportType;

            switch (transportType)
            {
                case TransportType.Mqtt_WebSocket_Only:
                    this.Proxy = DefaultWebProxySettings.Instance;
                    this.transportType = transportType;
                    break;
                case TransportType.Mqtt_Tcp_Only:
                    this.transportType = transportType;
                    break;
                case TransportType.Mqtt:
                    throw new ArgumentOutOfRangeException(nameof(transportType), transportType, "Must specify Mqtt_WebSocket_Only or Mqtt_Tcp_Only");
                default:
                    throw new ArgumentOutOfRangeException(nameof(transportType), transportType, "Unsupported Transport Type {0}".FormatInvariant(transportType));
            }

            this.CleanSession = DefaultCleanSession;
            this.ConnectArrivalTimeout = DefaultConnectArrivalTimeout;
            this.DeviceReceiveAckCanTimeout = DefaultDeviceReceiveAckCanTimeout;
            this.DeviceReceiveAckTimeout = DefaultDeviceReceiveAckTimeout;
            this.DupPropertyName = "mqtt-dup";
            this.HasWill = DefaultHasWill;
            this.KeepAliveInSeconds = DefaultKeepAliveInSeconds;
            this.MaxOutboundRetransmissionEnforced = DefaultMaxOutboundRetransmissionEnforced;
            this.MaxPendingInboundMessages = DefaultMaxPendingInboundMessages;
            this.PublishToServerQoS = DefaultPublishToServerQoS;
            this.ReceivingQoS = DefaultReceivingQoS;
            this.QoSPropertyName = "mqtt-qos";
            this.RetainPropertyName = "mqtt-retain";
            this.WillMessage = null;
            this.DefaultReceiveTimeout = TimeSpan.FromSeconds(DefaultReceiveTimeoutInSeconds);
        }

        /// <summary>Gets or sets a value indicating whether a receive acknowledgement can timeout.</summary>
        /// <value>
        ///   <c>true</c> if device receive ack can timeout; otherwise, <c>false</c>.</value>
        public bool DeviceReceiveAckCanTimeout { get; set; }

        /// <summary>Gets or sets the device receive ack timeout.</summary>
        /// <value>The device receive ack timeout.</value>
        public TimeSpan DeviceReceiveAckTimeout { get; set; }

        /// <summary>Gets or sets the publish to server QoS.</summary>
        /// <value>The publish to server QoS.</value>
        public QualityOfService PublishToServerQoS { get; set; }

        /// <summary>Gets or sets the receiving QoS.</summary>
        /// <value>The receiving QoS.</value>
        public QualityOfService ReceivingQoS { get; set; }

        /// <summary>Gets or sets the name of the retain property.</summary>
        /// <value>The name of the retain property.</value>
        public string RetainPropertyName { get; set; }

        /// <summary>Gets or sets the name of the duplicate property.</summary>
        /// <value>The name of the duplicate property.</value>
        /// <remarks>This is set if a duplicate message is received.</remarks>
        public string DupPropertyName { get; set; }

        /// <summary>Gets or sets the name of the QoS property.</summary>
        /// <value>The name of the QoS property.</value>
        public string QoSPropertyName { get; set; }

        /// <summary>Gets or sets a value indicating whether maximum outbound retransmission is enforced.</summary>
        /// <value>
        ///   <c>true</c> if [maximum outbound retransmission enforced]; otherwise, <c>false</c>.</value>
        public bool MaxOutboundRetransmissionEnforced { get; set; }

        /// <summary>Gets or sets the maximum pending inbound messages.</summary>
        /// <value>The maximum pending inbound messages.</value>
        public int MaxPendingInboundMessages { get; set; }

        /// <summary>Gets or sets the connect arrival timeout.</summary>
        /// <value>The connect arrival timeout.</value>
        public TimeSpan ConnectArrivalTimeout { get; set; }

        /// <summary>Gets or sets a value indicating whether [clean session].</summary>
        /// <value>
        ///   <c>true</c> if [clean session]; otherwise, <c>false</c>.</value>
        public bool CleanSession { get; set; }

        /// <summary>Gets or sets the keep alive in seconds.</summary>
        /// <value>The keep alive in seconds.</value>
        public int KeepAliveInSeconds { get; set; }

        /// <summary>Gets or sets a value indicating whether this instance has an MQTT will message.</summary>
        /// <value>
        ///   <c>true</c> if this instance has a will message; otherwise, <c>false</c>.</value>
        public bool HasWill { get; set; }

        /// <summary>Gets or sets the will message.</summary>
        /// <value>The will message.</value>
        public IWillMessage WillMessage { get; set; }

        /// <summary>Returns the transport type of the TransportSettings object.</summary>
        /// <returns>The TransportType</returns>
        public TransportType GetTransportType()
        {
            return this.transportType;
        }

        /// <summary>Gets or sets the default receive timeout.</summary>
        /// <value>The default receive timeout.</value>
        public TimeSpan DefaultReceiveTimeout { get; set; }

        /// <summary>Gets or sets the remote certificate validation callback.</summary>
        /// <value>The remote certificate validation callback.</value>
        public RemoteCertificateValidationCallback RemoteCertificateValidationCallback { get; set; }

        /// <summary>Gets or sets the client certificate.</summary>
        /// <value>The client certificate.</value>
        public X509Certificate ClientCertificate { get; set; }

        /// <summary>Gets or sets the proxy.</summary>
        /// <value>The proxy.</value>
        public IWebProxy Proxy { get; set; }
    }
}
