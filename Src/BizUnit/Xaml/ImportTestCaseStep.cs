//---------------------------------------------------------------------
// File: ImportTestCaseStep.cs
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

using BizUnit.Common;

namespace BizUnit.Xaml
{
    ///<summary>
    /// Used to import another test case into this test case. This is especially
    /// useful when testing logic that builds on different scenarios. Each scenario
    /// may have a test case, subsequent scenarios may import their dependant tests
    /// to avoid having to duplicate test code.
    ///</summary>
    sealed public class ImportTestCaseStep : TestStepBase
    {
        ///<summary>
        /// The file path of the test case to be imported. Either TestCase should be set or TestCasePath, but not both.
        ///</summary>
        public string TestCasePath { get; set; }

        ///<summary>
        /// The test case to be imported. Either TestCase should be set or TestCasePath, but not both.
        ///</summary>
        public TestCase TestCase { get; set; }

        internal TestCase GetTestCase()
        {
            if(null != TestCase)
                return TestCase;

            return TestCase.LoadFromFile(TestCasePath);
        }

        public override void Execute(Context context)
        {
            ; // no op
        }

        public override void Validate(Context context)
        {
            if(string.IsNullOrEmpty(TestCasePath))
            {
                ArgumentValidation.CheckForNullReference(TestCase, "TestCase");
            }

            if (null == TestCase)
            {
                ArgumentValidation.CheckForEmptyString(TestCasePath, "TestCasePath");
            }
        }
    }
}
