// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Azure.Devices.Client
{
    /// <summary>
    /// Status code for Method Response
    /// </summary>
    public enum MethodResposeStatusCode
    {
        /// <summary>Bad request</summary>
        BadRequest = 400,

        /// <summary>Application internal error</summary>
        UserCodeException = 500,

        /// <summary>Method not implemented</summary>
        MethodNotImplemented = 501
    }
}
