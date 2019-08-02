// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Azure.Devices.Client.Extensions;
using Microsoft.Azure.Devices.Common;
using Microsoft.Azure.Devices.Shared;
using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Devices.Client
{
    /// <summary>
    /// Authentication method that uses a shared access signature token stored within a Trusted Platform Module (TPM) and allows for token refresh.
    /// </summary>
    public sealed class DeviceAuthenticationWithTpm : DeviceAuthenticationWithTokenRefresh
    {
        private readonly SecurityProviderTpm _securityProvider;

        /// <summary>Initializes a new instance of the <see cref="DeviceAuthenticationWithTpm"/> class.</summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <param name="securityProvider">The security provider.</param>
        /// <exception cref="ArgumentNullException">securityProvider</exception>
        public DeviceAuthenticationWithTpm(
            string deviceId,
            SecurityProviderTpm securityProvider) : base(deviceId)
        {
            _securityProvider = securityProvider ?? throw new ArgumentNullException(nameof(securityProvider));
        }

        /// <summary>Initializes a new instance of the <see cref="DeviceAuthenticationWithTpm"/> class.</summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <param name="securityProvider">The security provider.</param>
        /// <param name="suggestedTimeToLiveSeconds">The suggested time to live seconds.</param>
        /// <param name="timeBufferPercentage">The time buffer percentage.</param>
        /// <exception cref="ArgumentNullException">securityProvider</exception>
        public DeviceAuthenticationWithTpm(
            string deviceId,
            SecurityProviderTpm securityProvider,
            int suggestedTimeToLiveSeconds,
            int timeBufferPercentage) : base(deviceId, suggestedTimeToLiveSeconds, timeBufferPercentage)
        {
            _securityProvider = securityProvider ?? throw new ArgumentNullException(nameof(securityProvider));
        }

        /// <summary>Allows the application to create a new token.</summary>
        /// <param name="iotHub">The iot hub.</param>
        /// <param name="suggestedTimeToLiveSeconds">The suggested time to live seconds.</param>
        /// <returns>The token.</returns>
        /// <remarks>It is guaranteed that this method is not called twice for the same token from different threads.</remarks>
        protected override Task<string> SafeCreateNewToken(string iotHub, int suggestedTimeToLiveSeconds)
        {
            var builder = new TpmSharedAccessSignatureBuilder(_securityProvider)
            {
                TimeToLive = TimeSpan.FromSeconds(suggestedTimeToLiveSeconds),
                Target = "{0}/devices/{1}".FormatInvariant(
                    iotHub,
                    WebUtility.UrlEncode(DeviceId)),
            };

            return Task.FromResult(builder.ToSignature());
        }

        private class TpmSharedAccessSignatureBuilder : SharedAccessSignatureBuilder
        {
            private readonly SecurityProviderTpm _securityProvider;

            public TpmSharedAccessSignatureBuilder(SecurityProviderTpm securityProvider)
            {
                _securityProvider = securityProvider;
            }

            protected override string Sign(string requestString, string key)
            {
                Debug.Assert(key == null);

                byte[] encodedBytes = Encoding.UTF8.GetBytes(requestString);
                byte[] hmac = _securityProvider.Sign(encodedBytes);
                return Convert.ToBase64String(hmac);
            }
        }
    }
}
