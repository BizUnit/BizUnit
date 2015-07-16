//---------------------------------------------------------------------
// File: Logger.cs
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
using System.Text;
using System.IO;
using System.Xml;

namespace BizUnit
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

	    public bool ConcurrentExecutionMode
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

        public void TestStageStart(TestStage stage, DateTime time)
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

        public void TestStageEnd(TestStage stage, DateTime time, Exception stageException)
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

        public void LogException(Exception e)
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

        public void LogData(string description, string data)
		{
			WriteLine(new string('~', 79));
			WriteLine( "Data: {0}", description );
			WriteLine(new string('~', 79));
			WriteLine( data );
			WriteLine(new string('~', 79));
		}

        public void LogXmlData(string description, string data)
        {
            WriteLine(new string('~', 79));
            WriteLine("Data: {0}", description);
            WriteLine(new string('~', 79));
            WriteLine(FormatPrettyPrint(data));
            WriteLine(new string('~', 79));
        }

        public void Log(LogLevel logLevel, string text)
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

        public void Log(LogLevel logLevel, string text, params object[] args)
		{
            string formattedText = string.Format(text, args);
            Log(logLevel, formattedText);
		}

        public void LogBufferedText(ILogger logger)
		{
            if (!logger.ConcurrentExecutionMode)
			{
				throw new ApplicationException("This instance is not a concurrent test step!");
			}

            WriteLine(logger.BufferedText);
		}

	    public string BufferedText
	    {
	        get
	        {
                if (null != _sb)
                    return _sb.ToString();

                return null;
	        }
	    }

        public void TestStepStart(string testStepName, DateTime time, bool runConcurrently, bool failOnError)
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

        public void TestStepEnd(string testStepName, DateTime time, Exception ex)
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

        public void ValidateTestSteps(TestStage stage, string testStepName, Exception ex)
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

	    public void TestGroupStart(string testGroupName, TestGroupPhase testGroupPhase, DateTime time, string userName)
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

        public void TestGroupEnd(TestGroupPhase testGroupPhase, DateTime time, Exception executionException)
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

        public void TestStart(string testName, DateTime time, string userName)
        {
            WriteLine(" ");
            WriteLine(new string('-', 79));
            WriteLine("                                   S T A R T");
            WriteLine(" ");
            WriteLine(string.Format("Test: {0} started @ {1} by {2}", testName, FormatDate(time), userName));
            WriteLine(new string('-', 79));
        }

        public void TestEnd(string testName, DateTime time, Exception ex)
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

        public void ValidatorStart(string validatorName, DateTime time)
        {
            WriteLine("");
            WriteLine(string.Format("Validation: {0} started @ {1}", validatorName, FormatDate(time)));
        }

        public void ValidatorEnd(string validatorName, DateTime time, Exception ex)
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

        public void ContextLoaderStart(string contextLoaderName, DateTime time)
        {
            WriteLine("");
            WriteLine(string.Format("ContextLoad: {0} started @ {1}", contextLoaderName, FormatDate(time)));
        }

        public void ContextLoaderEnd(string contextLoaderName, DateTime time, Exception ex)
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
        
        public object Clone()
	    {
	        return new Logger();
	    }

        public void Flush()
        {
            string buff = BufferedText;
            if(null != buff)
            {
                WriteLine(buff);
            }
        }

        public void Close()
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
