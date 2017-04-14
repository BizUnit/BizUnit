//---------------------------------------------------------------------
// File: TestStepExecutionException.cs
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

namespace BizUnit.TestBuilder
{
    /// <summary>
    /// TestStepExecutionException is thrown by BizUnit to indicate a validation step failed.
    /// </summary>
    /// <remarks>The ValidationStepExecutionException is thrown by BizUnit when a validation step fails, the 
    /// framework automatically wraps the exception thrown by the validaiton step with an 
    /// TestStepExecutionException</remarks>
    public class TestStepExecutionException : Exception
    {
#pragma warning disable CS1573 // Parameter 'stage' has no matching param tag in the XML comment for 'TestStepExecutionException.TestStepExecutionException(string, TestStage, string, string)' (but other parameters do)
#pragma warning disable CS1573 // Parameter 'testStepName' has no matching param tag in the XML comment for 'TestStepExecutionException.TestStepExecutionException(string, TestStage, string, string)' (but other parameters do)
        /// <summary>
        /// TestStepExecutionException constructor.
        /// </summary>
        /// <param name="message">The message associated with this exception.</param>
        /// <param name="testCaseName">The name of the BizUnit test case executing whilst the validation step failed.</param>
        /// 
        /// <remarks>
        /// The following example demonstrates how to create and call BizUnit using 
        /// the BizUnit Test Case Object Model:
        ///	</remarks>
        public TestStepExecutionException(string message, TestStage stage, string testCaseName, string testStepName)
#pragma warning restore CS1573 // Parameter 'testStepName' has no matching param tag in the XML comment for 'TestStepExecutionException.TestStepExecutionException(string, TestStage, string, string)' (but other parameters do)
#pragma warning restore CS1573 // Parameter 'stage' has no matching param tag in the XML comment for 'TestStepExecutionException.TestStepExecutionException(string, TestStage, string, string)' (but other parameters do)
            : base(message) 
        {
            Stage = stage;
            TestCaseName = testCaseName;
            TestStepName = testStepName;
        }

        /// <summary>
        /// TestStepExecutionException constructor.
        /// </summary>
        /// <param name="message">The message associated with this exception.</param>
        /// <param name="testCaseName">The name of the BizUnit test case executing whilst the validation step failed.</param>
        /// 
        /// <remarks>
        /// The following example demonstrates how to create and call BizUnit using 
        /// the BizUnit Test Case Object Model:
        ///	</remarks>
        public TestStepExecutionException(
            string message, 
#pragma warning disable CS1573 // Parameter 'innerException' has no matching param tag in the XML comment for 'TestStepExecutionException.TestStepExecutionException(string, Exception, TestStage, string, string)' (but other parameters do)
            Exception innerException, 
#pragma warning restore CS1573 // Parameter 'innerException' has no matching param tag in the XML comment for 'TestStepExecutionException.TestStepExecutionException(string, Exception, TestStage, string, string)' (but other parameters do)
#pragma warning disable CS1573 // Parameter 'stage' has no matching param tag in the XML comment for 'TestStepExecutionException.TestStepExecutionException(string, Exception, TestStage, string, string)' (but other parameters do)
            TestStage stage, 
#pragma warning restore CS1573 // Parameter 'stage' has no matching param tag in the XML comment for 'TestStepExecutionException.TestStepExecutionException(string, Exception, TestStage, string, string)' (but other parameters do)
            string testCaseName, 
#pragma warning disable CS1573 // Parameter 'testStepName' has no matching param tag in the XML comment for 'TestStepExecutionException.TestStepExecutionException(string, Exception, TestStage, string, string)' (but other parameters do)
            string testStepName)
#pragma warning restore CS1573 // Parameter 'testStepName' has no matching param tag in the XML comment for 'TestStepExecutionException.TestStepExecutionException(string, Exception, TestStage, string, string)' (but other parameters do)
            : base(message, innerException)
        {
            Stage = stage;
            TestCaseName = testCaseName;
            TestStepName = testStepName;
        }

        /// <summary>
        /// The name of the test case
        /// </summary>
        public string TestCaseName { get; private set; }

        /// <summary>
        /// The name of the test step
        /// </summary>
        public string TestStepName { get; private set; }

        /// <summary>
        /// The test stage being executed
        /// </summary>
        public TestStage Stage { get; private set; }
    }
}
