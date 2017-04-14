//---------------------------------------------------------------------
// File: BizUnit.cs
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
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using BizUnit.Common;
using BizUnit.TestBuilder;
using BizUnit.Core;

namespace BizUnit
{
    /// <summary>
    /// BizUnit test framework for the rapid development of automated test cases. Test cases may be created as 'coded tests'
    /// or in XAML. 
    /// <para>
    ///	Test cases have three stages:
    ///	<para>1. TestSetup - used to setup the conditions ready to execute the test</para>
    ///	<para>2. TestExecution - the main execution stage of the test</para>
    ///	<para>3: TestCleanup - the final stage is always executed regardless of whether the test passes 
    ///	or fails in order to leave the system in the state prior to executing the test</para>
    ///	</para>
    /// 
    /// </summary>
    /// 
    /// <remarks>
    /// The following example demonstrates how to create a BizUnit coded test and execute it:
    /// 
    /// <code escaped="true">
    /// namespace WoodgroveBank.BVTs
    ///	{
    ///     using System;
    ///     using NUnit.Framework;
    ///     using BizUnit;
    ///
    ///     // This is an example of calling BizUnit from NUnit...
    ///     [TestFixture]
    ///     public class SmokeTests
    ///     {
    ///         // Create the test case
    ///         var testCase = new TestCase();
    ///     
    ///         // Create test steps...
    ///         var delayStep = new DelayStep {DelayMilliSeconds = 500};
    ///     
    ///         // Add test steps to the required test stage
    ///         testCase.ExecutionSteps.Add(delayStep);
    ///     
    ///         // Create a new instance of BizUnit and run the test
    ///         var bizUnit = new TestRunner(testCase);
    ///         bizUnit.Run();
    ///     }
    /// }		
    ///	</code>
    /// 
    /// <para>
    ///	The following XML shows the XAML for the coded test case shown above: 
    /// </para> 
    /// <code escaped="true">
    /// <TestCase 
    ///   Description="{x:Null}" 
    ///   ExpectedResults="{x:Null}" 
    ///   Name="{x:Null}" Preconditions="{x:Null}" 
    ///   Purpose="{x:Null}" Reference="{x:Null}" 
    ///   BizUnitVersion="4.0.133.0" 
    ///   xmlns="clr-namespace:BizUnit.Xaml;assembly=BizUnit" 
    ///   xmlns:btt="clr-namespace:BizUnit.TestSteps.Time;assembly=BizUnit.TestSteps" 
    ///   xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    ///   <TestCase.ExecutionSteps>
    ///     <btt:DelayStep 
    ///       SubSteps="{x:Null}" 
    ///       DelayMilliSeconds="500" 
    ///       FailOnError="True" 
    ///       RunConcurrently="False" />
    ///     </TestCase.ExecutionSteps>
    ///   </TestCase>    
    /// </code>
    /// </remarks>
    public class TestRunner
    {
        string _testName = "Unknown";
        Exception _executionException;
        Context _context;
        internal ILogger _logger;
        readonly Queue _completedConcurrentSteps = new Queue();
        int _inflightQueueDepth;
        TestGroupPhase _testGroupPhase = TestGroupPhase.Unknown;
        private TestCase _xamlTestCase;
        internal const string BizUnitTestCaseStartTime = "BizUnitTestCaseStartTime";
        private const string BizUnitTestCaseName = "BizUnitTestCaseName";

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'TestRunner.TestStepStartEvent'
        public event EventHandler<TestStepEventArgs> TestStepStartEvent;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'TestRunner.TestStepStartEvent'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'TestRunner.TestStepStopEvent'
        public event EventHandler<TestStepEventArgs> TestStepStopEvent;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'TestRunner.TestStepStopEvent'
        

        /// <summary>
        /// BizUnit constructor, introduced in BizUnit 4.0 
        /// </summary>
        /// <param name="testCase">The BizUnit test case object model that has been built to represent the test to be executed.</param>
        /// 
        /// <remarks>
        /// From BizUnit 4.0 test case maybe programatically created by creating
        /// test steps, configuring them and then adding them to a test case or 
        /// by loading Xaml test cases. Test cases developed programatically 
        /// maybe serialised to Xaml using TestCase.SaveToFile(), 
        /// similarly Xaml test cases maybe deserialised into a 
        /// TestCase using TestCase.LoadFromFile(). 
        /// 
        /// The exmaple below illustrates loading and running a Xaml test case:
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
        ///         var bu = new TestRunner(TestCase.LoadFromFile("DelayTestCaseTest.xaml"));
        ///         
        ///         // Run the test...
        ///         bu.Run();
        ///		}
        ///	}		
        ///	</code>
        ///	
        /// The exmaple below illustrates programtically creating a test case and subsequently running it:
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
        ///         var bu = new TestRunner(tc));
        ///
        ///         sw = new Stopwatch().Start();
        ///
        ///         // Run the test case...
        ///         bu.Run();
        ///
        ///         var actualDuration = sw.ElapsedMilliseconds;
        ///         Console.WriteLine("Observed delay: {0}", actualDuration);
        ///         Assert.AreEqual(actualDuration, stepDelayDuration, 20);
        ///		}
        ///	}		
        ///	</code>
        /// 
        ///	</remarks>
        public TestRunner(TestCase testCase)
        {
            ArgumentValidation.CheckForNullReference(testCase, "testCase");

            _context = new Context(this);
            _logger = _context.Logger;
            LoadXamlTestCaseAndInit(testCase, TestGroupPhase.Unknown, _context);
        }

