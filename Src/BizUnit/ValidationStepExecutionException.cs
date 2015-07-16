//---------------------------------------------------------------------
// File: ValidationStepExecutionException.cs
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
    /// ValidationStepExecutionException is thrown by BizUnit to indicate a validation step failed.
    /// </summary>
    /// <remarks>The ValidationStepExecutionException is thrown by BizUnit when a validation step fails, the 
    /// framework automatically wraps the exception thrown by the validaiton step with an 
    /// ValidationStepExecutionException</remarks>
    public class ValidationStepExecutionException : Exception
    {
        /// <summary>
        /// ValidationStepExecutionException constructor.
        /// </summary>
        /// <param name="message">The message associated with this exception.</param>
        /// <param name="testCaseName">The name of the BizUnit test case executing whilst the validation step failed.</param>
        /// 
        /// <remarks>
        /// The following example demonstrates how to create and call BizUnit using 
        /// the BizUnit Test Case Object Model:
        ///	</remarks>
        public ValidationStepExecutionException(string message, string testCaseName)
            : base(message)
        {
            TestCaseName = testCaseName;
        }

        /// <summary>
        /// ValidationStepExecutionException constructor.
        /// </summary>
        /// <param name="message">The message associated with this exception.</param>
        /// <param name="innerException">The exception thrown by the validation step.</param>
        /// <param name="testCaseName">The name of the BizUnit test case executing whilst the validation step failed.</param>
        /// 
        /// <remarks>
        /// The following example demonstrates how to create and call BizUnit using 
        /// the BizUnit Test Case Object Model:
        ///	</remarks>
        public ValidationStepExecutionException(string message, Exception innerException, string testCaseName)
            : base(message, innerException)
        {
            TestCaseName = testCaseName;
        }

        /// <summary>
        /// The name of the test case
        /// </summary>
        public string TestCaseName { get; private set; }
    }
}
