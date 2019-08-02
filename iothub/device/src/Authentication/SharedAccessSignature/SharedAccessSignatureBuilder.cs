// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Azure.Devices.Client
{
    using System;
    using System.Net;
    using Microsoft.Azure.Devices.Client.Extensions;
    using Microsoft.Azure.Devices.Common;
#if !NETMF
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography;
#endif
    using System.Globalization;
    using System.Text;

    /// <summary>A class that allows creation of Shared Access Signature strings.</summary>
    public class SharedAccessSignatureBuilder
    {
        private string key;

        /// <summary>Initializes a new instance of the <see cref="SharedAccessSignatureBuilder"/> class.</summary>
        public SharedAccessSignatureBuilder()
        {
#if NETMF
            this.TimeToLive = new TimeSpan(0, 60, 0);
#else
            this.TimeToLive = TimeSpan.FromMinutes(60);
#endif
        }

        /// <summary>Gets or sets the name of the key.</summary>
        /// <value>The name of the key.</value>
        public string KeyName { get; set; }

        /// <summary>Gets or sets the BASE64 encoded key.</summary>
        /// <value>The key.</value>
        public string Key
        {
            get
            {
                return this.key;
            }

            set
            {
#if !NETMF
                StringValidationHelper.EnsureBase64String(value, "Key");
#endif
                this.key = value;
            }
        }

        /// <summary>Gets or sets the target.</summary>
        /// <value>The target.</value>
        public string Target { get; set; }

        /// <summary>Gets or sets the time to live.</summary>
        /// <value>The time to live.</value>
        public TimeSpan TimeToLive { get; set; }

        /// <summary>Converts to signature.</summary>
        /// <returns>The signature.</returns>
        public string ToSignature()
        {
            return BuildSignature(this.KeyName, this.Key, this.Target, this.TimeToLive);
        }

        private string BuildSignature(string keyName, string key, string target, TimeSpan timeToLive)
        {
            string expiresOn = BuildExpiresOn(timeToLive);
            string audience = WebUtility.UrlEncode(target);

#if !NETMF
            List<string> fields = new List<string>
            {
                audience,
                expiresOn
            };
#endif

            // Example string to be signed:
            // dh://myiothub.azure-devices.net/a/b/c?myvalue1=a
            // <Value for ExpiresOn>

#if NETMF
            string signature = Sign(audience + "\n" + expiresOn, key);
#else
            string signature = Sign(string.Join("\n", fields), key);
#endif

            // Example returned string:
            // SharedAccessSignature sr=ENCODED(dh://myiothub.azure-devices.net/a/b/c?myvalue1=a)&sig=<Signature>&se=<ExpiresOnValue>[&skn=<KeyName>]

#if NETMF
            var buffer = new StringBuilder();
            buffer.Append(SharedAccessSignatureConstants.SharedAccessSignature + " ");
            buffer.Append(SharedAccessSignatureConstants.AudienceFieldName + "=" + audience);
            buffer.Append("&" + SharedAccessSignatureConstants.SignatureFieldName + "=" + WebUtility.UrlEncode(signature));
            buffer.Append("&" + SharedAccessSignatureConstants.ExpiryFieldName + "=" + WebUtility.UrlEncode(expiresOn));

            if (!keyName.IsNullOrWhiteSpace())
            {
                buffer.Append("&" + SharedAccessSignatureConstants.KeyNameFieldName + "=" + WebUtility.UrlEncode(keyName));
            }
#else
            var buffer = new StringBuilder();
            buffer.AppendFormat(CultureInfo.InvariantCulture, "{0} {1}={2}&{3}={4}&{5}={6}",
                SharedAccessSignatureConstants.SharedAccessSignature,
                SharedAccessSignatureConstants.AudienceFieldName, audience,
                SharedAccessSignatureConstants.SignatureFieldName, WebUtility.UrlEncode(signature),
                SharedAccessSignatureConstants.ExpiryFieldName, WebUtility.UrlEncode(expiresOn));

            if (!keyName.IsNullOrWhiteSpace())
            {
                buffer.AppendFormat(CultureInfo.InvariantCulture, "&{0}={1}",
                    SharedAccessSignatureConstants.KeyNameFieldName, WebUtility.UrlEncode(keyName));
            }
#endif

            return buffer.ToString();
        }

        private string BuildExpiresOn(TimeSpan timeToLive)
        {
#if MF_FRAMEWORK_VERSION_V4_3
            // .NETMF < v4.4 had a know bug with DateTime.Kind: values were always created with DateTimeKind.Local 
            // this requires us to perform an extra step to make a DateTime to be in UTC, otherwise the expiry date will be calculated wrongly

            // the 'absolute' value is correct but DateTimeKind is Local (WRONG!)
            DateTime expiresOn = TimeZone.CurrentTimeZone.ToUniversalTime(DateTime.UtcNow.Add(timeToLive));
            TimeSpan secondsFromBaseTime = expiresOn.Subtract(TimeZone.CurrentTimeZone.ToUniversalTime(SharedAccessSignatureConstants.EpochTime));
            return (secondsFromBaseTime.Ticks / TimeSpan.TicksPerSecond).ToString();
#elif MF_FRAMEWORK_VERSION_V4_4
            DateTime expiresOn = DateTime.UtcNow.Add(timeToLive);
            TimeSpan secondsFromBaseTime = expiresOn.Subtract(SharedAccessSignatureConstants.EpochTime);
            return ((uint)(secondsFromBaseTime.Ticks / TimeSpan.TicksPerSecond)).ToString();
#else
            DateTime expiresOn = DateTime.UtcNow.Add(timeToLive);
            TimeSpan secondsFromBaseTime = expiresOn.Subtract(SharedAccessSignatureConstants.EpochTime);
            long seconds = Convert.ToInt64(secondsFromBaseTime.TotalSeconds, CultureInfo.InvariantCulture);
            return Convert.ToString(seconds, CultureInfo.InvariantCulture);
#endif
        }
#if NETMF
        private string Sign(string requestString, string key)
        {
            // computing SHA256 signature using a managed code library
            var hmac = SHA.computeHMAC_SHA256(Convert.FromBase64String(key), Encoding.UTF8.GetBytes(requestString));
            return Convert.ToBase64String(hmac);
        }
#else
        /// <summary>Signs the specified request string using the specified key.</summary>
        /// <param name="requestString">The request string.</param>
        /// <param name="key">The key.</param>
        /// <returns>The HMACSHA256 signed result.</returns>
        /// <remarks>This can be overriden to implement custom key storage such as TPM or other Hardware Security Modules.</remarks>
        protected virtual string Sign(string requestString, string key)
        {
            using (var algorithm = new HMACSHA256(Convert.FromBase64String(key)))
            {
                return Convert.ToBase64String(algorithm.ComputeHash(Encoding.UTF8.GetBytes(requestString)));
            }
        }
#endif
    }
}
