// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net.NetworkInformation;

namespace Microsoft.Azure.Devices.E2ETests
{
    public class Program
    {
        private static Dictionary<string, Tuple<string, Func<PerfScenarioConfig, PerfScenario>>> s_scenarios
            = new Dictionary<string, Tuple<string, Func<PerfScenarioConfig, PerfScenario>>>()
        {
            {"generate_iothub_config",
                new Tuple<string, Func<PerfScenarioConfig, PerfScenario>>(
                    "Generate the IoT Hub configuration required for the test (creates multiple devices).",
                    (c) => {return new GenerateIotHubConfigTest(c);})},

            {"device_d2c",
                new Tuple<string, Func<PerfScenarioConfig, PerfScenario>>(
                    "Devices sending events to IoT Hub.",
                    (c) => {return new DeviceD2C(c);})},

            {"device_c2d",
                new Tuple<string, Func<PerfScenarioConfig, PerfScenario>>(
                    "Devices receiving events from the IoT Hub.", 
                    null) },

            {"device_method",
                new Tuple<string, Func<PerfScenarioConfig, PerfScenario>>(
                    "Devices receiving methods from IoT Hub.", 
                    null) },

            {"service_c2d",
                new Tuple<string, Func<PerfScenarioConfig, PerfScenario>>(
                    "Services sending events to devices through IoT Hub.", 
                    null) },

            {"service_method",
                new Tuple<string, Func<PerfScenarioConfig, PerfScenario>>("Services calling methods on devices through IoT Hub.", null) },
            
            {"single_device_d2c",
                new Tuple<string, Func<PerfScenarioConfig, PerfScenario>>("A single device sending many events to IoT Hub.", null) },




            { "device_combined_mux",
                new Tuple<string, Func<PerfScenarioConfig, PerfScenario>>(
                    "Devices using multiple features to IoT Hub on AMQP multiplexed connections.",
                    (c) => {return new DeviceD2C(c, pooled:true, poolSize: 400);})},
        };

        private static void Help()
        {
            Console.WriteLine(
                "Usage: \n\n" +
                "   iotclientperf [-topslna] -f <scenario>\n" +
                "       -t <seconds>    : Execution time (default 10 seconds).\n" +
                "       -o <path>       : Output path (default outputs to console).\n" +
                "       -p <protocol>   : Protocol (default mqtt). \n" +
                "                         Possible values: mqtt | mqtt_ws | amqp | amqp_ws | http.\n" +
                "       -s <bytes>      : Payload size (default 128 bytes). This depends on the scenario.\n" +
                "       -l <parallel_op>: Maximum parallel operations. (default 100 operations/scenarios in parallel).\n" +
                "       -n <count>      : Number of scenario instances. (default 1 instance).\n" + 
                "       -a <authType>   : Authentication type (default sas).\n" +
                "                         Possible values: sas | sas_policy | x509 \n" +
                "       -c <connections>: Enables AMQP Pooling. The connection pool size. (default -1: disabled)\n" +
                "       -f <scenario>   : Scenario name. One of the following: \n"
            );

            foreach (string scenario in s_scenarios.Keys)
            {
                Console.WriteLine($"\t{scenario}: {s_scenarios[scenario].Item1}");
            }
        }

        private static Dictionary<string, Client.TransportType> s_transportDictionary = new Dictionary<string, Client.TransportType>()
        {
            {"mqtt", Client.TransportType.Mqtt_Tcp_Only },
            {"mqtt_ws", Client.TransportType.Mqtt_WebSocket_Only },
            {"amqp", Client.TransportType.Amqp_Tcp_Only },
            {"amqp_ws", Client.TransportType.Amqp_WebSocket_Only},
            {"http", Client.TransportType.Http1 },
        };

        public static int Main(string[] args)
        {
            Console.WriteLine("IoT Client Performance test");

            if (args.Length < 1)
            {
                Help();
                return -1;
            }

            int param_counter = 0;
            int t = 10;
            string o = null;
            string p = "mqtt";
            int s = 128;
            int l = 100;
            int n = 1;
            string a = "sas";
            int c = -1;
            string f = null;

            while (param_counter + 1 < args.Length)
            {
                switch (args[param_counter])
                {
                    case "--":
                        break;

                    case "-t":
                        t = int.Parse(args[++param_counter], CultureInfo.InvariantCulture);
                        break;

                    case "-o":
                        o = args[++param_counter];
                        break;

                    case "-p":
                        p = args[++param_counter];
                        break;

                    case "-s":
                        s = int.Parse(args[++param_counter], CultureInfo.InvariantCulture);
                        break;

                    case "-l":
                        l = int.Parse(args[++param_counter], CultureInfo.InvariantCulture);
                        break;

                    case "-n":
                        n = int.Parse(args[++param_counter], CultureInfo.InvariantCulture);
                        break;

                    case "-a":
                        a = args[++param_counter];
                        break;

                    case "-c":
                        c = int.Parse(args[++param_counter], CultureInfo.InvariantCulture);
                        break;

                    case "-f":
                        f = args[++param_counter];
                        break;

                    default:
                        Console.WriteLine($"Unknown parameter: {args[param_counter]}.");
                        return -1;
                }

                param_counter++;
            }

            if (f == null)
            {
                Console.Error.WriteLine("Missing -f <scenario> parameter.");
                Help();
                return -1;
            }

            Tuple<string, Func<PerfScenarioConfig, PerfScenario>> scenario;
            Func<PerfScenarioConfig, PerfScenario> scenarioFactory;
            if (!s_scenarios.TryGetValue(f, out scenario))
            {
                Console.Error.WriteLine($"Unknown scenario: {f}");
                return -1;
            }

            scenarioFactory = scenario.Item2;

            Client.TransportType transportType;
            if (!s_transportDictionary.TryGetValue(p, out transportType))
            {
                Console.Error.WriteLine($"Unknown transport type: {p}");
                return -1;
            }

            ResultWriter resultWriter;
            if (o == null)
            {
                resultWriter = new ResultWriterConsole();
            }
            else
            {
                resultWriter = new ResultWriterFile(o);
            }
            
            var runner = new PerfTestRunner(
                resultWriter,
                t,
                transportType,
                s,
                l,
                n,
                a,
                c,
                f,
                scenarioFactory);

            runner.RunTestAsync().GetAwaiter().GetResult();

            return 0;
        }
    }
}
