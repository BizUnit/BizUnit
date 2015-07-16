//---------------------------------------------------------------------
// File: BizUnitTestCase.cs
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
using BizUnit.Common;
using System.Xml;
using System.Collections.Generic;

namespace BizUnit.BizUnitOM
{
    /// <summary>
    /// The BizUnitTestCase represents a BizUnit Test Case when using the object model.
    /// The BizUnitTestCase is created and test steps are added to it, this is done either 
    /// by directly creating the test steps, or by creating TestStepBuilder and add it to 
    /// the BizUnitTestCase. Using a TestStepBuilder means that the creation of the test 
    /// step is delegated to the TestStepBuilder.
    /// Once the BizUnitTestCase has been constructed, it may be passed to BizUnit 
    /// and directly executed.
    /// </summary>
    /// 
    /// <remarks>
    /// The following example demonstrates how to use the BizUnitTestCase:
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
    [Obsolete("BizUnitTestCase has been deprecated. Please investigate the use of BizUnit.Xaml.TestCase.")]
    public class BizUnitTestCase
    {
        private readonly IList<BizUnitTestStepWrapper> _setupSteps = new List<BizUnitTestStepWrapper>();
        private readonly IList<BizUnitTestStepWrapper> _executionSteps = new List<BizUnitTestStepWrapper>();
        private readonly IList<BizUnitTestStepWrapper> _cleanupSteps = new List<BizUnitTestStepWrapper>();

        /// <summary>
        /// Constructor for BizUnitTestCase
        /// </summary>
        /// 
        /// <param name='name'>The name of the test case</param>
        public BizUnitTestCase(string name)
        {
            ArgumentValidation.CheckForNullReference(name, "name");

            Name = name;
        }

        /// <summary>
        /// Gets and sets the name of the test case
        /// </summary>
        /// <value>The name of the test case.</value>
        public string Name { get; set; }

        /// <summary>
        /// Used to add a test step to a test case at a specific stage of the test.
        /// </summary>
        /// 
        /// <param name='testStep'>The test step to add to the test case, 
        /// creation of the test step is delegated to the TestStepBuilder</param>
        /// <param name='stage'>The stage of the test case in which to add the test step</param>
        public void AddTestStep(TestStepBuilder testStep, TestStage stage)
        {
            ArgumentValidation.CheckForNullReference(testStep, "testStep");

            AddTestStep(testStep, stage, false, true);
        }

        /// <summary>
        /// Used to add a test step to a test case at a specific stage of the test.
        /// </summary>
        /// 
        /// <param name='testStep'>The test step to add to the test case, 
        /// creation of the test step is delegated to the TestStepBuilder</param>
        /// <param name='stage'>The stage of the test case in which to add the test step</param>
        /// <param name='runConcurrently'>Specifies whether the test step 
        /// should run concurrently to other test steps. Defaults to false if not specified.</param>
        /// <param name='failOnError'>Specifies whether the entire test case 
        /// should fail if this individual test step fails, defaults to true if not specified.</param>
        public void AddTestStep(TestStepBuilder testStep, TestStage stage, bool runConcurrently, bool failOnError)
        {
            ArgumentValidation.CheckForNullReference(testStep, "testStep");

            AddTestStepInternal(new BizUnitTestStepWrapper(testStep, runConcurrently, failOnError), stage);
        }

        /// <summary>
        /// Used to add a test step to a test case at a specific stage of the test.
        /// </summary>
        /// 
        /// <param name='testStep'>The test step to add to the test case.</param>
        /// <param name='config'>The configuration for the test step to be used when it is executed.</param>
        /// <param name='stage'>The stage of the test case in which to add the test step</param>
        public void AddTestStep(ITestStep testStep, string config, TestStage stage)
        {
            ArgumentValidation.CheckForNullReference(testStep, "testStep");
            ArgumentValidation.CheckForNullReference(config, "config");

            var doc = new XmlDocument();
            doc.LoadXml(config);
            XmlNode configNode = doc.DocumentElement;

            AddTestStepInternal(new BizUnitTestStepWrapper(testStep, configNode), stage);
        }