        /// <summary>
        /// BizUnit constructor, introduced in BizUnit 4.0 
        /// </summary>
        /// <param name="testCase">The BizUnit test case object model that has been built to represent the test to be executed.</param>
        /// <param name="ctx">The BizUnit test context to be used. If this is not supplied a new contxt will created.</param>
        /// 
        /// <remarks>
        /// From BizUnit 4.0 test case maybe programatically created by creating
        /// test steps, configuring them and then adding them to a test case or 
        /// by loading Xaml test cases. Test cases developed programatically 
        /// maybe serialised to Xaml using TestCase.SaveToFile(), 
        /// similarly Xaml test cases maybe deserialised into a 
        /// TestCase using TestCase.LoadFromFile(). 
        /// 
        /// The exmaple below illustrates loading and running a Xaml test case:
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
        ///         var bu = new TestRunner(TestCase.LoadFromFile("DelayTestCaseTest.xaml"));
        ///         
        ///         // Run the test...
        ///         bu.Run();
        ///		}
        ///	}		
        ///	</code>
        ///	
        /// The exmaple below illustrates programtically creating a test case and subsequently running it:
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
        ///         var bu = new TestRunner(tc));
        ///
        ///         sw = new Stopwatch().Start();
        ///
        ///         // Run the test case...
        ///         bu.Run();
        ///
        ///         var actualDuration = sw.ElapsedMilliseconds;
        ///         Console.WriteLine("Observed delay: {0}", actualDuration);
        ///         Assert.AreEqual(actualDuration, stepDelayDuration, 20);
        ///		}
        ///	}		
        ///	</code>
        /// 
        ///	</remarks>
        public TestRunner(TestCase testCase, Context ctx)
        {
            ArgumentValidation.CheckForNullReference(testCase, "testCase");
            ArgumentValidation.CheckForNullReference(ctx, "ctx");

            _logger = ctx.Logger;
            LoadXamlTestCaseAndInit(testCase, TestGroupPhase.Unknown, ctx);
        }

        private void LoadXamlTestCaseAndInit(TestCase testCase, TestGroupPhase phase, Context ctx)
        {
            ArgumentValidation.CheckForNullReference(testCase, "testCase");
            // ctx - optional

            if (null != ctx)
            {
                _context = ctx;
                _context.Initialize(this);
            }
            else
            {
                _context = new Context(this);
                _logger = _context.Logger;
            }

            _xamlTestCase = testCase;
            _testGroupPhase = phase;
            _testName = testCase.Name;
            DateTime now = DateTime.Now;

            // Validate test case...
            testCase.Validate(_context);

            if (phase == TestGroupPhase.Unknown)
            {
                _logger.TestStart(_testName, now, GetUserName());
                _context.Add(BizUnitTestCaseStartTime, now, true);
            }
            else
            {
                _logger.TestGroupStart(testCase.Name, phase, now, GetUserName());
            }
        }

        public Context Ctx
        {
            get
            {
                return _context;
            }
        }

        public void Run()
        {
            RunTestInternal(_xamlTestCase);
        }

        private void RunTestInternal(TestCase xamlTestCase)
        {
            try
            {
                _context.SetTestName(xamlTestCase.Name);

                Setup(xamlTestCase.SetupSteps);
                Execute(xamlTestCase.ExecutionSteps);
                TearDown(xamlTestCase.CleanupSteps);

            }
            finally
            {
                if (null != _logger)
                {
                    _logger.Flush();
                    _logger.Close();
                }

                xamlTestCase.Cleanup(_context);
            }

            if (null != _executionException)
            {
                throw _executionException;
            }
        }

        private void Setup(IEnumerable<TestStepBase> testSteps)
        {
            ExecuteSteps(testSteps, TestStage.Setup);
        }

        private void Execute(IEnumerable<TestStepBase> testSteps)
        {
            ExecuteSteps(testSteps, TestStage.Execution);
        }

        private void TearDown(IEnumerable<TestStepBase> testSteps)
        {
            ExecuteSteps(testSteps, TestStage.Cleanup);
        }

