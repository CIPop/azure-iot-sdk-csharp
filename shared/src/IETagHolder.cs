// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Azure.Devices.Shared
{
    /// <summary>
    /// ETag Holder Interface.
    /// </summary>
    public interface IETagHolder
    {
        /// <summary>
        /// Gets or sets eTag value.
        /// </summary>
        string ETag { get; set; }
    }
}
