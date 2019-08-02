// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Azure.Devices.Shared
{
    /// <summary>
    /// ETag Holder.
    /// </summary>
    public class ETagHolder : IETagHolder
    {
        /// <summary>
        /// Gets or sets eTag value.
        /// </summary>
        public string ETag { get; set; }
    }
}
