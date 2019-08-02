﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client.Extensions;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Azure.Devices.Client.Transport.AmqpIoT;

namespace Microsoft.Azure.Devices.Client.Transport.Amqp
{
#pragma warning disable IDE0060 // Remove unused parameter - TODO - REMOVE this pragma when implementation is ready.

    internal class AmqpTransportHandler : TransportHandler
    {
        #region Members-Constructor
        private const int ResponseTimeoutInSeconds = 300;
        private readonly ConcurrentDictionary<string, TaskCompletionSource<Twin>> _twinResponseCompletions = new ConcurrentDictionary<string, TaskCompletionSource<Twin>>();

#pragma warning disable CA1810 // Initialize reference type static fields inline: We use the static ctor to have init-once semantics.
        static AmqpTransportHandler()
#pragma warning restore CA1810 // Initialize reference type static fields inline
        {
            try
            {
                AmqpIoTTrace.SetProvider(new AmqpIoTTransportLog());
            }
            catch (Exception ex)
            {
                // Do not throw from static ctor.
                if (Logging.IsEnabled) Logging.Error(null, ex, nameof(AmqpTransportHandler));
            }
        }

        internal AmqpTransportHandler(
            IPipelineContext context,
            IotHubConnectionString connectionString,
            AmqpTransportSettings transportSettings,
            Func<MethodRequestInternal, Task> methodHandler = null,
            Action<TwinCollection> desiredPropertyListener = null,
            Func<string, Message, Task> eventListener = null)
            : base(context, transportSettings)
        {
        }
        #endregion


        #region Open-Close
        public override async Task OpenAsync(CancellationToken cancellationToken)
        {
            if (Logging.IsEnabled) Logging.Enter(this, cancellationToken, $"{nameof(OpenAsync)}");

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                // TODO
                await Task.Yield();
                throw new NotImplementedException();

            }
            catch (Exception exception) when (!exception.IsFatal() && !(exception is OperationCanceledException))
            {
                Exception newException = AmqpIoTExceptionAdapter.ConvertToIoTHubException(exception);
                if (newException != exception)
                {
                    throw newException;
                }
                else
                {
                    // Maintain the original stack.
                    throw;
                }
            }
            finally
            {
                if (Logging.IsEnabled) Logging.Exit(this, cancellationToken, $"{nameof(OpenAsync)}");
            }
        }

