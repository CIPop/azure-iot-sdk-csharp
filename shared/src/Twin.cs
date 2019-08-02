// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Microsoft.Azure.Devices.Shared
{
    /// <summary>
    /// Twin Representation.
    /// </summary>
    [JsonConverter(typeof(TwinJsonConverter))]
    public class Twin : IETagHolder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Twin"/> class.
        /// Creates an instance of <see cref="Twin"/>.
        /// </summary>
        public Twin()
        {
            Tags = new TwinCollection();
            Properties = new TwinProperties();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Twin"/> class.
        /// </summary>
        /// <param name="deviceId">Device Id.</param>
        public Twin(string deviceId)
            : this()
        {
            DeviceId = deviceId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Twin"/> class.
        /// </summary>
        /// <param name="twinProperties">The twin properties.</param>
        public Twin(TwinProperties twinProperties)
        {
            Tags = new TwinCollection();
            Properties = twinProperties;
        }

        /// <summary>
        /// Gets or sets and sets the <see cref="Twin"/> Id.
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// Gets or sets and sets the <see cref="Twin" /> Module Id.
        /// </summary>
        public string ModuleId { get; set; }

        /// <summary>
        /// Gets or sets and sets the <see cref="Twin"/> tags.
        /// </summary>
        public TwinCollection Tags { get; set; }

        /// <summary>
        /// Gets or sets and sets the <see cref="Twin"/> properties.
        /// </summary>
        public TwinProperties Properties { get; set; }

        /// <summary>
        /// Gets the <see cref="Twin"/> configuration properties. These are read only.
        /// </summary>
        public IDictionary<string, ConfigurationInfo> Configurations { get; internal set; }

        /// <summary>
        /// Gets or sets the <see cref="Twin"/> capabilities. These are read only.
        /// </summary>
        public DeviceCapabilities Capabilities { get; set; }

        /// <summary>
        /// Gets or sets twin's ETag.
        /// </summary>
        public string ETag { get; set; }

        /// <summary>
        /// Gets or sets twin's Version.
        /// </summary>
        [DefaultValue(null)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public long? Version { get; set; }

        /// <summary>
        /// Gets the corresponding Device's Status.
        /// </summary>
        [DefaultValue(null)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public DeviceStatus? Status { get; internal set; }

        /// <summary>
        /// Gets reason, if any, for the corresponding Device to be in specified <see cref="Status"/>.
        /// </summary>
        [DefaultValue(null)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string StatusReason { get; internal set; }

        /// <summary>
        /// Gets time when the corresponding Device's <see cref="Status"/> was last updated.
        /// </summary>
        [DefaultValue(null)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public DateTime? StatusUpdatedTime { get; internal set; }

        /// <summary>
        /// Gets corresponding Device's ConnectionState.
        /// </summary>
        [DefaultValue(null)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [JsonConverter(typeof(StringEnumConverter))]
        public DeviceConnectionState? ConnectionState { get; internal set; }

        /// <summary>
        /// Gets time when the corresponding Device was last active.
        /// </summary>
        [DefaultValue(null)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public DateTime? LastActivityTime { get; internal set; }

        /// <summary>
        /// Gets number of messages sent to the corresponding Device from the Cloud.
        /// </summary>
        [DefaultValue(null)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public int? CloudToDeviceMessageCount { get; internal set; }

        /// <summary>
        /// Gets corresponding Device's authentication type.
        /// </summary>
        [DefaultValue(null)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public AuthenticationType? AuthenticationType { get; internal set; }

        /// <summary>
        /// Gets corresponding Device's X509 thumbprint.
        /// </summary>
        [DefaultValue(null)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public X509Thumbprint X509Thumbprint { get; internal set; }

        /// <summary>
        /// Gets the Twin as a JSON string.
        /// </summary>
        /// <param name="formatting">Optional. Formatting for the output JSON string.</param>
        /// <returns>JSON string.</returns>
        public string ToJson(Formatting formatting = Formatting.None)
        {
            return JsonConvert.SerializeObject(this, formatting);
        }
    }
}
