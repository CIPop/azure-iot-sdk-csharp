﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Azure.Devices.Client.Transport.AmqpIoT;

#if !NETSTANDARD1_3
using System.Configuration;
#endif

namespace Microsoft.Azure.Devices.Client.Transport.Amqp
{
    internal class AmqpConnector
    {
        #region Members-Constructor
        const string DisableServerCertificateValidationKeyName = "Microsoft.Azure.Devices.DisableServerCertificateValidation";
        static readonly bool DisableServerCertificateValidation = InitializeDisableServerCertificateValidation();

        private AmqpIoT.AmqpIoTConnection _amqpIoTConnection;
        private bool _disposed;

        internal AmqpConnector(AmqpTransportSettings amqpTransportSettings, string hostName)
        {
            _amqpIoTConnection = new AmqpIoT.AmqpIoTConnection(amqpTransportSettings, hostName, DisableServerCertificateValidation);
        }
        #endregion

        #region Open-Close
        public async Task<AmqpIoT.AmqpIoTConnection> OpenConnectionAsync(TimeSpan timeout)
        {
            if (Logging.IsEnabled) Logging.Enter(this, timeout, $"{nameof(OpenConnectionAsync)}");

            await _amqpIoTConnection.OpenConnectionAsync(timeout).ConfigureAwait(false);

            return _amqpIoTConnection;
        }
        #endregion

        #region Authentication
        private static bool InitializeDisableServerCertificateValidation()
        {
#if !NET451
            bool flag;
            if (!AppContext.TryGetSwitch("DisableServerCertificateValidationKeyName", out flag))
            {
                return false;
            }
            return flag;
#else
            string value = ConfigurationManager.AppSettings[DisableServerCertificateValidationKeyName];
            if (!string.IsNullOrEmpty(value))
            {
                return bool.Parse(value);
            }
            return false;
#endif
        }
        #endregion

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (Logging.IsEnabled) Logging.Info(this, disposing, $"{nameof(Dispose)}");

            _disposed = true;
        }
    }
}
