//---------------------------------------------------------------------
// File: PerfmonCountersStep.cs
// 
// Summary: 
//
// Copyright (c) 2004-2015, Kevin B. Smith. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------

namespace BizUnit.CoreSteps.TestSteps
{
    using System;
    using System.Xml;
    using System.Diagnostics;

    /// <summary>
    /// The PerfmonCountersStep controls perfmon counters, it maybe used to start, and stop 
    /// a set of perfmon counters
    /// </summary>
    /// 
    /// <remarks>
    /// The following shows an example of the Xml representation of this test step.
    /// 
    /// <code escaped="true">
    ///	<TestStep assemblyPath="" typeName="BizUnit.PerfmonCountersStep">
    ///		<PerfmonAction>Start|Stop</PerfmonAction>
    ///     <CounterSetName>BTS-PerfTest01</CounterSetName>
    ///		<CountersListFilePath>C:\Program Files\BizUnit 2.1\Src\SDK\BizUnitFunctionalTests\TestData\Test_08_PerfCounters.txt</CountersListFilePath>
    ///     <SampleInterval>5</SampleInterval>
    ///     <PerfmonLogFilePath>C:\Program Files\BizUnit 2.1\Src\SDK\BizUnitFunctionalTests\TestData\BTS_Run01_%DateTime%.blg</PerfmonLogFilePath>
    ///     <UserName>intranet\kevinsmi</UserName>
    ///     <PassWord>MySecurePassw0rd!</PassWord>
    ///	</TestStep>
    ///	</code>
    ///	
    ///	<list type="table">
    ///		<listheader>
    ///			<term>Tag</term>
    ///			<description>Description</description>
    ///		</listheader>
    ///		<item>
    ///			<term>PerfmonAction</term>
    ///			<description>Start or stop a counter set<para>(Start|Stop)</para></description>
    ///		</item>
    ///		<item>
    ///			<term>CounterSetName</term>
    ///			<description>The name of the perfmon counters to start recording<para>(only required if PerfmonAction=Start)</para></description>
    ///		</item>
    ///		<item>
    ///			<term>CountersListFilePath</term>
    ///			<description>The file containing the set of perfmon counters to record<para>(only required if PerfmonAction=Start)</para></description>
    ///		</item>
    ///		<item>
    ///			<term>PerfmonLogFilePath</term>
    ///			<description>The location to write the perfmon log file</description>
    ///		</item>
    ///		<item>
    ///			<term>SampleInterval</term>
    ///			<description>The interval to record perfmon counters (seconds)</description>
    ///		</item>
    ///		<item>
    ///			<term>PerfmonLogFilePath</term>
    ///			<description>The path of the perfmon log file</description>
    ///		</item>
    ///		<item>
    ///			<term>UserName</term>
    ///			<description>The user name to run logman under (optional)</description>
    ///		</item>
    ///		<item>
    ///			<term>PassWord</term>
    ///			<description>The password for the user name which logman is running under (optional)</description>
    ///		</item>
    ///	</list>
    ///	</remarks>
    [Obsolete("PerfmonCountersStep has been deprecated. Investigate the BizUnit.TestSteps namespace.")]
    class PerfmonCountersStep : ITestStep
    {
        /// <summary>
        /// ITestStep.Execute() implementation
        /// </summary>
        /// <param name='testConfig'>The Xml fragment containing the configuration for this test step</param>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public void Execute(XmlNode testConfig, Context context)
        {
            string perfmonAction = context.ReadConfigAsString(testConfig, "PerfmonAction");
            string counterSetName = context.ReadConfigAsString(testConfig, "CounterSetName");
            string workingDirectory = Environment.CurrentDirectory;

            switch(perfmonAction)
            {
                case "Start":
                    string countersListFilePath = context.ReadConfigAsString(testConfig, "CountersListFilePath");
                    string perfmonLogFilePath = context.ReadConfigAsString(testConfig, "PerfmonLogFilePath");
                    string sampleInterval = context.ReadConfigAsString(testConfig, "SampleInterval");
                    string userName = context.ReadConfigAsString(testConfig, "UserName", true);
                    string passWord = context.ReadConfigAsString(testConfig, "PassWord", true);

                    string processParams;
                    if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(passWord))
                    {
                        processParams =
                            string.Format("create counter {0} -cf \"{1}\" -si {2} -o \"{3}\" --v -u {4} {5}",
                                          counterSetName, countersListFilePath, sampleInterval, perfmonLogFilePath,
                                          userName, passWord);
                    }
                    else
                    {
                        processParams = "create counter " + counterSetName + " -cf \"" + countersListFilePath +
                                        "\" -si " + sampleInterval + " -o \"" + perfmonLogFilePath + "\" --v";
                    }

                    context.LogInfo("PerfmonCountersStep is about to start the perfmon counter set : \"{0}\" writting log file:\"{1}\"", counterSetName, perfmonLogFilePath); 
                    
                    try
                    {
                        ExecuteLogman(workingDirectory, "delete " + counterSetName, context);
                    }
                    catch (Exception)
                    {
                        // Eat the exception, we don't care if the perfcounter set didn't already exist!
                    }

                    ExecuteLogman(workingDirectory, processParams, context); // create the counter set
                    ExecuteLogman(workingDirectory, "start " + counterSetName, context); // start the counter set

                    break;

                case "Stop":
                    context.LogInfo("PerfmonCountersStep is about to stop the perfmon counter set : \"{0}\"", counterSetName);
                    ExecuteLogman(workingDirectory, "stop " + counterSetName, context);
                    break;

                default:
                    throw new ApplicationException(string.Format("PerfmonCountersStep does not recognise PerfmonAction= {0}", perfmonAction));
            }
        }

        private static void ExecuteLogman(string workingDirectory, string processParams, Context context)
        {
            var process = new Process
                              {
                                  StartInfo =
                                      {
                                          UseShellExecute = false,
                                          RedirectStandardOutput = true,
                                          CreateNoWindow = true,
                                          Arguments = processParams,
                                          FileName = "logman",
                                          WorkingDirectory = workingDirectory
                                      }
                              };

            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            int exitCode = process.ExitCode;

            context.LogInfo("logman output: {0}", output); 

            if (0 != exitCode)
            {
                throw new ApplicationException(string.Format("PerfmonCountersStep received an exit code: {0} while executing process {1}\n\nOutput: {2}", exitCode, processParams, output));
            }
        }
    }
}
