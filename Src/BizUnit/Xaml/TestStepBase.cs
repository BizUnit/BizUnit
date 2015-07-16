//---------------------------------------------------------------------
// File: TestStepBase.cs
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
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace BizUnit.Xaml
{
    ///<summary>
    /// The base class for all test steps. 
    ///</summary>
    public abstract class TestStepBase
    {
        ///<summary>
        /// Whether to execute this test step concurrently or sequentially with 
        /// the other steps within the test case.
        ///</summary>
        public bool RunConcurrently { get; set; }

        ///<summary>
        /// Indicates if the test step should cause the test case to fail 
        /// if the step fails. This is often useful during the cleanup stage 
        /// in order to have the best attempt at cleanup
        ///</summary>
        public bool FailOnError { get; set; }

        ///<summary>
        /// The list of sub-steps to be executed by the test step, there maybe 
        /// zero or more sub-steps. Each sub-step is called, with the data being 
        /// passed from one to the next typically.
        ///</summary>
        public Collection<SubStepBase> SubSteps { get; set; }

        ///<summary>
        /// The exception details should execution fail
        ///</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Exception ExecuteException { get; set; }

        ///<summary>
        /// Executes the test steps logic
        ///</summary>
        ///<param name="context">The test context being used in the current TestCase</param>
        public abstract void Execute(Context context);

        ///<summary>
        /// Executes the test steps validation logic
        ///</summary>
        ///<param name="context">The test context being used in the current TestCase</param>
        public abstract void Validate(Context context);

        ///<summary>
        /// Default constructor
        ///</summary>
        protected TestStepBase()
        {
            FailOnError = true;
        }
    }
}
