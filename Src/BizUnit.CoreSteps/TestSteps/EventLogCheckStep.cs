//---------------------------------------------------------------------
// File: EventLogCheckStep.cs
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

using BizUnit.BizUnitOM;

namespace BizUnit.CoreSteps.TestSteps
    {
	    using System;
	    using System.Diagnostics;
	    using System.Xml;
	    using System.Text.RegularExpressions;
        using System.Collections.Generic;

	    /// <summary>
	    /// The EventLogCheckStep test step looks for an event log entry. Note: this test step only looks for the event log entry during the time period of the test case.
	    /// </summary>
	    /// <remarks>
	    /// The following shows an example of the Xml representation of this test step.
	    /// 
	    /// <code escaped="true">
	    /// <TestStep assemblyPath="" typeName="BizUnit.EventLogCheckStep">
	    ///		<DelayBeforeCheck>0</DelayBeforeCheck> <!-- Optional, seconds to delay performing check -->
	    ///		<Machine>UKBTSTEST01</Machine>
	    ///		<EventLog>Application</EventLog>
	    ///		<Source>BizTalk Governor</Source>
	    ///		<Type>Error</Type>
	    ///		<EventId>10002</EventId>
	    ///		<ValidationRegex>The BizTalk Governor disabled the receive location: GovernorIn</ValidationRegex>
        ///		<FailIfFound>False</FailIfFound>
        /// </TestStep>
	    ///	</code>
	    ///	
	    ///	<list type="table">
	    ///		<listheader>
	    ///			<term>Tag</term>
	    ///			<description>Description</description>
	    ///		</listheader>
	    ///		<item>
	    ///			<term>DelayBeforeCheck</term>
	    ///			<description>The time to wait before checking the event log (seconds)</description>
	    ///		</item>
	    ///		<item>
	    ///			<term>Machine</term>
	    ///			<description>The name of the machine where the event log should be checked, (optional, defaults to local machine)</description>
	    ///		</item>
	    ///		<item>
	    ///			<term>EventLog</term>
	    ///			<description>The event log to check, e.g. Application, System etc.</description>
	    ///		</item>
	    ///		<item>
	    ///			<term>Source</term>
	    ///			<description>The event log source, e.g. BizTalk Server 2004</description>
	    ///		</item>
	    ///		<item>
	    ///			<term>Type</term>
	    ///			<description>The type of the event log entry, e.g. Error, Warning, Info.</description>
	    ///		</item>
	    ///		<item>
	    ///			<term>EventId</term>
	    ///			<description>The ID of the event to look for <para>(optional)</para></description>
	    ///		</item>
	    ///		<item>
	    ///			<term>ValidationRegex</term>
	    ///			<description>Regular expression used to check the event message.</description>
	    ///		</item>
	    ///		<item>
	    ///			<term>FailIfFound</term>
	    ///			<description>Flag to indicate whether the test step should fail if the event log entry is not present.</description>
	    ///		</item>
	    ///	</list>
	    ///	</remarks>	
        [Obsolete("EventLogCheckStep has been deprecated. Investigate the BizUnit.TestSteps namespace.")]
	    public class EventLogCheckStep : ITestStepOM
	    {
	        private int _delayBeforeCheck;
	        private string _machine;
	        private string _eventLog;
	        private string _source;
	        private string _type;
	        private int _eventId;
	        private IList<string> _validationRegexs = new List<string>();
	        private bool _failIfFound;
	        private EventLogEntryType _entryType;

	        public int DelayBeforeCheck
	        {
	            set
	            {
	                _delayBeforeCheck = value;
	            }
	        }

	        public string Machine
	        {
	            set
	            {
	                _machine = value;
	            }
	        }

	        public string EventLog
	        {
	            set
	            {
	                _eventLog = value;
	            }
	        }

	        public string Source
	        {
	            set
	            {
	                _source = value;
	            }
	        }

	        public string EventType
	        {
	            set
	            {
	                _type = value;
	            }
	        }

	        public int EventId
	        {
	            set
	            {
	                _eventId = value;
	            }
	        }

	        public IList<string> ValidationRegexs
	        {
	            set
	            {
	                _validationRegexs = value;
	            }
	        }

	        public bool FailIfFound
	        {
	            set
	            {
	                _failIfFound = value;
	            }
	        }

            /// <summary>
		    /// ITestStep.Execute() implementation
		    /// </summary>
		    /// <param name='testConfig'>The Xml fragment containing the configuration for this test step</param>
		    /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
		    public void Execute(XmlNode testConfig, Context context)
		    {
			    _delayBeforeCheck = context.ReadConfigAsInt32(testConfig, "DelayBeforeCheck", true);
			    _machine = context.ReadConfigAsString(testConfig, "Machine", true);
			    _eventId = context.ReadConfigAsInt32(testConfig, "EventId", true);
			    _eventLog = context.ReadConfigAsString(testConfig, "EventLog");
			    _source = context.ReadConfigAsString(testConfig, "Source");
			    _type = context.ReadConfigAsString(testConfig, "Type");
                _failIfFound = context.ReadConfigAsBool(testConfig, "FailIfFound", true);
                _entryType = (EventLogEntryType)Enum.Parse(typeof(EventLogEntryType), _type, true);
			    XmlNodeList validationNodes = testConfig.SelectNodes("ValidationRegex");

                if (string.IsNullOrEmpty(_machine))
                {
                    _machine = Environment.MachineName;
                }

                if (null != validationNodes)
                {
                    foreach (XmlNode validationNode in validationNodes)
                    {
                        _validationRegexs.Add(validationNode.InnerText);
                    }
                }

                Execute(context);
		    }

	        public void Execute(Context context)
	        {
                if (_delayBeforeCheck > 0)
                {
                    context.LogInfo("Waiting for {0} seconds before checking the event log.", _delayBeforeCheck);
                    System.Threading.Thread.Sleep(_delayBeforeCheck * 1000);
                }

                var entryType = (EventLogEntryType)Enum.Parse(typeof(EventLogEntryType), _type, true);

	            DateTime cutOffTime = context.TestCaseStart;
                // Note: event log time is always truncated, so the cut off time also need sto be!
                cutOffTime = cutOffTime.Subtract(new TimeSpan(0, 0, 0, 0, context.TestCaseStart.Millisecond + 1)); 

                bool found = false;
                using (var log = new EventLog(_eventLog, _machine))
                {
                    var entries = log.Entries;

                    context.LogInfo("Scanning {0} event log entries from log: '{1}' on machine: '{2}', cutOffTime: '{3}'.", entries.Count, _eventLog, _machine, cutOffTime.ToString("HH:mm:ss.fff dd/MM/yyyy"));
                    for (int i = entries.Count - 1; i >= 0; i--)
                    {
                        var entry = entries[i];
                        if (0 > (DateTime.Compare(entry.TimeGenerated, cutOffTime)))
                        {
                            context.LogInfo("Scanning of event log stopped, event.TimeGenerated: {0}, cutOffTime: {1}", entry.TimeGenerated.ToString("HH:mm:ss.fff dd/MM/yyyy"), cutOffTime.ToString("HH:mm:ss.fff dd/MM/yyyy")); 
                            break;
                        }

                        context.LogInfo("Checking entry, Source: {0}, EntryType: {1}, EventId: {2}", entry.Source, entry.EntryType, entry.InstanceId);

                        // Note: EventId is optional...
                        if (((entry.Source == _source) && (entry.EntryType == entryType)) &&
                             (((_eventId > 0) && (entry.InstanceId == _eventId)) || (_eventId == 0)))
                        {
                            foreach (string validationRegex in _validationRegexs)
                            {
                                string matchPattern = validationRegex;
                                Match match = Regex.Match(entry.Message, matchPattern);

                                if (match.Success)
                                {
                                    found = true;
                                    context.LogInfo("Successfully matched event log entry generated at '{0}'.", entry.TimeGenerated);
                                    context.LogData("Event log entry.", entry.Message);
                                    break;
                                }
                            }
                        }

                        if (found)
                        {
                            break;
                        }
                    }
                }

                // Check that its ok
                if (!_failIfFound && !found)
                {
                    throw new ApplicationException("Failed to find expected event log entry.");
                }
                
                if (_failIfFound && found)
                {
                    throw new ApplicationException("Found event log entry which should not be present.");
                }
            }

	        public void Validate(Context context)
	        {
                if (string.IsNullOrEmpty(_machine))
                {
                    _machine = Environment.MachineName;
                }

                _entryType = (EventLogEntryType)Enum.Parse(typeof(EventLogEntryType), _type, true);
            }
	    }
    }
