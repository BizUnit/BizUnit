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

using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using BizUnit.Xaml;

namespace BizUnit.TestSteps.EventLog
{
    /// <summary>
    /// The EventLogCheckStep test step looks for an event log entry. 
    /// Note: this test step only looks for the event log entry during the time period of the test case.
    /// </summary>
    public class EventLogCheckStep : TestStepBase
    {
        ///<summary>
        /// The time period to wait before checking the event log
        ///</summary>
        public int DelayBeforeCheck { get; set; }

        ///<summary>
        /// The machine name on which to check the event log
        ///</summary>
        public string Machine { get; set; }

        ///<summary>
        /// The event log to check, e.g. Application, System etc.
        ///</summary>
        public string EventLog { get; set; }

        ///<summary>
        /// The event log source, e.g. BizTalk Server 2010
        ///</summary>
        public string Source { get; set; }

        ///<summary>
        /// The ID of the event to look for (optional)
        ///</summary>
        public int EventId { get; set; }

        ///<summary>
        /// Flag to indicate whether the test step should fail if the event log entry is not present.
        ///</summary>
        public bool FailIfFound { get; set; }

        ///<summary>
        /// The event type, e.g. Info, Warn, Error
        ///</summary>
        public EventLogEntryType EntryType { get; set; }

        ///<summary>
        /// Regular expression used to check the event message.
        ///</summary>
        public Collection<string> ValidationRegExs { get; set; }

        ///<summary>
        /// Default constructor
        ///</summary>
        public EventLogCheckStep()
        {
            ValidationRegExs = new Collection<string>();
        }

        /// <summary>
        /// ITestStep.Execute() implementation
        /// </summary>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public override void Execute(Context context)
        {
            if (DelayBeforeCheck > 0)
            {
                context.LogInfo("Waiting for {0} seconds before checking the event log.", DelayBeforeCheck);
                System.Threading.Thread.Sleep(DelayBeforeCheck * 1000);
            }

            DateTime cutOffTime = context.TestCaseStart;
            // Note: event log time is always truncated, so the cut off time also need sto be!
            cutOffTime = cutOffTime.Subtract(new TimeSpan(0, 0, 0, 0, context.TestCaseStart.Millisecond + 1));

            bool found = false;
            using (var log = new System.Diagnostics.EventLog(EventLog, Machine))
            {
                EventLogEntryCollection entries = log.Entries;

                context.LogInfo("Scanning {0} event log entries from log: '{1}' on machine: '{2}', cutOffTime: '{3}'.", entries.Count, EventLog, Machine, cutOffTime.ToString("HH:mm:ss.fff dd/MM/yyyy"));
                for (int i = entries.Count - 1; i >= 0; i--)
                {
                    EventLogEntry entry = entries[i];
                    if (0 > (DateTime.Compare(entry.TimeGenerated, cutOffTime)))
                    {
                        context.LogInfo("Scanning of event log stopped, event.TimeGenerated: {0}, cutOffTime: {1}", entry.TimeGenerated.ToString("HH:mm:ss.fff dd/MM/yyyy"), cutOffTime.ToString("HH:mm:ss.fff dd/MM/yyyy"));
                        break;
                    }

                    context.LogInfo("Checking entry, Source: {0}, EntryType: {1}, EventId: {2}", entry.Source, entry.EntryType, entry.InstanceId);

                    // Note: EventId is optional...
                    if (((entry.Source == Source) && (entry.EntryType == EntryType)) &&
                         (((EventId > 0) && (entry.InstanceId == EventId)) || (EventId == 0)))
                    {
                        foreach (string validationRegex in ValidationRegExs)
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
            if (!FailIfFound && !found)
            {
                throw new ApplicationException("Failed to find expected event log entry.");
            }

            if (FailIfFound && found)
            {
                throw new ApplicationException("Found event log entry which should not be present.");
            }
        }

        public override void Validate(Context context)
        {
            if (string.IsNullOrEmpty(Machine))
            {
                Machine = Environment.MachineName;
            }
        }
    }
}
