// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Azure.Devices.Client
{
    /// <summary>
    /// Connection status change reason supported by DeviceClient
    /// </summary>   
    public enum ConnectionStatusChangeReason
    {
        /// <summary>
        /// Connection established.
        /// </summary>
        Connection_Ok,
        /// <summary>
        /// The SAS token expired.
        /// </summary>
        Expired_SAS_Token,
        /// <summary>
        /// The device is disabled.
        /// </summary>
        Device_Disabled,
        /// <summary>
        /// Bad credential supplied during authentication.
        /// </summary>
        Bad_Credential,
        /// <summary>
        /// Stopping retries per the specified Retry Policy.
        /// </summary>
        Retry_Expired,
        /// <summary>
        /// No network available.
        /// </summary>
        No_Network,
        /// <summary>
        /// A communication error occured.
        /// </summary>
        Communication_Error,
        /// <summary>
        /// The application closed the client.
        /// </summary>
        Client_Close
    }
}