        public override async Task CloseAsync(CancellationToken cancellationToken)
        {
            if (Logging.IsEnabled) Logging.Enter(this, $"{nameof(CloseAsync)}");

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                OnTransportClosedGracefully();

                // TODO
                await Task.Yield();
                throw new NotImplementedException();


            }
            finally
            {
                if (Logging.IsEnabled) Logging.Exit(this, $"{nameof(CloseAsync)}");
            }
        }
        #endregion

        #region Telemetry
        public override async Task SendEventAsync(Message message, CancellationToken cancellationToken)
        {
            if (Logging.IsEnabled) Logging.Enter(this, message, cancellationToken, $"{nameof(SendEventAsync)}");

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                // TODO
                await Task.Yield();
                throw new NotImplementedException();

            }
            finally
            {
                if (Logging.IsEnabled) Logging.Exit(this, message, cancellationToken, $"{nameof(SendEventAsync)}");
            }
        }

        public override async Task SendEventAsync(IEnumerable<Message> messages, CancellationToken cancellationToken)
        {
            if (Logging.IsEnabled) Logging.Enter(this, messages, cancellationToken, $"{nameof(SendEventAsync)}");

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                // TODO
                await Task.Yield();
                throw new NotImplementedException();


            }
            finally
            {
                if (Logging.IsEnabled) Logging.Exit(this, messages, cancellationToken, $"{nameof(SendEventAsync)}");
            }
        }

        public override async Task<Message> ReceiveAsync(TimeSpan timeout, CancellationToken cancellationToken)
        {
            if (Logging.IsEnabled) Logging.Enter(this, timeout, cancellationToken, $"{nameof(ReceiveAsync)}");

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                //TODO: 
                await Task.Yield();
                throw new NotImplementedException();

            }
            finally
            {
                if (Logging.IsEnabled) Logging.Exit(this, timeout, cancellationToken, $"{nameof(ReceiveAsync)}");
            }
        }
        #endregion

        #region Methods
        public override async Task EnableMethodsAsync(CancellationToken cancellationToken)
        {
            if (Logging.IsEnabled) Logging.Enter(this, cancellationToken, $"{nameof(EnableMethodsAsync)}");

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                //TODO: 
                await Task.Yield();
                throw new NotImplementedException();


            }
            catch (Exception exception) when (!exception.IsFatal() && !(exception is OperationCanceledException))
            {
                throw AmqpIoTExceptionAdapter.ConvertToIoTHubException(exception);
            }
            finally
            {
                if (Logging.IsEnabled) Logging.Exit(this, cancellationToken, $"{nameof(EnableMethodsAsync)}");
            }
        }

        public override async Task DisableMethodsAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (Logging.IsEnabled) Logging.Enter(this, cancellationToken, $"{nameof(DisableMethodsAsync)}");

                cancellationToken.ThrowIfCancellationRequested();
                //TODO: 
                await Task.Yield();
                throw new NotImplementedException();
            }
            finally
            {
                if (Logging.IsEnabled) Logging.Exit(this, cancellationToken, $"{nameof(DisableMethodsAsync)}");
            }
        }

        public override async Task SendMethodResponseAsync(MethodResponseInternal methodResponse, CancellationToken cancellationToken)
        {
            if (Logging.IsEnabled) Logging.Enter(this, methodResponse, cancellationToken, $"{nameof(SendMethodResponseAsync)}");

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                //TODO: 
                await Task.Yield();
                throw new NotImplementedException();
            }
            finally
            {
                if (Logging.IsEnabled) Logging.Exit(this, methodResponse, cancellationToken, $"{nameof(SendMethodResponseAsync)}");
            }
        }
        #endregion

        #region Twin
        public override async Task EnableTwinPatchAsync(CancellationToken cancellationToken)
        {
            if (Logging.IsEnabled) Logging.Enter(this, cancellationToken, $"{nameof(EnableTwinPatchAsync)}");

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                //TODO: 
                await Task.Yield();
                throw new NotImplementedException();


            }
            catch (Exception exception) when (!exception.IsFatal() && !(exception is OperationCanceledException))
            {
                throw AmqpIoTExceptionAdapter.ConvertToIoTHubException(exception);
            }
            finally
            {
                if (Logging.IsEnabled) Logging.Exit(this, cancellationToken, $"{nameof(EnableTwinPatchAsync)}");
            }
        }

        public override async Task<Twin> SendTwinGetAsync(CancellationToken cancellationToken)
        {
            if (Logging.IsEnabled) Logging.Enter(this, cancellationToken, $"{nameof(SendTwinGetAsync)}");
            try
            {
                await EnableTwinPatchAsync(cancellationToken).ConfigureAwait(false);

                Twin twin = await RoundTripTwinMessage(AmqpTwinMessageType.Get, null, cancellationToken).ConfigureAwait(false);
                if (twin == null)
                {
                    throw new InvalidOperationException("Service rejected the message");
                }
                return twin;
            }
            catch (Exception exception) when (!exception.IsFatal() && !(exception is OperationCanceledException))
            {
                throw AmqpIoTExceptionAdapter.ConvertToIoTHubException(exception);
            }
            finally
            {
                if (Logging.IsEnabled) Logging.Exit(this, cancellationToken, $"{nameof(SendTwinGetAsync)}");
            }
        }

        public override async Task SendTwinPatchAsync(TwinCollection reportedProperties, CancellationToken cancellationToken)
        {
            if (Logging.IsEnabled) Logging.Enter(this, reportedProperties, cancellationToken, $"{nameof(SendTwinPatchAsync)}");
            try
            {
                await EnableTwinPatchAsync(cancellationToken).ConfigureAwait(false);
                Twin twin = await RoundTripTwinMessage(AmqpTwinMessageType.Patch, reportedProperties, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception) when (!exception.IsFatal() && !(exception is OperationCanceledException))
            {
                throw AmqpIoTExceptionAdapter.ConvertToIoTHubException(exception);
            }
            finally
            {
                if (Logging.IsEnabled) Logging.Exit(this, reportedProperties, cancellationToken, $"{nameof(SendTwinPatchAsync)}");
            }
        }

        private async Task<Twin> RoundTripTwinMessage(AmqpTwinMessageType amqpTwinMessageType, TwinCollection reportedProperties, CancellationToken cancellationToken)
        {
            if (Logging.IsEnabled) Logging.Enter(this, cancellationToken, $"{nameof(RoundTripTwinMessage)}");
            string correlationId = Guid.NewGuid().ToString();
            Twin response = null;

            try
            {
                var taskCompletionSource = new TaskCompletionSource<Twin>();
                _twinResponseCompletions[correlationId] = taskCompletionSource;


                var receivingTask = taskCompletionSource.Task;
                if (await Task.WhenAny(receivingTask, Task.Delay(TimeSpan.FromSeconds(ResponseTimeoutInSeconds))).ConfigureAwait(false) == receivingTask)
                {
                    // Task completed within timeout.
                    // Consider that the task may have faulted or been canceled.
                    // We re-await the task so that any exceptions/cancellation is rethrown.
                    response = await receivingTask.ConfigureAwait(false);
                    if (response == null)
                    {
                        throw new InvalidOperationException("Service response is null");
                    }
                }
                else
                {
                    // Timeout happen
                    throw new TimeoutException();
                }
            }
            finally
            {
                _twinResponseCompletions.TryRemove(correlationId, out _);
                if (Logging.IsEnabled) Logging.Exit(this, cancellationToken, $"{nameof(RoundTripTwinMessage)}");
            }

            return response;
        }
        #endregion

        #region Events
        public override async Task EnableEventReceiveAsync(CancellationToken cancellationToken)
        {
            if (Logging.IsEnabled) Logging.Enter(this, cancellationToken, $"{nameof(EnableEventReceiveAsync)}");

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                // TODO
                await Task.Yield();
                throw new NotImplementedException();
            }
            finally
            {
                if (Logging.IsEnabled) Logging.Exit(this, cancellationToken, $"{nameof(EnableEventReceiveAsync)}");
            }
        }
        #endregion

        #region Accept-Dispose
        public override Task CompleteAsync(string lockToken, CancellationToken cancellationToken)
        {
            if (Logging.IsEnabled) Logging.Enter(this, lockToken, cancellationToken, $"{nameof(CompleteAsync)}");

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                return DisposeMessageAsync(lockToken, AmqpIoTDisposeActions.Accepted, cancellationToken);
            }
            finally
            {
                if (Logging.IsEnabled) Logging.Exit(this, lockToken, cancellationToken, $"{nameof(CompleteAsync)}");
            }
        }

        public override Task AbandonAsync(string lockToken, CancellationToken cancellationToken)
        {
            if (Logging.IsEnabled) Logging.Enter(this, lockToken, cancellationToken, $"{nameof(AbandonAsync)}");

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                return DisposeMessageAsync(lockToken, AmqpIoTDisposeActions.Released, cancellationToken);
            }
            finally
            {
                if (Logging.IsEnabled) Logging.Exit(this, lockToken, cancellationToken, $"{nameof(AbandonAsync)}");
            }
        }

        public override Task RejectAsync(string lockToken, CancellationToken cancellationToken)
        {
            if (Logging.IsEnabled) Logging.Enter(this, lockToken, cancellationToken, $"{nameof(RejectAsync)}");

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                return DisposeMessageAsync(lockToken, AmqpIoTDisposeActions.Rejected, cancellationToken);
            }
            finally
            {
                if (Logging.IsEnabled) Logging.Exit(this, lockToken, cancellationToken, $"{nameof(RejectAsync)}");
            }
        }

        private async Task DisposeMessageAsync(string lockToken, AmqpIoTDisposeActions outcome, CancellationToken cancellationToken)
        {
            if (Logging.IsEnabled) Logging.Enter(this, outcome, $"{nameof(DisposeMessageAsync)}");

            AmqpIoTOutcome disposeOutcome = new AmqpIoTOutcome(null);
            try
            {
                // Currently, the same mechanism is used for sending feedback for C2D messages and events received by modules.
                // However, devices only support C2D messages (they cannot receive events), and modules only support receiving events
                // (they cannot receive C2D messages). So we use this to distinguish whether to dispose the message (i.e. send outcome on)
                // the DeviceBoundReceivingLink or the EventsReceivingLink. 
                // If this changes (i.e. modules are able to receive C2D messages, or devices are able to receive telemetry), this logic 
                // will have to be updated.

                // TODO
                disposeOutcome.ThrowIfError();

                await Task.Yield();
                throw new NotImplementedException();

            }
            catch (Exception exception) when (!exception.IsFatal() && !(exception is OperationCanceledException))
            {
                throw AmqpIoTExceptionAdapter.ConvertToIoTHubException(exception);
            }
            finally
            {
                if (Logging.IsEnabled) Logging.Exit(this, outcome, $"{nameof(DisposeMessageAsync)}");
            }
        }
        #endregion

        #region IDispose
        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;

            base.Dispose(disposing);
            if (disposing)
            {
            }

        }
        #endregion
    }
}
#pragma warning restore IDE0060 // Remove unused parameter - TODO: REMOVE after implementation is ready.
