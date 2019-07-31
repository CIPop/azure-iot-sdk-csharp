// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Azure.Devices.Common;
using Microsoft.Azure.Devices.Client.Exceptions;
using Microsoft.Azure.Devices.Client.Extensions;
using Microsoft.Azure.Devices.Client.Transport.AmqpIoT;

namespace Microsoft.Azure.Devices.Client.Transport.Amqp
{
    internal abstract class AmqpIoTConnection
    {
        public event EventHandler OnConnectionDisconnected;
        private AmqpIoTCbsLink _amqpIoTCbsLink;

        public AmqpIoTConnection()
        {
        }

#if false
        public AmqpUnit CreateAmqpUnit(
            DeviceIdentity deviceIdentity, 
            Func<MethodRequestInternal, Task> methodHandler, 
            Action<Twin, string, TwinCollection> twinMessageListener, 
            Func<string, Message, Task> eventListener)
        {
            if (Logging.IsEnabled) Logging.Enter(this, deviceIdentity, $"{nameof(CreateAmqpUnit)}");

            AmqpUnit amqpUnit = new AmqpUnit(
                deviceIdentity, 
                this,
                this,
                methodHandler,
                twinMessageListener, 
                eventListener);
            amqpUnit.OnUnitDisconnected += (o, args) =>
            {
                bool gracefulDisconnect = (bool)o;
                RemoveDevice(deviceIdentity, gracefulDisconnect);
            };

            _amqpUnits.Remove(deviceIdentity);
            _amqpUnits.Add(deviceIdentity, amqpUnit);
            if (Logging.IsEnabled) Logging.Exit(this, deviceIdentity, $"{nameof(CreateAmqpUnit)}");
            return amqpUnit;
        }

        public int GetNumberOfUnits()
        {
            int count = _amqpUnits.Count;
            if (Logging.IsEnabled) Logging.Info(this, count, $"{nameof(GetNumberOfUnits)}");
            return count;
        }

        private void OnConnectionClosed(object o, EventArgs args)
        {
            if (Logging.IsEnabled) Logging.Enter(this, o, $"{nameof(OnConnectionClosed)}");
            if (_amqpIoTConnection != null && ReferenceEquals(_amqpIoTConnection, o))
            {
                _amqpAuthenticationRefresher?.StopLoop();
                foreach (AmqpUnit unit in _amqpUnits.Values)
                {
                    unit.OnConnectionDisconnected();
                }
                _amqpUnits.Clear();
                OnConnectionDisconnected?.Invoke(this, EventArgs.Empty);
            }
            if (Logging.IsEnabled) Logging.Exit(this, o, $"{nameof(OnConnectionClosed)}");
        }

        private void RemoveDevice(DeviceIdentity deviceIdentity, bool gracefulDisconnect)
        {
            if (Logging.IsEnabled) Logging.Enter(this, deviceIdentity, $"{nameof(RemoveDevice)}");
            bool removed = _amqpUnits.Remove(deviceIdentity);
            if (removed && GetNumberOfUnits() == 0)
            {
                // TODO #887: handle gracefulDisconnect
                Shutdown();
            }
            if (Logging.IsEnabled) Logging.Exit(this, deviceIdentity, $"{nameof(RemoveDevice)}");
        }

        private void Shutdown()
        {
            if (Logging.IsEnabled) Logging.Enter(this, _amqpIoTConnection, $"{nameof(Shutdown)}");
            _amqpAuthenticationRefresher?.StopLoop();
            _amqpIoTConnection?.Abort();
            OnConnectionDisconnected?.Invoke(this, EventArgs.Empty);
            if (Logging.IsEnabled) Logging.Exit(this, _amqpIoTConnection, $"{nameof(Shutdown)}");
        }

        public async Task<IAmqpIoTAuthenticationRefresher> CreateRefresher(DeviceIdentity deviceIdentity, TimeSpan timeout)
        {
            if (Logging.IsEnabled) Logging.Enter(this, deviceIdentity, timeout, $"{nameof(CreateRefresher)}");
            if (_amqpIoTConnection == null)
            {
                throw new IotHubCommunicationException();
            }

            if (_amqpIoTCbsLink == null)
            {
                _amqpIoTCbsLink = _amqpIoTConnection.CreateCbsLink(deviceIdentity, timeout);
            }

            IAmqpIoTAuthenticationRefresher amqpAuthenticator = new AmqpAuthenticationRefresher(deviceIdentity, _amqpIoTCbsLink);
            await amqpAuthenticator.InitLoopAsync(timeout).ConfigureAwait(false);
            if (Logging.IsEnabled) Logging.Exit(this, deviceIdentity, timeout, $"{nameof(CreateRefresher)}");
            return amqpAuthenticator;
        }

