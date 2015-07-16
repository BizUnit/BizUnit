//---------------------------------------------------------------------
// File: TestCase.cs
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
using System.IO;
using System.Reflection;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using BizUnit.Common;

namespace BizUnit.Xaml
{
    ///<summary>
    /// TestCase maybe used to create a test case programatically. Test steps 
    /// should be added to the appropriate stage for subsequent execution.
    /// The TestCase is executed via BizUnit.
    /// A TestCase maybe serialised into Xaml using TestCase.SaveToFile() or
    /// loaded from a Xaml file using TestCase.LoadFromFile().
    ///</summary>
    /// 
    /// <remarks>
    /// The exmaple below illustrates loading and running a Xaml TestCase:
    /// 
    /// <code escaped="true">
    ///	namespace WoodgroveBank.BVTs
    ///	{
    ///	using System;
    ///	using NUnit.Framework;
    ///	using BizUnit;
    ///
    ///	[TestMethod]
    ///	public class SampleTests
    ///	{
    ///		[Test]
    ///		public void ExecuteXamlTestCase()
    ///		{
    ///         // Load the Xaml test case...
    ///         var bu = new BizUnit(TestCase.LoadFromFile("DelayTestCaseTest.xaml"));
    ///         
    ///         // Run the test...
    ///         bu.RunTest();
    ///		}
    ///	}		
    ///	</code>
    ///	
    /// The exmaple below illustrates programtically creating a TestCase and subsequently running it:
    /// 
    /// <code escaped="true">
    ///	namespace WoodgroveBank.BVTs
    ///	{
    ///	using System;
    ///	using NUnit.Framework;
    ///	using BizUnit;
    ///
    ///	[TestMethod]
    ///	public class SampleTests
    ///	{
    ///		[Test]
    ///		public void ExecuteProgramaticallyCreatedTestCase()
    ///		{
    ///         int stepDelayDuration = 500;
    ///         var step = new DelayStep();
    ///         step.DelayMilliSeconds = stepDelayDuration;
    ///
    ///         var sw = new Stopwatch();
    ///         sw.Start();
    ///
    ///         var tc = new TestCase();
    ///         tc.ExecutionSteps.Add(step);
    ///         
    ///         // If we wanted to serialise the test case:
    ///         // TestCase.SaveToFile(tc, "DelayTestCaseTest.xaml");
    /// 
    ///         var bu = new BizUnit(tc));
    ///
    ///         sw = new Stopwatch().Start();
    ///
    ///         // Run the test case...
    ///         bu.RunTest();
    ///
    ///         var actualDuration = sw.ElapsedMilliseconds;
    ///         Console.WriteLine("Observed delay: {0}", actualDuration);
    ///         Assert.AreEqual(actualDuration, stepDelayDuration, 20);
    ///		}
    ///	}		
    ///	</code>
    /// 
    ///	</remarks>
    public class TestCase
    {
        private Collection<TestStepBase> _setupSteps;
        private Collection<TestStepBase> _executionSteps;
        private Collection<TestStepBase> _cleanupSteps;

        ///<summary>
        /// The name of the test case
        ///</summary>
        public string Name { get; set; }

        ///<summary>
        /// The description of what the test case does
        ///</summary>
        public string Description { get; set; }

        ///<summary>
        /// The category of the test case, for example Build Varification 
        /// Test (BVT), System Integration Test, User Acceptance Test, etc.
        ///</summary>
        public string Category { get; set; }

        ///<summary>
        /// The description of what the test case is designed to test
        ///</summary>
        public string Purpose { get; set; }

        ///<summary>
        /// A reference for the test, e.g. Usecase 101
        ///</summary>
        public string Reference { get; set; }

        ///<summary>
        /// Details of any preconditions required prior to running the test
        ///</summary>
        public string Preconditions { get; set; }

        ///<summary>
        /// The expected results from execution of the test
        ///</summary>
        public string ExpectedResults { get; set; }

