﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

using DateTimeT = System.DateTime;

namespace Microsoft.Azure.Devices.Shared
{
    /// <summary>
    /// <see cref="Metadata"/> for properties in <see cref="TwinCollection"/>.
    /// </summary>
    [SuppressMessage(
        "Microsoft.Design",
        "CA1724:TypeNamesShouldNotMatchNamespaces",
        Justification = "Public API cannot change name.")]
    public sealed class Metadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Metadata"/> class.
        /// </summary>
        /// <param name="lastUpdated">Last updated time.</param>
        /// <param name="lastUpdatedVersion">Last updated version.</param>
        public Metadata(DateTimeT lastUpdated, long? lastUpdatedVersion)
        {
            LastUpdated = lastUpdated;
            LastUpdatedVersion = lastUpdatedVersion;
        }

        /// <summary>
        /// Gets or sets time when a property was last updated.
        /// </summary>
        public DateTimeT LastUpdated { get; set; }

        /// <summary>
        /// Gets or sets the last updated version.
        /// </summary>
        /// <remarks>
        /// This SHOULD be null for Reported properties metadata and MUST not be null for Desired properties metadata.
        /// </remarks>
        public long? LastUpdatedVersion { get; set; }
    }
}
