// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text;

namespace Microsoft.Azure.Devices.E2ETests
{
    public class TelemetryMetrics
    {
        private static string s_configString; // Contains all Config* parameters.
        public double? WallTime;
        public int? Id;
        public string OperationType; // e.g. OpenAsync / SendAsync, etc
        public double? ScheduleTime;
        public double? ExecuteTime;
        public string ErrorMessage;

        public static string GetHeader()
        {
            return
                "WallTimeMs," +
                "Id, " +
                "Operation," +
                "ScheduleTimeMs, " +
                "ExecuteTimeMs, " +

                "ConfigScenario," +
                "ConfigTimeSeconds," +
                "ConfigTransportType," +
                "ConfigMessageSizeBytes," +
                "ConfigParallelOperations," +
                "ConfigScenarioInstances," +
                "ConfigAuthType," +
                
                "ErrorMessage, ";
        }

        public static void SetStaticConfigParameters(
            int timeSeconds,
            Client.TransportType transportType,
            int messageSizeBytes,
            int maximumParallelOperations,
            int scenarioInstances,
            string authType,
            string scenario)
        {
            s_configString = $"{scenario},{timeSeconds},{transportType.ToString()},{messageSizeBytes},{maximumParallelOperations},{scenarioInstances},{authType},";
        }

        public override string ToString()
        {
            var sb = new StringBuilder(); 
            Add(sb, WallTime);
            Add(sb, Id);
            Add(sb, OperationType);
            Add(sb, ScheduleTime);
            Add(sb, ExecuteTime);
            Add(sb, s_configString);
            Add(sb, ErrorMessage);

            return sb.ToString();
        }

        private void Add(StringBuilder sb, object data)
        {
            if (data != null)
            {
                sb.Append(data.ToString());
            }

            sb.Append(',');
        }
    }
}
