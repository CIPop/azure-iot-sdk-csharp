// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Newtonsoft.Json;

namespace Microsoft.Azure.Devices.Shared
{
    /// <summary>
    /// Represents <see cref="Twin"/> properties.
    /// </summary>
    public class TwinProperties
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TwinProperties"/> class.
        /// </summary>
        public TwinProperties()
        {
            Desired = new TwinCollection();
            Reported = new TwinCollection();
        }

        /// <summary>
        /// Gets or sets and sets the <see cref="Twin"/> desired properties.
        /// </summary>
        [JsonProperty(PropertyName = "desired", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public TwinCollection Desired { get; set; }

        /// <summary>
        /// Gets or sets and sets the <see cref="Twin"/> reported properties.
        /// </summary>
        [JsonProperty(PropertyName = "reported", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public TwinCollection Reported { get; set; }
    }
}