        private void ExecuteSteps(IEnumerable<TestStepBase> testSteps, TestStage stage)
        {
            _logger.TestStageStart(stage, DateTime.Now);
            _context.SetTestStage(stage);

            try
            {
                if (null == testSteps)
                {
                    return;
                }

                foreach (var step in testSteps)
                {
                    ExecuteXamlTestStep(step, stage);
                }

                FlushConcurrentQueue(true, stage);
            }
            catch (Exception e)
            {
                // If we caught an exception on the main test execution, save it, perform cleanup,
                // then throw the exception...
                _executionException = e;
            }

            _logger.TestStageEnd(stage, DateTime.Now, _executionException);
        }

        private void ExecuteXamlTestStep(TestStepBase testStep, TestStage stage)
        {
            try
            {
                // Should this step be executed concurrently?
                if (testStep.RunConcurrently)
                {
                    _context.LogInfo("Queuing concurrent step: {0} for execution", testStep.GetType().ToString());
                    Interlocked.Increment(ref _inflightQueueDepth);
                    ThreadPool.QueueUserWorkItem(new WaitCallback(WorkerThreadThunk), new ConcurrentTestStepWrapper(testStep, _context));
                }
                else
                {
                    _logger.TestStepStart(testStep.GetType().ToString(), DateTime.Now, false, testStep.FailOnError);
                    if (testStep is ImportTestCaseStep)
                    {
                        ExecuteImportedTestCase(testStep as ImportTestCaseStep, _context);
                    }
                    else
                    {
                        testStep.Execute(_context);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.TestStepEnd(testStep.GetType().ToString(), DateTime.Now, e);

                if (testStep.FailOnError)
                {
                    if (e is ValidationStepExecutionException)
                    {
                        throw;
                    }

                    var tsee = new TestStepExecutionException("BizUnit encountered an error executing a test step", e, stage, _testName, testStep.GetType().ToString());
                    throw tsee;
                }
            }

            if (!testStep.RunConcurrently)
            {
                _logger.TestStepEnd(testStep.GetType().ToString(), DateTime.Now, null);
            }

            FlushConcurrentQueue(false, stage);
        }

        private static void ExecuteImportedTestCase(ImportTestCaseStep testStep, Context context)
        {
            var testCase = testStep.GetTestCase();
            var bu = new TestRunner(testCase, context);
            bu.Run();
        } 

        private void FlushConcurrentQueue(bool waitingToFinish, TestStage stage)
        {
            if (waitingToFinish && _inflightQueueDepth == 0)
            {
                return;
            }

            while ((_completedConcurrentSteps.Count > 0) || waitingToFinish)
            {
                object obj = null;

                lock (_completedConcurrentSteps.SyncRoot)
                {
                    if (_completedConcurrentSteps.Count > 0)
                    {
                        try
                        {
                            obj = _completedConcurrentSteps.Dequeue();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogException(ex);
                        }
                    }
                }

                if (null != obj)
                {
                    var step = (ConcurrentTestStepWrapper)obj;
                    _logger.LogBufferedText(step.Logger);

                    _logger.TestStepEnd(step.Name, DateTime.Now, step.FailureException);

                    // Check to see if the test step failed, if it did throw the exception...
                    if (null != step.FailureException)
                    {
                        Interlocked.Decrement(ref _inflightQueueDepth);

                        if (step.FailOnError)
                        {
                            if (step.FailureException is ValidationStepExecutionException)
                            {
                                throw step.FailureException;
                            }
                            else
                            {
                                var tsee = new TestStepExecutionException("BizUnit encountered an error concurrently executing a test step", step.FailureException, stage, _testName, step.StepName);
                                throw tsee;
                            }
                        }
                    }
                    else
                    {
                        Interlocked.Decrement(ref _inflightQueueDepth);
                    }
                }

                if (waitingToFinish && (_inflightQueueDepth > 0))
                {
                    Thread.Sleep(250);
                }
                else if (waitingToFinish && (_inflightQueueDepth == 0))
                {
                    break;
                }
            }
        }

        private void WorkerThreadThunk(Object stateInfo)
        {
            var step = (ConcurrentTestStepWrapper)stateInfo;
            step.Execute();

            // This step is completed, add to queue
            lock (_completedConcurrentSteps.SyncRoot)
            {
                _completedConcurrentSteps.Enqueue(step);
            }
        }

        internal static string GetNow()
        {
            return DateTime.Now.ToString("HH:mm:ss.fff dd/MM/yyyy");
        }

        internal static string GetUserName()
        {
            string usersDomain = Environment.UserDomainName;
            string usersName = Environment.UserName;

            return usersDomain + "\\" + usersName;
        }

        internal void OnTestStepStart(TestStepEventArgs e)
        {
            if(null != TestStepStartEvent)
            {                
                EventHandler<TestStepEventArgs> testStepStartEvent = TestStepStartEvent;
                testStepStartEvent(this, e);
            }
        }

        internal void OnTestStepStop(TestStepEventArgs e)
        {
            if (null != TestStepStopEvent)
            {
                EventHandler<TestStepEventArgs> testStepStopEvent = TestStepStopEvent;
                testStepStopEvent(this, e);
            }
        }
    }
}
