﻿
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace Microsoft.Azure.Devices.Client
{
    using System;
    using System.IO;
    using System.Threading;
#if NETMF
    using System.Collections;
#else
    using Microsoft.Azure.Devices.Client.Common.Api;
    using System.Collections.Generic;
#endif
    using Microsoft.Azure.Devices.Common;
    using DateTimeT = System.DateTime;

    /// <summary>
    /// The data structure represent the method request coming from the IotHub.
    /// </summary>
    public sealed class MethodRequestInternal : IDisposable
    {
#if NETMF
        Stream bodyStream;
#else
        volatile Stream bodyStream;
#endif
        bool disposed;
        bool ownsBodyStream;
        int getBodyCalled;

        /// <summary>
        /// Default constructor with no body data
        /// </summary>
        internal MethodRequestInternal(CancellationToken cancellationToken)
        {
#if !NETMF
            this.InitializeWithStream(Stream.Null, true);
#endif
            this.CancellationToken = cancellationToken;
        }

#if !NETMF
        /// <summary>
        /// This constructor is only used in the receive path from Amqp path, 
        /// or in Cloning from a Message that has serialized.
        /// </summary>

        internal MethodRequestInternal(string name, string requestId, Stream bodyStream, CancellationToken cancellationToken)
            : this(cancellationToken)
        {
            Name = name;
            RequestId = requestId;
            Stream stream = bodyStream;
            this.InitializeWithStream(stream, false);
        }
#endif

        internal CancellationToken CancellationToken
        {
            get;
            private set;
        }

        /// <summary>
        /// Property indicating the method name for this instance
        /// </summary>
        internal string Name
        {
            get; private set;
        }

        /// <summary>
        /// the request ID for the transport layer
        /// </summary>
        internal string RequestId
        {
            get; private set;
        }

        internal Stream BodyStream
        {
            get
            {
                return this.bodyStream;
            }
        }

        /// <summary>
        /// Dispose the current method data instance
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Return the body stream of the current method data instance
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">throws if the method has been called.</exception>
        /// <exception cref="ObjectDisposedException">throws if the method data has already been disposed.</exception>
        /// <remarks>This method can only be called once and afterwards method will throw <see cref="InvalidOperationException"/>.</remarks>
        internal Stream GetBodyStream()
        {
            this.ThrowIfDisposed();
            this.SetGetBodyCalled();
            if (this.bodyStream != null)
            {
                return this.bodyStream;
            }

#if NETMF
            return null;
#else
            return Stream.Null;
#endif
        }

        /// <summary>
        /// This methods return the body stream as a byte array
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">throws if the method has been called.</exception>
        /// <exception cref="ObjectDisposedException">throws if the method data has already been disposed.</exception>
        internal byte[] GetBytes()
        {
            this.ThrowIfDisposed();
            this.SetGetBodyCalled();
            if (this.bodyStream == null)
            {
                return new byte[] { };
            }

            // This is just fail safe code in case we are not using the Amqp protocol.
            return ReadFullStream(this.bodyStream);
        }

        // Test hook only
        internal void ResetGetBodyCalled()
        {
            Interlocked.Exchange(ref this.getBodyCalled, 0);
            if (this.bodyStream != null && this.bodyStream.CanSeek)
            {
                this.bodyStream.Seek(0, SeekOrigin.Begin);
            }
        }

        internal bool TryResetBody(long position)
        {
            if (this.bodyStream != null && this.bodyStream.CanSeek)
            {
                this.bodyStream.Seek(position, SeekOrigin.Begin);
                Interlocked.Exchange(ref this.getBodyCalled, 0);
                return true;
            }
            return false;
        }

#if NETMF
        internal bool IsBodyCalled
        {
            // A safe comparison for one that will never actually perform an exchange (maybe not necessary?)
            get { return Interlocked.CompareExchange(ref this.getBodyCalled, 9999, 9999) == 1; }
        }
#else
        internal bool IsBodyCalled => Volatile.Read(ref this.getBodyCalled) == 1;
#endif

        void SetGetBodyCalled()
        {
            if (1 == Interlocked.Exchange(ref this.getBodyCalled, 1))
            {
#if NETMF
                throw new InvalidOperationException("The message body cannot be read multiple times. To reuse it store the value after reading.");
#else
                throw Fx.Exception.AsError(new InvalidOperationException(Common.Api.ApiResources.MessageBodyConsumed));
#endif
            }
        }

        void InitializeWithStream(Stream stream, bool ownsStream)
        {
            // This method should only be used in constructor because
            // this has no locking on the bodyStream.
            this.bodyStream = stream;
            this.ownsBodyStream = ownsStream;
        }

        static byte[] ReadFullStream(Stream inputStream)
        {
#if NETMF
            inputStream.Position = 0;
            byte[] buffer = new byte[inputStream.Length];

            inputStream.Read(buffer, 0, (int)inputStream.Length);

            return buffer;
#else
            using (var ms = new MemoryStream())
            {
                inputStream.CopyTo(ms);
                return ms.ToArray();
            }
#endif
        }

        void ThrowIfDisposed()
        {
            if (this.disposed)
            {
#if NETMF
                throw new Exception("Message disposed");
#else
                throw Fx.Exception.ObjectDisposed(Common.Api.ApiResources.MessageDisposed);
#endif
            }
        }

        void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.bodyStream != null && this.ownsBodyStream)
                    {
                        this.bodyStream.Dispose();
                        this.bodyStream = null;
                    }
                }

                this.disposed = true;
            }
        }
    }
}