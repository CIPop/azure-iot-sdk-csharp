// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using DotNetty.Transport.Channels;

namespace Microsoft.Azure.Devices.Client.Transport.Mqtt
{
    /// <summary>Channel Message Exception</summary>
    public class ChannelMessageProcessingException : Exception
    {
        /// <summary>Initializes a new instance of the <see cref="ChannelMessageProcessingException"/> class.</summary>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="context">The context.</param>
        public ChannelMessageProcessingException(Exception innerException, IChannelHandlerContext context)
            : base(string.Empty, innerException)
        {
            this.Context = context;
        }

        /// <summary>Gets the context.</summary>
        /// <value>The context.</value>
        public IChannelHandlerContext Context { get; private set; }
    }
}
