﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Azure.Devices.Client.Exceptions;
using Microsoft.Azure.Devices.Client.Extensions;
using Microsoft.Azure.Devices.Client.Transport.AmqpIoT;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Azure.Devices.Client.Transport.Amqp
{
    internal class AmqpIoTConnectionX509 : AmqpIoTConnection
    {
        private X509Certificate2 _certificate;

    }
}