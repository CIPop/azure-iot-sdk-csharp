// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Azure.Devices.Provisioning.Client.Transport;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Microsoft.Azure.Devices.Provisioning.Transport.Mqtt.UnitTests
{
    [TestClass]
    [TestCategory("Unit")]
    public class ProvisioningErrorDetailsMqttTests
    {
        private static double? _throttledDelay = 32;
        private static string _validTopicNameThrottled = $"$dps/registrations/res/429/?$rid=9&Retry-After={_throttledDelay}";

        private static double? acceptedDelay = 23;
        private static string _validTopicNameAccepted = $"$dps/registrations/res/202/?$rid=9&Retry-After={acceptedDelay}";

        private TimeSpan _defaultInterval = TimeSpan.FromSeconds(2);

        [TestMethod]
        public void RetryAfterValidThrottled()
        {
            TimeSpan? actual = ProvisioningErrorDetailsMqtt.GetRetryAfterFromTopic(_validTopicNameThrottled, _defaultInterval);
            Assert.IsNotNull(actual);
            Assert.AreEqual(_throttledDelay, actual?.Seconds);
        }

        [TestMethod]
        public void RetryAfterValidAccepted()
        {
            TimeSpan? actual = ProvisioningErrorDetailsMqtt.GetRetryAfterFromTopic(_validTopicNameAccepted, _defaultInterval);
            Assert.IsNotNull(actual);
            Assert.AreEqual(acceptedDelay, actual?.Seconds);
        }

        [TestMethod]
        public void RetryAfterWithNoRetryAfterValue()
        {
            string invalidTopic = "$dps/registrations/res/429/?$rid=9&Retry-After=";
            TimeSpan? actual = ProvisioningErrorDetailsMqtt.GetRetryAfterFromTopic(invalidTopic, _defaultInterval);
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void RetryAfterWithNoRetryAfterQueryKeyOrValue()
        {
            string invalidTopic = "$dps/registrations/res/429/?$rid=9";
            TimeSpan? actual = ProvisioningErrorDetailsMqtt.GetRetryAfterFromTopic(invalidTopic, _defaultInterval);
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void RetryAfterWithNoQueryString()
        {
            string invalidTopic = "$dps/registrations/res/429/";
            TimeSpan? actual = ProvisioningErrorDetailsMqtt.GetRetryAfterFromTopic(invalidTopic, _defaultInterval);
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void RetryAfterWithNoTopicString()
        {
            string invalidTopic = "";
            TimeSpan? actual = ProvisioningErrorDetailsMqtt.GetRetryAfterFromTopic(invalidTopic, _defaultInterval);
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void RetryAfterWithTooSmallOfDelayChoosesDefault()
        {
            string invalidTopic = "$dps/registrations/res/429/?$rid=9&Retry-After=0";
            TimeSpan? actual = ProvisioningErrorDetailsMqtt.GetRetryAfterFromTopic(invalidTopic, _defaultInterval);
            Assert.IsNotNull(actual);
            Assert.AreEqual(_defaultInterval.Seconds, actual?.Seconds);
        }

        [TestMethod]
        public void RetryAfterWithNegativeDelayChoosesDefault()
        {
            string invalidTopic = "$dps/registrations/res/429/?$rid=9&Retry-After=-1";
            TimeSpan? actual = ProvisioningErrorDetailsMqtt.GetRetryAfterFromTopic(invalidTopic, _defaultInterval);
            Assert.IsNotNull(actual);
            Assert.AreEqual(_defaultInterval.Seconds, actual?.Seconds);
        }
    }
}
