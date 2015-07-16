//---------------------------------------------------------------------
// File: EventLogClearStep.cs
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

namespace BizUnit.CoreSteps.TestSteps
{
	using System;
	using System.Diagnostics;
	using System.Xml;
    using System.Collections.Generic;

	/// <summary>
	/// The EventLogClearStep test step clears the event log(s) on specified machines. Note: caution should be taken when clearing event log.
	/// </summary>
	/// 
	/// <remarks>
	/// The following shows an example of the Xml representation of this test step.
	/// 
	/// <code escaped="true">
	/// <TestStep assemblyPath="" typeName="BizUnit.EventLogClearStep">
    ///		<Machine>UKBTSTEST01,UKBTSTEST02,UKBTSTEST03</Machine>
	///		<EventLog>Application</EventLog>
	/// </TestStep>
	///	</code>
	///	
	///	<list type="table">
	///		<listheader>
	///			<term>Tag</term>
	///			<description>Description</description>
	///		</listheader>
	///		<item>
	///			<term>Machine</term>
	///			<description>The name(s) of the machine(s) where the event log should be cleared, a comma delimeted list may be specified <para>(optional, defaults to local machine)</para></description>
	///		</item>
	///		<item>
	///			<term>EventLog</term>
	///			<description>The event log to clear, e.g. Application, System etc.</description>
	///		</item>
	///	</list>
	///	</remarks>	
    [Obsolete("EventLogClearStep has been deprecated. Investigate the BizUnit.TestSteps namespace.")]
	public class EventLogClearStep : ITestStep
	{
		/// <summary>
		/// ITestStep.Execute() implementation
		/// </summary>
		/// <param name='testConfig'>The Xml fragment containing the configuration for this test step</param>
		/// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
		public void Execute(XmlNode testConfig, Context context)
		{
			var rawListOfMachines = context.ReadConfigAsString(testConfig, "Machine", true);
            
            var listOfMachines = new List<string>();

            if (string.IsNullOrEmpty(rawListOfMachines))
            {
                listOfMachines.Add(Environment.MachineName);
            }
            else
            {
                listOfMachines.AddRange(rawListOfMachines.Split(','));
            }

            foreach (var machine in listOfMachines)
            {
                var eventLog = context.ReadConfigAsString(testConfig, "EventLog");

                using (var log = new EventLog(eventLog, machine))
                {
                    context.LogInfo("About to clear the '{0}' event log on machine '{1}'of all entries.", eventLog,
                                    machine);
                    log.Clear();
                }
            }
		}
	}
}
