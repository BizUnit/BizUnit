//---------------------------------------------------------------------
// File: TestStepBuilder.cs
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
using System.Reflection;
using BizUnit.Common;

namespace BizUnit.BizUnitOM
{
    /// <summary>
    /// The TestStepBuilder abstracts a test step, it is responsible for 
    /// creating and configuring a test step that implements ITestStepOM.
    /// </summary>
    /// 
    /// <remarks>
    /// The following example demonstrates how to use the TestStepBuilder:
    /// 
    /// <code escaped="true">
    /// // Create the TestStepBuilder
    /// TestStepBuilder tsb = new TestStepBuilder("BizUnit.FileCreateStep");
    /// 
    /// // Set the properties on the test step...
    /// object[] args = new object[1];
    /// args[0] = @"..\..\..\Test\BizUnit.Tests\Data\LoadGenScript001.xml";
    /// tsb.SetProperty("SourcePath", args);
    /// 
    /// args = new object[1];
    /// args[0] = @"..\..\..\Test\BizUnit.Tests\Out\Data_%Guid%.xml";
    /// tsb.SetProperty("CreationPath", args);
    /// 
    /// // Create the BizUnitTestCase
    /// BizUnitTestCase testCase = new BizUnitTestCase();
    /// 
    /// // Add the test step builder to the BizUnitTestCase...
    /// testCase.AddTestStep(tsb, TestStage.Execution);
    /// 
    /// // Create and execute an instance of BizUnit...
    /// BizUnit bizUnit = new BizUnit(testCase);
    /// bizUnit.RunTest();
    ///	</code>
    ///	
    ///	</remarks>
    [Obsolete("TestStepBuilder has been deprecated. Please investigate the use of BizUnit.Xaml.TestCase.")]
    public class TestStepBuilder : TestStepBuilderBase
    {
        private readonly ITestStepOM _testStepOm;

        /// <summary>
        /// TestStepBuilder constructor.
        /// </summary>
        /// 
        /// <param name='testStep'>A test step that has already been created and 
        /// that implements ITestStepOM.</param>
        public TestStepBuilder(ITestStepOM testStep)
        {
            ArgumentValidation.CheckForNullReference(testStep, "testStep");

            _testStepOm = testStep;
        }

        /// <summary>
        /// TestStepBuilder constructor.
        /// </summary>
        /// 
        /// <param name='typeName'>The type name of the test step that will be 
        /// created by the TestStepBuilder, the step should implement ITestStepOM.</param>
        public TestStepBuilder(string typeName)
            : base(typeName)
        {
            ArgumentValidation.CheckForEmptyString(typeName, "typeName");

            _testStepOm = RawTestStep as ITestStepOM;
        }

        /// <summary>
        /// TestStepBuilder constructor.
        /// </summary>
        /// 
        /// <param name='typeName'>The type name of the test step that will be 
        /// created by the TestStepBuilder, the step should implement ITestStepOM.</param>
        /// <param name='assemblyPath'>The assembly path of the test step that will be 
        /// created by the TestStepBuilder, the step should implement ITestStepOM.</param>
        public TestStepBuilder(string typeName, string assemblyPath)
            : base(typeName, assemblyPath)
        {
            ArgumentValidation.CheckForEmptyString(typeName, "typeName");
            // assemblyPath - optional

            _testStepOm = RawTestStep as ITestStepOM;
        }

        internal ITestStepOM TestStepOM
        {
            get { return _testStepOm; }
        }

        /// <summary>
        /// Set the ValidationStepBuilder property.
        /// </summary>
        /// 
        /// <value>The Validation sub-step builder that will be executed during the 
        /// execution of the test step.</value>
        public ValidationStepBuilder ValidationStepBuilder { get; set; }

        /// <summary>
        /// Set the ContextLoaderStepBuilder property.
        /// </summary>
        /// 
        /// <value>The Context Loader sub-step builder that will be executed during the 
        /// execution of the test step.</value>
        public ContextLoaderStepBuilder ContextLoaderStepBuilder { get; set; }

        private void SetProperty(Type t, object value)
        {
            PropertyInfo pi = GetProperty(t);
            if (null != pi)
            {
                var args = new object[1];
                args[0] = value;
                pi.GetSetMethod().Invoke(RawTestStep, args);
            }
        }

        private PropertyInfo GetProperty(Type t)
        {
            PropertyInfo[] propertiesInfo = RawTestStep.GetType().GetProperties();
            foreach (PropertyInfo propertyInfo in propertiesInfo)
            {
                if (t == propertyInfo.PropertyType)
                {
                    return propertyInfo;
                }
            }
            return null;
        }

        internal void PrepareSubStepsForExecution(Context ctx)
        {
            if (null != ValidationStepBuilder)
            {
                ValidationStepBuilder.PrepareStepForExecution(ctx);
                SetProperty(typeof(IValidationStepOM), ValidationStepBuilder.ValidationStep);
            }

            if (null != ContextLoaderStepBuilder)
            {
                ContextLoaderStepBuilder.PrepareStepForExecution(ctx);
                SetProperty(typeof(IContextLoaderStepOM), ContextLoaderStepBuilder.ContextLoaderStep);
            }
        }
    }
}
