//---------------------------------------------------------------------
// File: EventLogSaveStep.cs
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
    using System.Xml;
    using System.Management;
    using System.Collections.Generic;

    /// <summary>
    /// The EventLogSaveStep test step clears the event log. Note: caution should be taken when clearing event log.
    /// </summary>
    /// 
    /// <remarks>
    /// The following shows an example of the Xml representation of this test step.
    /// 
    /// <code escaped="true">
    /// <TestStep assemblyPath="" typeName="BizUnit.EventLogSaveStep">
    ///		<Server>UKBTSTEST01,UKBTSTEST02,UKBTSTEST03,UKBTSTEST04</Server>
    ///		<DestinationPath>C:\MyLogs\</DestinationPath>
    /// </TestStep>
    ///	</code>
    ///	
    ///	<list type="table">
    ///		<listheader>
    ///			<term>Tag</term>
    ///			<description>Description</description>
    ///		</listheader>
    ///		<item>
    ///			<term>Server</term>
    ///			<description>The name of the machine(s) where the event log should be saved from, multiple servers are specified through the use of a comma delimiter</description>
    ///		</item>
    ///		<item>
    ///			<term>DestinationPath</term>
    ///			<description>The local path to save the event log to, minus the file name or trailing backslash, the resulting filename will be SERVERNAME.evt</description>
    ///		</item>
    ///	</list>
    ///	</remarks>	
    [Obsolete("EventLogSaveStep has been deprecated. Investigate the BizUnit.TestSteps namespace.")]
    public class EventLogSaveStep : ITestStep
    {
        /// <summary>
        /// ITestStep.Execute() implementation
        /// </summary>
        /// <param name='testConfig'>The Xml fragment containing the configuration for this test step</param>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public void Execute(XmlNode testConfig, Context context)
        {
            var destinationPath = context.ReadConfigAsString(testConfig, "DestinationPath");
            var rawListOfServers = context.ReadConfigAsString(testConfig, "Server");
            
            var listOfServers = new List<string>();
            listOfServers.AddRange(rawListOfServers.Split(','));

            foreach (var server in listOfServers)
            {
                context.LogInfo("About to save the event on server: {0} to the following directory: {1}", server,
                                destinationPath);

                ManagementScope scope;

                if ((server.ToUpper() != Environment.MachineName.ToUpper()))
                {
                    var options = new ConnectionOptions
                                      {
                                          Impersonation = ImpersonationLevel.Impersonate,
                                          EnablePrivileges = true
                                      };
                    scope = new ManagementScope(string.Format(@"\\{0}\root\cimv2", server), options);
                }
                else
                {
                    var options = new ConnectionOptions
                                      {
                                          Impersonation = ImpersonationLevel.Impersonate,
                                          EnablePrivileges = true
                                      };
                    scope = new ManagementScope(@"root\cimv2", options);
                }

                var query = new SelectQuery("Select * from Win32_NTEventLogFile");
                var searcher = new ManagementObjectSearcher(scope, query);

                foreach (var logFileObject in searcher.Get())
                {
                    var methodArgs = new object[] { destinationPath + @"\" + server + ".evt" };

                    try
                    {
                        ((ManagementObject)logFileObject).InvokeMethod("BackupEventLog", methodArgs);
                    }
                    catch (Exception e1)
                    {
                        //access denied on method call if scope path referes to the same  
                        context.LogException(e1);

                        throw;
                    }
                }
            }
        }
    }
}

