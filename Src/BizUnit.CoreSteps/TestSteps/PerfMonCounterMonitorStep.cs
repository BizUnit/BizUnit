//---------------------------------------------------------------------
// File: DelayStep.cs
// 
// Summary: 
//
//---------------------------------------------------------------------
// Copyright (c) 2004-2015, Kevin B. Smith. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------

using System.Diagnostics;

namespace BizUnit.CoreSteps.TestSteps
{
    using System;
    using System.Xml;

    /// <summary>
    /// The PerfMonCounterMonitorStep is used to monitor a perfmon counter 
    /// until it reaches a target value. If the counter reaches the target 
    /// value within the timeout period the step will succeed, otherwise it 
    /// will fail.
	/// </summary>
	/// 
	/// <remarks>
	/// The following shows an example of the Xml representation of this test step.
	/// 
	/// <code escaped="true">
    ///	<TestStep assemblyPath="" typeName="BizUnit.PerfMonCounterMonitorStep">
    ///		<CategoryName>BizTalk:Message Box:Host Counters</CategoryName>
    ///     <CounterName>Host Queue - Length</CounterName>
    ///     <InstanceName>Host:BizTalkMsgBoxDB:ServerName</InstanceName>
    ///     <Server>BLIZARD001</Server>
    ///     <CounterTargetValue>50.0</CounterTargetValue>
    ///     <SleepTime>100</SleepTime>
    ///     <TimeOut>100</TimeOut>
    ///	</TestStep>
	///	</code>
	///	
	///	<list type="table">
	///		<listheader>
	///			<term>Tag</term>
	///			<description>Description</description>
	///		</listheader>
	///		<item>
    ///			<term>CategoryName</term>
	///			<description>The name of the perfmon category</description>
	///		</item>
	///		<item>
    ///			<term>CounterName</term>
	///			<description>The name of the perfmon counter to monitor</description>
	///		</item>
    ///		<item>
    ///			<term>InstanceName</term>
    ///			<description>The perfmon counter instance name<para>(optional)</para></description>
    ///		</item>
    ///		<item>
    ///			<term>Server</term>
    ///			<description>The name of the server to monitor the counter on</description>
    ///		</item>
    ///		<item>
    ///			<term>CounterTargetValue</term>
    ///			<description>The target perfmon counter value, once the counter reaches this value the step will complete</description>
    ///		</item>
    ///		<item>
    ///			<term>SleepTime</term>
    ///			<description>The length of the delay in miliseconds between checks of the counter<para>(optional)</para></description>
    ///		</item>
    ///		<item>
    ///			<term>TimeOut</term>
    ///			<description>The maximum length of time the step will spend checking the counter (seconds)<para>(optional)</para></description>
    ///		</item>
    /// </list>
	///	</remarks>
    [Obsolete("PerfMonCounterMonitorStep has been deprecated. Investigate the BizUnit.TestSteps namespace.")]
    public class PerfMonCounterMonitorStep : ITestStep
    {
        /// <summary>
        /// ITestStep.Execute() implementation
        /// </summary>
        /// <param name='testConfig'>The Xml fragment containing the configuration for this test step</param>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public void Execute(XmlNode testConfig, Context context)
        {
            // Read config...
            string categoryName = context.ReadConfigAsString(testConfig, "CategoryName");
            string counterName = context.ReadConfigAsString(testConfig, "CounterName");
            string instanceName = context.ReadConfigAsString(testConfig, "InstanceName", true);
            string server = context.ReadConfigAsString(testConfig, "Server");
            float counterTargetValue = context.ReadConfigAsFloat(testConfig, "CounterTargetValue");
            int sleepTime = context.ReadConfigAsInt32(testConfig, "SleepTime", true);
            int timeOut = context.ReadConfigAsInt32(testConfig, "TimeOut", true);

            context.LogInfo("About to start monitoring: {0}\\{1}\\{2}({3}) for the target value: {4}", server, categoryName, counterName, instanceName, counterTargetValue);

            // Init perfmon counter...
            var perfCounter = new PerformanceCounter
                                  {
                                      CategoryName = categoryName,
                                      CounterName = counterName,
                                      MachineName = server
                                  };

            if (null != instanceName)
            {
                perfCounter.InstanceName = instanceName;
            }

            // Set default value for sleepTime
            if ( 0 == sleepTime)
            {
                sleepTime = 100;
            }

            DateTime now = DateTime.Now;
            DateTime end = now;

            if (0 != timeOut)
            {
                end = now.AddSeconds(timeOut);
            }

            bool targetHit = false;

            do
            {
                if (perfCounter.NextValue() == counterTargetValue)
                {
                    targetHit = true;
                    context.LogInfo("Target hit");

                }
                else if ((end > now) || (0 == timeOut))
                {
                    System.Threading.Thread.Sleep(sleepTime);
                }
            } while ( (!targetHit) && ((end > now) || (0 == timeOut)));

            if (!targetHit)
            {
                throw new ApplicationException("The target perfmon counter was not hit!");
            }
        }
    }
}
