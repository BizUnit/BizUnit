//---------------------------------------------------------------------
// File: Logger.cs
// 
// Summary: 
//
//---------------------------------------------------------------------
// Copyright (c) 2004-2017, Kevin B. Smith. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------

using System;
using System.Text;
using System.IO;
using System.Xml;

namespace BizUnit.Core.Utilites
{
	/// <summary>
	/// The BizUnit Logger is used to log data from BizUnit and test steps.
	/// </summary>
	public class Logger : ILogger
	{
		StringBuilder _sb = null;
		bool _concurrentExecutionMode = false;
		const string Crlf = "\r\n";
        const string InfoLogLevel = "Info";
        const string ErrorLogLevel = "Error";
        const string WarningLogLevel = "Warning";

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Logger.ConcurrentExecutionMode'
	    public bool ConcurrentExecutionMode
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Logger.ConcurrentExecutionMode'
	    {
	        get
	        {
	            return _concurrentExecutionMode;
	        }

            set
            {
                _concurrentExecutionMode = value;

                if (_concurrentExecutionMode)
                {
                    _sb = new StringBuilder();
                }
            }
	    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Logger.TestStageStart(TestStage, DateTime)'
        public void TestStageStart(TestStage stage, DateTime time)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Logger.TestStageStart(TestStage, DateTime)'
        {
            switch(stage)
            {
                case TestStage.Setup:
                    WriteLine(" ");
                    WriteLine("Setup Stage: started @ {0}", FormatDate(time));
                    break;

                case TestStage.Execution:
                    WriteLine(" ");
                    WriteLine("Execute Stage: started @ {0}", FormatDate(time));
                    break;

                case TestStage.Cleanup:
                    WriteLine(" ");
                    WriteLine("Cleanup Stage: started @ {0}", FormatDate(time));
                    break;
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Logger.TestStageEnd(TestStage, DateTime, Exception)'
        public void TestStageEnd(TestStage stage, DateTime time, Exception stageException)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Logger.TestStageEnd(TestStage, DateTime, Exception)'
        {
            if (null != stageException)
            {
                LogException(stageException);
            }

            WriteLine(" ");
            if (null == stageException)
            {
                WriteLine("{0} Stage: ended @ {1}", stage, FormatDate(time));
            }
            else
            {
                WriteLine("{0} Stage: ended @ {1} with ERROR's", stage, FormatDate(time));
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Logger.LogException(Exception)'
        public void LogException(Exception e)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Logger.LogException(Exception)'
		{
            if (null == e)
            {
                return;    
            }

			WriteLine(new string('*', 79));
            WriteLine("{0}: {1}", ErrorLogLevel, "Exception caught!");
            WriteLine( e.ToString() );
			WriteLine(new string('*', 79));
		}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Logger.LogData(string, string)'
        public void LogData(string description, string data)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Logger.LogData(string, string)'
		{
			WriteLine(new string('~', 79));
			WriteLine( "Data: {0}", description );
			WriteLine(new string('~', 79));
			WriteLine( data );
			WriteLine(new string('~', 79));
		}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Logger.LogXmlData(string, string)'
        public void LogXmlData(string description, string data)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Logger.LogXmlData(string, string)'
        {
            WriteLine(new string('~', 79));
            WriteLine("Data: {0}", description);
            WriteLine(new string('~', 79));
            WriteLine(FormatPrettyPrint(data));
            WriteLine(new string('~', 79));
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Logger.Log(LogLevel, string)'
        public void Log(LogLevel logLevel, string text)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Logger.Log(LogLevel, string)'
		{
			switch(logLevel)
			{
                case (LogLevel.INFO):
                    WriteLine("{0}: {1}", InfoLogLevel, text);
                    break;

                case (LogLevel.WARNING):
                    WriteLine("{0}: {1}", WarningLogLevel, text);
                    break;

                case (LogLevel.ERROR):
                    WriteLine("{0}: {1}", ErrorLogLevel, text);
                    break;

				default:
			        throw new ApplicationException("Invalid log level was set!");
			};
		}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Logger.Log(LogLevel, string, params object[])'
        public void Log(LogLevel logLevel, string text, params object[] args)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Logger.Log(LogLevel, string, params object[])'
		{
            string formattedText = string.Format(text, args);
            Log(logLevel, formattedText);
		}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Logger.LogBufferedText(ILogger)'
        public void LogBufferedText(ILogger logger)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Logger.LogBufferedText(ILogger)'
		{
            if (!logger.ConcurrentExecutionMode)
			{
				throw new ApplicationException("This instance is not a concurrent test step!");
			}

            WriteLine(logger.BufferedText);
		}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Logger.BufferedText'
	    public string BufferedText
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Logger.BufferedText'
	    {
	        get
	        {
                if (null != _sb)
                    return _sb.ToString();

                return null;
	        }
	    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Logger.TestStepStart(string, DateTime, bool, bool)'
        public void TestStepStart(string testStepName, DateTime time, bool runConcurrently, bool failOnError)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Logger.TestStepStart(string, DateTime, bool, bool)'
        {
            WriteLine("");
            if (runConcurrently)
            {
                WriteLine(
                    string.Format("Step: {0} started  c o n c u r r e n t l y  @ {1}, failOnError = {2}", testStepName,
                                  FormatDate(time), failOnError));
            }
            else
            {
                WriteLine(
                    string.Format("Step: {0} started  @ {1}, failOnError = {2}", testStepName,
                                  FormatDate(time), failOnError));
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Logger.TestStepEnd(string, DateTime, Exception)'
        public void TestStepEnd(string testStepName, DateTime time, Exception ex)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Logger.TestStepEnd(string, DateTime, Exception)'
        {
            if (null == ex)
            {
                WriteLine(string.Format("Step: {0} ended @ {1}", testStepName, FormatDate(time)));
            }
            else
            {
                WriteLine(string.Format("Step: {0} ended @ {1} with ERRORS, exception: {2}", testStepName, FormatDate(time), ex.GetType().ToString()));
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Logger.ValidateTestSteps(TestStage, string, Exception)'
        public void ValidateTestSteps(TestStage stage, string testStepName, Exception ex)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Logger.ValidateTestSteps(TestStage, string, Exception)'
	    {
            if (null == ex)
            {
                WriteLine(string.Format("Test step validation for stage: {0}, step: {1} was successful.", stage, testStepName));
            }
            else
            {
                WriteLine(string.Format("Test step validation for stage: {0}, step: {1} failed: {2}", stage, testStepName, ex));
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Logger.TestGroupStart(string, TestGroupPhase, DateTime, string)'
	    public void TestGroupStart(string testGroupName, TestGroupPhase testGroupPhase, DateTime time, string userName)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Logger.TestGroupStart(string, TestGroupPhase, DateTime, string)'
        {
            if (testGroupPhase == TestGroupPhase.TestGroupSetup)
            {
                WriteLine(" ");
                WriteLine(new string('-', 79));
                WriteLine("                        T E S T   G R O U P   S E T U P");
                WriteLine(" ");
                WriteLine(string.Format("Test Group Setup: {0} started @ {1} by {2}", testGroupName, FormatDate(time), userName));
                WriteLine(new string('-', 79));
            }
            else
            {
                WriteLine(" ");
                WriteLine(new string('-', 79));
                WriteLine("                   T E S T   G R O U P   T E A R D O W N");
                WriteLine(" ");
                WriteLine(string.Format("Test Group Tear Down: {0} completed @ {1}", testGroupName, FormatDate(time)));
                WriteLine(new string('-', 79));
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Logger.TestGroupEnd(TestGroupPhase, DateTime, Exception)'
        public void TestGroupEnd(TestGroupPhase testGroupPhase, DateTime time, Exception executionException)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Logger.TestGroupEnd(TestGroupPhase, DateTime, Exception)'
        {
            if (testGroupPhase == TestGroupPhase.TestGroupSetup)
            {
                if (null != executionException)
                {
                    WriteLine(string.Format("Test Group Setup completed @ {0}", FormatDate(time)));
                    WriteLine("          ****** T E S T   G R O U P   S E T U P   F A I L E D ******");
                }
                else
                {
                    WriteLine(string.Format("Test Group Setup completed @ {0}", FormatDate(time)));
                    WriteLine("                  T E S T   G R O U P   S E T U P   P A S S");
                }
            }
            else
            {
                if (null != executionException)
                {
                    WriteLine(string.Format("Test Group Tear Down completed @ {0}", FormatDate(time)));
                    WriteLine("       ****** T E S T   G R O U P   T E A R D O W N   F A I L E D ******");
                }
                else
                {
                    WriteLine(string.Format("Test Group Tear Down completed @ {0}", FormatDate(time)));
                    WriteLine("              T E S T   G R O U P   T E A R D O W N   P A S S");
                }
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Logger.TestStart(string, DateTime, string)'
        public void TestStart(string testName, DateTime time, string userName)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Logger.TestStart(string, DateTime, string)'
        {
            WriteLine(" ");
            WriteLine(new string('-', 79));
            WriteLine("                                   S T A R T");
            WriteLine(" ");
            WriteLine(string.Format("Test: {0} started @ {1} by {2}", testName, FormatDate(time), userName));
            WriteLine(new string('-', 79));
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Logger.TestEnd(string, DateTime, Exception)'
        public void TestEnd(string testName, DateTime time, Exception ex)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Logger.TestEnd(string, DateTime, Exception)'
        {
            LogException(ex);
            WriteLine(new string('-', 79));
            WriteLine(string.Format("Test: {0} ended @ {1}", testName, FormatDate(time)));
            WriteLine("");
            if (null != ex)
            {
                WriteLine("                             ****** F A I L ******");
            }
            else
            {
                WriteLine("                                    P A S S");
            }

            WriteLine(new string('-', 79));
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Logger.ValidatorStart(string, DateTime)'
        public void ValidatorStart(string validatorName, DateTime time)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Logger.ValidatorStart(string, DateTime)'
        {
            WriteLine("");
            WriteLine(string.Format("Validation: {0} started @ {1}", validatorName, FormatDate(time)));
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Logger.ValidatorEnd(string, DateTime, Exception)'
        public void ValidatorEnd(string validatorName, DateTime time, Exception ex)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Logger.ValidatorEnd(string, DateTime, Exception)'
        {
            if(null == ex)
            {
                WriteLine(string.Format("Validation: {0} ended @ {1}", validatorName, FormatDate(time)));
            }
            else
            {
                WriteLine(string.Format("Validation: {0} ended @ {1} with ERRORS, exception: {2}", validatorName, FormatDate(time), ex.GetType().ToString()));
            }
            WriteLine("");
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Logger.ContextLoaderStart(string, DateTime)'
        public void ContextLoaderStart(string contextLoaderName, DateTime time)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Logger.ContextLoaderStart(string, DateTime)'
        {
            WriteLine("");
            WriteLine(string.Format("ContextLoad: {0} started @ {1}", contextLoaderName, FormatDate(time)));
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Logger.ContextLoaderEnd(string, DateTime, Exception)'
        public void ContextLoaderEnd(string contextLoaderName, DateTime time, Exception ex)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Logger.ContextLoaderEnd(string, DateTime, Exception)'
        {
            if (null == ex)
            {
                WriteLine(string.Format("ContextLoad: {0} ended @ {1}", contextLoaderName, FormatDate(time)));
            }
            else
            {
                WriteLine(string.Format("ContextLoad: {0} ended @ {1} with ERRORS, exception: {2}", contextLoaderName, FormatDate(time), ex.GetType().ToString()));
            }
            WriteLine("");
        }
        
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Logger.Clone()'
        public object Clone()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Logger.Clone()'
	    {
	        return new Logger();
	    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Logger.Flush()'
        public void Flush()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Logger.Flush()'
        {
            string buff = BufferedText;
            if(null != buff)
            {
                WriteLine(buff);
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Logger.Close()'
        public void Close()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Logger.Close()'
        {
        }

        private static string FormatDate(DateTime time)
        {
            return time.ToString("HH:mm:ss.fff dd/MM/yyyy");
        }

        private void WriteLine(string s)
        {
            if (_concurrentExecutionMode)
            {
                _sb.Append(s);
                _sb.Append(Crlf);
            }
            else
            {
                Console.WriteLine(s);
            }
        }

        private void WriteLine(string s, params object[] args)
        {
            if (_concurrentExecutionMode)
            {
                _sb.Append(String.Format(s, args));
                _sb.Append(Crlf);
            }
            else
            {
                Console.WriteLine(s, args);
            }
        }

        private static string FormatPrettyPrint(string data)
        {
            var doc = new XmlDocument();
            doc.LoadXml(data);

            return FormatPrettyPrint(doc);
        }

        private static string FormatPrettyPrint(XmlDocument doc)
        {
            var ms = new MemoryStream();
            var tw = new XmlTextWriter(ms, Encoding.Unicode) {Formatting = Formatting.Indented};

            doc.WriteContentTo(tw);
            tw.Flush();
            ms.Flush();
            ms.Seek(0, SeekOrigin.Begin);

            var sr = new StreamReader(ms);
            return sr.ReadToEnd();
        }
    }
}