        /// <summary>
        /// Used to add a test step to a test case at a specific stage of the test.
        /// </summary>
        /// 
        /// <param name='testStep'>The test step to add to the test case.</param>
        /// <param name='config'>The configuration for the test step to be used when it is executed.</param>
        /// <param name='stage'>The stage of the test case in which to add the test step</param>
        /// <param name='runConcurrently'>Specifies whether the test step 
        /// should run concurrently to other test steps. Defaults to false if not specified.</param>
        /// <param name='failOnError'>Specifies whether the entire test case 
        /// should fail if this individual test step fails, defaults to true if not specified.</param>
        public void AddTestStep(ITestStep testStep, string config, TestStage stage, bool runConcurrently, bool failOnError)
        {
            ArgumentValidation.CheckForNullReference(testStep, "testStep");
            ArgumentValidation.CheckForNullReference(config, "config");

            var doc = new XmlDocument();
            doc.LoadXml(config);
            XmlNode configNode = doc.DocumentElement;

            AddTestStepInternal(new BizUnitTestStepWrapper(testStep, configNode, runConcurrently, failOnError), stage);
        }

        /// <summary>
        /// Used to add a test step to a test case at a specific stage of the test.
        /// </summary>
        /// 
        /// <param name='testStep'>The test step to add to the test case.</param>
        /// <param name='stage'>The stage of the test case in which to add the test step</param>
        public void AddTestStep(ITestStepOM testStep, TestStage stage)
        {
            AddTestStep(testStep, stage, false, true);
        }

        /// <summary>
        /// Used to add a test step to a test case at a specific stage of the test.
        /// </summary>
        /// 
        /// <param name='testStep'>The test step to add to the test case.</param>
        /// <param name='stage'>The stage of the test case in which to add the test step</param>
        /// <param name='runConcurrently'>Specifies whether the test step 
        /// should run concurrently to other test steps. Defaults to false if not specified.</param>
        /// <param name='failOnError'>Specifies whether the entire test case 
        /// should fail if this individual test step fails, defaults to true if not specified.</param>
        public void AddTestStep(ITestStepOM testStep, TestStage stage, bool runConcurrently, bool failOnError)
        {
            ArgumentValidation.CheckForNullReference(testStep, "testStep");
            ArgumentValidation.CheckForNullReference(stage, "stage");

            AddTestStepInternal(new BizUnitTestStepWrapper(testStep, runConcurrently, failOnError), stage);
        }

        private void AddTestStepInternal(BizUnitTestStepWrapper stepWrapper, TestStage stage)
        {
            switch (stage)
            {
                case TestStage.Setup:
                    _setupSteps.Add(stepWrapper);
                    break;
                case TestStage.Execution:
                    _executionSteps.Add(stepWrapper);
                    break;
                case TestStage.Cleanup:
                    _cleanupSteps.Add(stepWrapper);
                    break;
            }
        }

        /// <summary>
        /// Gets the test setup steps.
        /// </summary>
        /// <value>The test setup steps.</value>
        public IList<BizUnitTestStepWrapper> SetupSteps
        {
            get
            {
                return _setupSteps;
            }
        }

        /// <summary>
        /// Gets the test execution steps.
        /// </summary>
        /// <value>The test execution steps.</value>
        public IList<BizUnitTestStepWrapper> ExecutionSteps
        {
            get
            {
                return _executionSteps;
            }
        }

        /// <summary>
        /// Gets the test cleanup steps.
        /// </summary>
        /// <value>The test cleanup steps.</value>
        public IList<BizUnitTestStepWrapper> CleanupSteps
        {
            get
            {
                return _cleanupSteps;
            }
        }
    }
}
