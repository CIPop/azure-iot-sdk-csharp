using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.Devices.Client.Transport
{
    internal enum AuthenticationModel
    {
        SharedAccessKeyIndividualIdentity,
        SharedAccessKeyHubPolicy,
        X509Certificate
    }
}
