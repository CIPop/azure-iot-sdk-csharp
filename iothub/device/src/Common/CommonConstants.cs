// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Azure.Devices.Client
{
    internal static class CommonConstants
    {
        public const string AmqpsScheme = "amqps";
        public const string MediaTypeForDeviceManagementApis = "application/json";
        
        // Device URI Templates
        public const string DeviceEventPathTemplate = "/devices/{0}/messages/events";
        public const string ModuleEventPathTemplate = "/devices/{0}/modules/{1}/messages/events";
        public const string DeviceBoundPathTemplate = "/devices/{0}/messages/deviceBound";
        public const string ModuleBoundPathTemplate = "/devices/{0}/modules/{1}/messages/deviceBound";
        public const string DeviceMethodPathTemplate = "/devices/{0}/methods/deviceBound";
        public const string ModuleMethodPathTemplate = "/devices/{0}/modules/{1}/methods/deviceBound";
        public const string DeviceTwinPathTemplate = "/devices/{0}/twin";
        public const string ModuleTwinPathTemplate = "/devices/{0}/modules/{1}/twin";
        public const string BlobUploadStatusPathTemplate = "/devices/{0}/files/";
        public const string BlobUploadPathTemplate = "/devices/{0}/files";
        public const string DeviceBoundPathCompleteTemplate = DeviceBoundPathTemplate + "/{1}";
        public const string DeviceBoundPathAbandonTemplate = DeviceBoundPathCompleteTemplate + "/abandon";
        public const string DeviceBoundPathRejectTemplate = DeviceBoundPathCompleteTemplate + "?reject";

        public const string BatchedMessageContentType = "application/vnd.microsoft.iothub.json";
        public const string DeviceToCloudOperation = "d2c";
        public const string CloudToDeviceOperation = "c2d";
    }
}
