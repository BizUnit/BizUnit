//---------------------------------------------------------------------
// File: ILogger.cs
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

namespace BizUnit
{
    /// <summary>
    /// The LogLevel enum defines the level of logging. 
    /// </summary>
    public enum LogLevel { INFO, WARNING, ERROR };

    /// <summary>
    /// The ILogger interface is implemented by custom loggers. This enables the BizUnit log output 
    /// to directed to custom syncs.
    /// </summary>
    public interface ILogger : ICloneable
    {
        /// <summary>
        /// Gets or set the concurrency mode, test steps that are executing concurrently will have 
        /// their own instance of a _logger and will have this property set to true.
        /// </summary>
        bool ConcurrentExecutionMode { set; get; }

        /// <summary>
        /// TestGroupStart is called by the BizUnit framework if/when a Test Group is started. 
        /// Note, test groups are optional.
        /// </summary>
        /// 
        /// <param name='testGroupName'>The name of the test group.</param>
        /// <param name='testGroupPhase'>The phase of the test group, e.g. setup or tear down</param>
        /// <param name='time'>The date time that this was invoked</param>
        /// <param name='userName'>The user name that BizUnit is running under</param>
        void TestGroupStart(string testGroupName, TestGroupPhase testGroupPhase, DateTime time, string userName);

        /// <summary>
        /// TestGroupEnd is called by the BizUnit framework if/when a Test Group phase ends. 
        /// Note, test groups are optional.
        /// </summary>
        /// 
        /// <param name='testGroupPhase'>The phase of the test group, e.g. setup or tear down</param>
        /// <param name='time'>The date time that this was invoked</param>
        /// <param name='executionException'>Will be null unless the phase exected with an error</param>
        void TestGroupEnd(TestGroupPhase testGroupPhase, DateTime time, Exception executionException);

        /// <summary>
        /// TestStart is called by the BizUnit framework at the start of a new test. 
        /// </summary>
        /// 
        /// <param name='testName'>The name of the test group.</param>
        /// <param name='time'>The date time that this was invoked</param>
        /// <param name='userName'>The user name that BizUnit is running under</param>
        void TestStart(string testName, DateTime time, string userName);

        /// <summary>
        /// TestEnd is called by the BizUnit framework at the end of a new test. 
        /// </summary>
        /// 
        /// <param name='testName'>The name of the test group.</param>
        /// <param name='time'>The date time that this was invoked</param>
        /// <param name='ex'>Will be null unless the test exected with an error</param>
        void TestEnd(string testName, DateTime time, Exception ex);

        /// <summary>
        /// TestStageStart is called by the BizUnit framework at the start of a test stage. 
        /// </summary>
        /// 
        /// <param name='stage'>The stage type, for example setup, execution or clenaup.</param>
        /// <param name='time'>The date time that this was invoked</param>
        void TestStageStart(TestStage stage, DateTime time);

        /// <summary>
        /// TestStageEnd is called by the BizUnit framework at the end of a given test stage. 
        /// </summary>
        /// 
        /// <param name='stage'>The stage type, for example setup, execution or clenaup.</param>
        /// <param name='time'>The date time that this was invoked</param>
        /// <param name='stageException'>Will be null unless the test stage exected with an error</param>
        void TestStageEnd(TestStage stage, DateTime time, Exception stageException);

        /// <summary>
        /// TestStepStart is called by the BizUnit framework at the start of a test step. 
        /// </summary>
        /// 
        /// <param name='testStepName'>The name of the test step being executed.</param>
        /// <param name='time'>The date time that this was invoked</param>
        /// <param name='runConcurrently'>Indicates whether the step is being executed in parallel to other steps</param>
        /// <param name='failOnError'>Indicates whether the step is configured to fail if an error occurs</param>
        void TestStepStart(string testStepName, DateTime time, bool runConcurrently, bool failOnError);

        /// <summary>
        /// TestStepEnd is called by the BizUnit framework at the end of a test step. 
        /// </summary>
        /// 
        /// <param name='testStepName'>The name of the test step being executed.</param>
        /// <param name='time'>The date time that this was invoked</param>
        /// <param name='ex'>The exception that was thrown by the step</param>
        void TestStepEnd(string testStepName, DateTime time, Exception ex);

