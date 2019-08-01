﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Azure.Devices.Client
{
    using System;
    using Microsoft.Azure.Devices.Common;

    /// <summary>
    /// Creates an instance of an implementation of <see cref="IAuthenticationMethod"/> based on known authentication parameters.
    /// </summary>
    public sealed class AuthenticationMethodFactory
    {
        internal static IAuthenticationMethod GetAuthenticationMethod(IotHubConnectionStringBuilder iotHubConnectionStringBuilder)
        {
            if (iotHubConnectionStringBuilder.SharedAccessKeyName != null)
            {
                return new DeviceAuthenticationWithSharedAccessPolicyKey(
                    iotHubConnectionStringBuilder.DeviceId, iotHubConnectionStringBuilder.SharedAccessKeyName, iotHubConnectionStringBuilder.SharedAccessKey);
            }
            else if (iotHubConnectionStringBuilder.SharedAccessKey != null)
            {
#if !NETMF
                if(iotHubConnectionStringBuilder.ModuleId != null)
                {
                    return new ModuleAuthenticationWithRegistrySymmetricKey(
                        iotHubConnectionStringBuilder.DeviceId, iotHubConnectionStringBuilder.ModuleId, iotHubConnectionStringBuilder.SharedAccessKey);
                }
                else
#endif
                {
                    return new DeviceAuthenticationWithRegistrySymmetricKey(
                        iotHubConnectionStringBuilder.DeviceId, iotHubConnectionStringBuilder.SharedAccessKey);
                }
            }
            else if (iotHubConnectionStringBuilder.SharedAccessSignature != null)
            {
#if !NETMF
                if(iotHubConnectionStringBuilder.ModuleId != null)
                {
                    return new ModuleAuthenticationWithToken(
                        iotHubConnectionStringBuilder.DeviceId, iotHubConnectionStringBuilder.ModuleId, iotHubConnectionStringBuilder.SharedAccessSignature);
                }
                else
#endif
                {
                    return new DeviceAuthenticationWithToken(
                        iotHubConnectionStringBuilder.DeviceId, iotHubConnectionStringBuilder.SharedAccessSignature);
                }
            }
#if !NETMF
            else if (iotHubConnectionStringBuilder.UsingX509Cert)
            {
                return new DeviceAuthenticationWithX509Certificate(iotHubConnectionStringBuilder.DeviceId, iotHubConnectionStringBuilder.Certificate);
            }
#endif

#if NETMF
            throw new InvalidOperationException("Unsupported Authentication Method " + iotHubConnectionStringBuilder.ToString());
#else
            throw new InvalidOperationException("Unsupported Authentication Method {0}".FormatInvariant(iotHubConnectionStringBuilder));
#endif
        }

        /// <summary>
        /// Creates a <see cref="DeviceAuthenticationWithSharedAccessPolicyKey"/> instance based on the parameters.
        /// </summary>
        /// <param name="deviceId">Device Identifier.</param>
        /// <param name="policyName">Name of the shared access policy to use.</param>
        /// <param name="key">Key associated with the shared access policy.</param>
        /// <returns>A new instance of the <see cref="DeviceAuthenticationWithSharedAccessPolicyKey"/> class.</returns>
        public static IAuthenticationMethod CreateAuthenticationWithSharedAccessPolicyKey(string deviceId, string policyName, string key)
        {
            return new DeviceAuthenticationWithSharedAccessPolicyKey(deviceId, policyName, key);
        }

        /// <summary>
        /// Creates a <see cref="DeviceAuthenticationWithToken"/> instance based on the parameters.
        /// </summary>
        /// <param name="deviceId">Device Identifier.</param>
        /// <param name="token">Security token associated with the device.</param>
        /// <returns>A new instance of the <see cref="DeviceAuthenticationWithToken"/> class.</returns>
        public static IAuthenticationMethod CreateAuthenticationWithToken(string deviceId, string token)
        {
            return new DeviceAuthenticationWithToken(deviceId, token);
        }

#if !NETMF
        /// <summary>
        /// Creates a <see cref="ModuleAuthenticationWithToken"/> instance based on the parameters.
        /// </summary>
        /// <param name="deviceId">Device Identifier.</param>
        /// <param name="moduleId">Module Identifier.</param>
        /// <param name="token">Security token associated with the device.</param>
        /// <returns>A new instance of the <see cref="ModuleAuthenticationWithToken"/> class.</returns>
        public static IAuthenticationMethod CreateAuthenticationWithToken(string deviceId, string moduleId, string token)
        {
            return new ModuleAuthenticationWithToken(deviceId, moduleId, token);
        }
#endif

        /// <summary>
        /// Creates a <see cref="DeviceAuthenticationWithRegistrySymmetricKey"/> instance based on the parameters.
        /// </summary>
        /// <param name="deviceId">Device Identifier.</param>
        /// <param name="key">Key associated with the device in the device registry.</param>
        /// <returns>A new instance of the <see cref="DeviceAuthenticationWithRegistrySymmetricKey"/> class.</returns>
        public static IAuthenticationMethod CreateAuthenticationWithRegistrySymmetricKey(string deviceId, string key)
        {
            return new DeviceAuthenticationWithRegistrySymmetricKey(deviceId, key);
        }

#if !NETMF
        /// <summary>
        /// Creates a <see cref="ModuleAuthenticationWithRegistrySymmetricKey"/> instance based on the parameters.
        /// </summary>
        /// <param name="deviceId">Device Identifier.</param>
        /// <param name="moduleId">Module Identifier.</param>
        /// <param name="key">Key associated with the module in the device registry.</param>
        /// <returns>A new instance of the <see cref="ModuleAuthenticationWithRegistrySymmetricKey"/> class.</returns>
        public static IAuthenticationMethod CreateAuthenticationWithRegistrySymmetricKey(string deviceId, string moduleId, string key)
        {
            return new ModuleAuthenticationWithRegistrySymmetricKey(deviceId, moduleId, key);
        }
#endif
    }
}
