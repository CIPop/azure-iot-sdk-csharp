// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Microsoft.Azure.Devices.Shared
{
    /// <summary>
    /// Specifies the configuration status.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    [SuppressMessage(
        "Microsoft.Naming",
        "CA1717:OnlyFlagsEnumsShouldHavePluralNames",
        Justification = "Status is singular.")]
#pragma warning disable CA1008 // Enums should have zero value (Reason: AppCompat, public API).
    public enum ConfigurationStatus
#pragma warning restore CA1008 // Enums should have zero value
    {
        /// <summary>
        /// Configuration targeted.
        /// </summary>
        Targeted = 1,

        /// <summary>
        /// Configuration applied.
        /// </summary>
        Applied = 2,
    }
}