        ///<summary>
        /// The version of BizUnit that was used to generate this test
        ///</summary>
        public string BizUnitVersion { get; set; }

        ///<summary>
        /// Default constructor
        ///</summary>
        public TestCase()
        {
            BizUnitVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        ///<summary>
        /// The test steps to be executed in the Setup stage of the test
        ///</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Collection<TestStepBase> SetupSteps 
        {
            get 
            {
                return _setupSteps ?? (_setupSteps = new Collection<TestStepBase>()); 
            } 
        }

        ///<summary>
        /// The test steps to be executed in the Execution stage of the test
        ///</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Collection<TestStepBase> ExecutionSteps
        {
            get
            {
                return _executionSteps ?? (_executionSteps = new Collection<TestStepBase>());
            }
        }

        ///<summary>
        /// The test steps to be executed in the Cleanup stage of the test, 
        /// these will always be executed even if the test fails
        ///</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Collection<TestStepBase> CleanupSteps
        {
            get
            {
                return _cleanupSteps ?? (_cleanupSteps = new Collection<TestStepBase>());
            }
        }

        ///<summary>
        /// Validates that the test has been correctly setup, this is called 
        /// by BizUnit, though may also be called from user code if required.
        ///</summary>
        ///<param name="ctx"></param>
        public void Validate(Context ctx)
        {
            ArgumentValidation.CheckForNullReference(ctx, "ctx");

            ValidateSteps(SetupSteps, TestStage.Setup, ctx);
            ValidateSteps(ExecutionSteps, TestStage.Execution, ctx);
            ValidateSteps(CleanupSteps, TestStage.Cleanup, ctx);
        }

        private static void ValidateSteps(IEnumerable<TestStepBase> steps, TestStage stage, Context ctx)
        {
            // Validate test Setup Steps
            foreach (var step in steps)
            {
                Exception caughtEx = null;
                try
                {
                    step.Validate(ctx);
                    if(null != step.SubSteps)
                    {
                        foreach (var subStep in step.SubSteps)
                        {
                            subStep.Validate(ctx);
                        }
                    }
                }
                catch(Exception ex)
                {
                    caughtEx = ex;
                    throw;
                }
                finally
                {
                    ctx.Logger.ValidateTestSteps(stage, step.GetType().ToString(), caughtEx);
                }
            }
        }

        ///<summary>
        /// Used to save a TestCase to disc in Xaml format
        ///</summary>
        ///<param name="testCase">The TestCase to be saved to disc</param>
        ///<param name="filePath">The file path of the Xaml test case representaiton.</param>
        public static void SaveToFile(TestCase testCase, string filePath)
        {
            BizUnitSerializationHelper.SaveToFile(testCase, filePath);
        }

        ///<summary>
        /// Used to save a TestCase to a string in Xaml format
        ///</summary>
        ///<param name="testCase">The TestCase to be saved to disc</param>
        ///<returns>The test case in Xaml format</returns>
        public static string Save(TestCase testCase)
        {
            return BizUnitSerializationHelper.Serialize(testCase);
        }

        ///<summary>
        /// Used to deserialise a Xaml test case stored on disc into a TestCase 
        ///</summary>
        ///<param name="filePath">The file path of the Xaml test case to deserialise.</param>
        ///<returns>The TestCase object</returns>
        public static TestCase LoadFromFile(string filePath)
        {
            string testCase;
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var sr = new StreamReader(fs);
                testCase = sr.ReadToEnd();
            }
            return (TestCase)BizUnitSerializationHelper.Deserialize(testCase);
        }

        ///<summary>
        /// Used to deserialise a Xaml test case into a TestCase 
        ///</summary>
        ///<param name="xamlTestCase">The Xaml test case</param>
        ///<returns>The TestCase object</returns>
        ///<exception cref="NotImplementedException"></exception>
        public static TestCase LoadXaml(string xamlTestCase)
        {
            return (TestCase)BizUnitSerializationHelper.Deserialize(xamlTestCase);
        }
    }
}