        public async Task<AmqpIoTSession> CreateSession(DeviceIdentity deviceIdentity, TimeSpan timeout)
        {
            if (Logging.IsEnabled) Logging.Enter(this, deviceIdentity, timeout, $"{nameof(CreateSession)}");
            AmqpIoT.AmqpIoTConnection amqpIoTConnection = await EnsureConnection(timeout).ConfigureAwait(false);

            AmqpIoTSession amqpIoTSession = amqpIoTConnection.AddSession();

            if (Logging.IsEnabled) Logging.Associate(amqpIoTConnection, amqpIoTSession, $"{nameof(CreateSession)}");
            if (Logging.IsEnabled) Logging.Exit(this, deviceIdentity, timeout, $"{nameof(CreateSession)}");
            return amqpIoTSession;
        }

        public async Task<AmqpIoT.AmqpIoTConnection> EnsureConnection(TimeSpan timeout)
        {
            if (Logging.IsEnabled) Logging.Enter(this, timeout, $"{nameof(EnsureConnection)}");
            AmqpIoT.AmqpIoTConnection amqpIoTConnection = null;
            IAmqpIoTAuthenticationRefresher amqpAuthenticationRefresher = null;
            AmqpIoTCbsLink amqpIoTCbsLink = null;
            bool gain = await _lock.WaitAsync(timeout).ConfigureAwait(false);
            if (!gain)
            {
                throw new TimeoutException();
            }
            try
            {
                if (_amqpIoTConnection == null)
                {
                    if (Logging.IsEnabled) Logging.Info(this, "Creating new AmqpConnection", $"{nameof(EnsureConnection)}");
                    // Create AmqpConnection
                    amqpIoTConnection = await _amqpIoTConnector.OpenConnectionAsync(timeout).ConfigureAwait(false);

                    if (_deviceIdentity.AuthenticationModel != AuthenticationModel.X509Certificate)
                    {
                        if (_amqpIoTCbsLink == null)
                        {
                            if (Logging.IsEnabled) Logging.Info(this, "Creating new AmqpCbsLink", $"{nameof(EnsureConnection)}");
                            amqpIoTCbsLink = amqpIoTConnection.CreateCbsLink(_deviceIdentity, timeout);
                        }
                        else
                        {
                            amqpIoTCbsLink = _amqpIoTCbsLink;
                        }

                        if (_deviceIdentity.AuthenticationModel == AuthenticationModel.SharedAccessKeyIndividualIdentity)
                        {
                            if (Logging.IsEnabled) Logging.Info(this, "Creating connection width AmqpAuthenticationRefresher", $"{nameof(EnsureConnection)}");
                            amqpAuthenticationRefresher = new AmqpAuthenticationRefresher(_deviceIdentity, amqpIoTCbsLink);
                            await amqpAuthenticationRefresher.InitLoopAsync(timeout).ConfigureAwait(false);
                        }
                    }
                    _amqpIoTConnection = amqpIoTConnection;
                    _amqpIoTCbsLink = amqpIoTCbsLink;
                    _amqpAuthenticationRefresher = amqpAuthenticationRefresher;
                    _amqpIoTConnection.Closed += OnConnectionClosed;
                    if (Logging.IsEnabled) Logging.Associate(this, _amqpIoTConnection, $"{nameof(_amqpIoTConnection)}");
                    if (Logging.IsEnabled) Logging.Associate(this, _amqpIoTCbsLink, $"{nameof(_amqpIoTCbsLink)}");
                }
                else if (_amqpIoTConnection.IsClosing())
                {
                    throw new IotHubCommunicationException("AMQP connection is closing.");
                }
                else
                {
                    amqpIoTConnection = _amqpIoTConnection;
                }
            }
            catch (Exception ex) when (!ex.IsFatal())
            {
                amqpIoTCbsLink?.Close();
                amqpAuthenticationRefresher?.StopLoop();
                amqpIoTConnection?.Abort();
                throw;
            }
            finally
            {
                _lock.Release();
            }
            if (Logging.IsEnabled) Logging.Exit(this, timeout, $"{nameof(EnsureConnection)}");
            return amqpIoTConnection;
        }
#endif
    }
}