        /// <summary>
        /// TestStepEnd is called by the BizUnit framework at the end of a test step. 
        /// </summary>
        /// 
        /// <param name='stage'>The test stage that the test step was added to.</param>
        /// <param name='testStepName'>The name of the test step being executed.</param>
        /// <param name='ex'>If failed the exception that was thrown.</param>
        void ValidateTestSteps(TestStage stage, string testStepName, Exception ex);

        /// <summary>
        /// ValidatorStart is called by the BizUnit framework at the start of a validator sub step. 
        /// </summary>
        /// 
        /// <param name='validatorName'>The name of the validator sub step.</param>
        /// <param name='time'>The date time that this was invoked</param>
        void ValidatorStart(string validatorName, DateTime time);

        /// <summary>
        /// ValidatorEnd is called by the BizUnit framework at the end of a validator sub step. 
        /// </summary>
        /// 
        /// <param name='validatorName'>The name of the validator sub step.</param>
        /// <param name='time'>The date time that this was invoked</param>
        /// <param name='ex'>The exception that was thrown by the sub step</param>
        void ValidatorEnd(string validatorName, DateTime time, Exception ex);

        /// <summary>
        /// ContextLoaderStart is called by the BizUnit framework at the start of a context loader sub step. 
        /// </summary>
        /// 
        /// <param name='validatorName'>The name of the validator sub step.</param>
        /// <param name='time'>The date time that this was invoked</param>
        void ContextLoaderStart(string validatorName, DateTime time);

        /// <summary>
        /// ContextLoaderEnd is called by the BizUnit framework at the end of a context loader sub step. 
        /// </summary>
        /// 
        /// <param name='validatorName'>The name of the validator sub step.</param>
        /// <param name='time'>The date time that this was invoked</param>
        /// <param name='ex'>The exception that was thrown by the sub step</param>
        void ContextLoaderEnd(string validatorName, DateTime time, Exception ex);

        /// <summary>
        /// Log is called by the BizUnit framework, by test steps or by sub steps in order to log information, warnings or errors. 
        /// </summary>
        /// 
        /// <param name='logLevel'>The level to log at.</param>
        /// <param name='text'>The text to log</param>
        void Log(LogLevel logLevel, string text);

        /// <summary>
        /// Log is called by the BizUnit framework, by test steps or by sub steps in order to log information, warnings or errors. 
        /// </summary>
        /// 
        /// <param name='logLevel'>The level to log at.</param>
        /// <param name='text'>The text to log</param>
        /// <param name='args'>Arguments to format into the text string</param>
        void Log(LogLevel logLevel, string text, params object[] args);

        /// <summary>
        /// LogException is called by the BizUnit framework, by test steps or by sub steps in order to log exceptions. 
        /// </summary>
        /// 
        /// <param name='e'>The exception to log.</param>
        void LogException(Exception e);

        /// <summary>
        /// LogData is called by test steps or by sub steps in order to log data. 
        /// </summary>
        /// 
        /// <param name='description'>The description of the data being logged.</param>
        /// <param name='data'>The data to log</param>
        void LogData(string description, string data);

        /// <summary>
        /// LogData is called by test steps or by sub steps in order to log Xml data. 
        /// </summary>
        /// 
        /// <param name='description'>The description of the data being logged.</param>
        /// <param name='data'>The data to log</param>
        void LogXmlData(string description, string data);
        
        /// <summary>
        /// Returns the log data that has been buffered whilst a test step has been executed concurrently. 
        /// Test steps that are executing concurrently will have their own instance of a _logger and will have 
        /// this property set to true.
        /// </summary>
        string BufferedText { get; }

        /// <summary>
        /// LogBufferedText is called to log text that has been buffered whilst a test step is executed conrurrently. 
        /// Test steps that are executing concurrently will have their own instance of a _logger and will have 
        /// this property set to true.
        /// </summary>
        /// 
        /// <param name='bufferedLogger'>The instance of the ILogger holding the buffered log data.</param>
        void LogBufferedText(ILogger bufferedLogger);

        /// <summary>
        /// Flush is used to flush the log data to the underlying storage, for eaxmple if the underlying 
        /// storage is a stream, the stream would be flushed during the invokation of this method. 
        /// </summary>
        void Flush();

        /// <summary>
        /// Close is called once the _logger is no longer used. 
        /// </summary>
        void Close();
    }
}
