//---------------------------------------------------------------------
// File: BizUnit.cs
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
using System.Xml;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using BizUnit.BizUnitOM;
using BizUnit.Common;
using BizUnit.Xaml;

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
    ///         var bizUnit = new BizUnit(testCase);
    ///         bizUnit.RunTest();
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
    public class BizUnit
    {
        XmlNodeList _setupSteps;
        XmlNodeList _executeSteps;
        XmlNodeList _teardownSteps;
        string _testName = "Unknown";
        Exception _executionException;
        Context _context;
        internal ILogger _logger;
        readonly Queue _completedConcurrentSteps = new Queue();
        int _inflightQueueDepth;
        TestGroupPhase _testGroupPhase = TestGroupPhase.Unknown;
        private BizUnitTestCase _testCaseObjectModel;
        private TestCase _xamlTestCase;
        internal const string BizUnitTestCaseStartTime = "BizUnitTestCaseStartTime";
        private const string BizUnitTestCaseName = "BizUnitTestCaseName";

        public event EventHandler<TestStepEventArgs> TestStepStartEvent;
        public event EventHandler<TestStepEventArgs> TestStepStopEvent;
        
        /// <summary>
        /// BizUnit constructor.
        /// </summary>
        /// <param name="configFile">The path of the test case file, maybe a relavtive path.</param>
        /// 
        /// <remarks>
        /// The following example demonstrates how to create and call BizUnit:
        /// 
        /// <code escaped="true">
        ///	namespace WoodgroveBank.BVTs
        ///	{
        ///	using System;
        ///	using NUnit.Framework;
        ///	using BizUnit;
        ///
        /// // This is an example of calling BizUnit from NUnit...
        ///	[TestFixture]
        ///	public class SmokeTests
        ///	{
        ///		[Test]
        ///		public void Test_01_Adapter_MSMQ()
        ///		{
        ///			BizUnit bizUnit = new BizUnit(@".\TestCases\Test_01_Adapter_MSMQ.xml");
        ///			bizUnit.RunTest();
        ///		}
        ///	}		
        ///	</code>
        ///	
        ///	</remarks>
        [Obsolete("BizUnitTestCase has been deprecated. Please investigate the use of public BizUnit(TestCase testCase).")]
        public BizUnit(string configFile)
        {
            ArgumentValidation.CheckForNullReference(configFile, "configFile");
            LoadXmlFromFileAndInit(configFile, TestGroupPhase.Unknown, null);
        }

        /// <summary>
        /// BizUnit constructor.
        /// </summary>
        /// <param name="configFile">The path of the test case file, maybe a relavtive path.</param>
        /// <param name="ctx">The BizUnit _context object may be flowed from an previous test case.</param>
        /// 
        /// <remarks>
        /// The following example demonstrates how to create and call BizUnit. 
        /// Note: the BizUnit _context object may be created and passed into
        /// BizUnit, any _context properties set on the _context object may be
        /// used by BizUnit steps. Context properties may be of any type, i.e. any 
        /// .Net type, of course the consumer of that _context object will need to know
        /// what type to expect. 
        /// Also note that many test steps have the ability to fetch their configuration 
        /// from the BizUnit _context if their configuration is decorated with the 
        /// attribute takeFromCtx.
        /// 
        /// <code escaped="true">
        ///	namespace WoodgroveBank.BVTs
        ///	{
        ///	using System;
        ///	using NUnit.Framework;
        ///	using BizUnit;
        ///
        /// // This is an example of calling BizUnit from NUnit...
        ///	[TestFixture]
        ///	public class SmokeTests
        ///	{
        ///     AddressBook addressBook = new AddressBook("Redmond");
        /// 
        ///     Context ctx = new Context();
        ///     ctx.Add("CorrelationId", "1110023");
        ///     ctx.Add("SomeStateToFlow", "Joe.Blogs@thunderbolt.com");
        ///     ctx.Add("AddressBook", addressBook);
        /// 
        ///		[Test]
        ///		public void Test_02_Adapter_MSMQ()
        ///		{
        ///			BizUnit bizUnit = new BizUnit(@".\TestCases\Test_01_Adapter_MSMQ.xml", ctx);
        ///			bizUnit.RunTest();
        ///		}
        ///	}		
        ///	</code>
        ///	
        ///	</remarks>
        [Obsolete("BizUnitTestCase has been deprecated. Please investigate the use of public BizUnit(TestCase testCase).")]
        public BizUnit(string configFile, Context ctx)
        {
            ArgumentValidation.CheckForNullReference(configFile, "configFile");
            ArgumentValidation.CheckForNullReference(ctx, "ctx");

            _logger = ctx.Logger;
            LoadXmlFromFileAndInit(configFile, TestGroupPhase.Unknown, ctx);
        }

        /// <summary>
        /// BizUnit constructor.
        /// </summary>
        /// <param name="configStream">The path of the test case file, maybe a relavtive path.</param>
        /// 
        /// <remarks>
        /// The following example demonstrates how to create and call BizUnit:
        /// 
        /// <code escaped="true">
        ///	namespace WoodgroveBank.BVTs
        ///	{
        ///	using System;
        ///	using NUnit.Framework;
        ///	using BizUnit;
        ///
        /// // This is an example of calling BizUnit from NUnit...
        ///	[TestFixture]
        ///	public class SmokeTests
        ///	{
        ///		[Test]
        ///		public void Test_03_Adapter_MSMQ()
        ///		{
        ///         // The test case is an embeded resource...
        ///			BizUnit bizUnit = new BizUnit(Assembly.GetExecutingAssembly().GetManifestResourceStream("BizUnit.SampleTests.BizUnitFunctionalTests.Test_04_MQSeriesTest.xml"));
        ///			bizUnit.RunTest();
        ///		}
        ///	}		
        ///	</code>
        ///	
        ///	</remarks>
        [Obsolete("BizUnitTestCase has been deprecated. Please investigate the use of public BizUnit(TestCase testCase).")]
        public BizUnit(Stream configStream)
        {
            ArgumentValidation.CheckForNullReference(configStream, "configStream");

            LoadXmlFromStreamAndInit(configStream, TestGroupPhase.Unknown, null);
        }

        /// <summary>
        /// BizUnit constructor.
        /// </summary>
        /// <param name="testCase">The BizUnit test case object model that has been built to represent the test to be executed.</param>
        /// 
        /// <remarks>
        /// The following example demonstrates how to create and call BizUnit using 
        /// the BizUnit Test Case Object Model:
        /// 
        /// <code escaped="true">
        ///	namespace WoodgroveBank.BVTs
        ///	{
        ///	using System;
        ///	using NUnit.Framework;
        ///	using BizUnit;
        ///
        ///	[TestMethod]
        ///	public class SmokeTests
        ///	{
        ///		[Test]
        ///		public void Test_03_Adapter_MSMQ()
        ///		{
        ///         // The test case is an embeded resource...
        ///         BizUnitTestCase testCase = new BizUnitTestCase();
        ///
        ///         FileCreateStep fcs = new FileCreateStep();
        ///         fcs.SourcePath = @"C:\Tests\BizUnit.Tests\Data\PO_MSFT001.xml";
        ///         fcs.CreationPath = @"C:\Tests\BizUnit.Tests\Data\PO_MSFT001_%Guid%.xml";
        ///         testCase.AddTestStep(fcs, TestStage.Execution);
        ///
        ///         BizUnit bizUnit = new BizUnit(testCase);
        ///         bizUnit.RunTest();
        ///		}
        ///	}		
        ///	</code>
        ///	
        ///	</remarks>
        [Obsolete("BizUnitTestCase has been deprecated. Please investigate the use of public BizUnit(TestCase testCase).")]
        public BizUnit(BizUnitTestCase testCase)
        {
            ArgumentValidation.CheckForNullReference(testCase, "testCase");

            LoadObjectModelTestCaseAndInit(testCase, TestGroupPhase.Unknown, null);
        }

        /// <summary>
        /// BizUnit constructor.
        /// </summary>
        /// <param name="testCase">The BizUnit test case object model that has been built to represent the test to be executed.</param>
        /// <param name="ctx">The BizUnit _context object may be flowed from an previous test case.</param>
        /// 
        /// <remarks>
        /// The following example demonstrates how to create and call BizUnit using 
        /// the BizUnit Test Case Object Model:
        /// 
        /// <code escaped="true">
        ///	namespace WoodgroveBank.BVTs
        ///	{
        ///	using System;
        ///	using NUnit.Framework;
        ///	using BizUnit;
        ///
        ///	[TestMethod]
        ///	public class SmokeTests
        ///	{
        ///		[Test]
        ///		public void Test_03_Adapter_MSMQ()
        ///		{
        ///         // The test case is an embeded resource...
        ///         BizUnitTestCase testCase = new BizUnitTestCase();
        ///
        ///         Context ctx = new Context();
        ///         ctx.Add("PathToWriteFileTo", testDirectory + @"\Data_%Guid%.xml");
        /// 
        ///         FileCreateStep fcs = new FileCreateStep();
        ///         fcs.SourcePath = @"C:\Tests\BizUnit.Tests\Data\PO_MSFT001.xml";
        ///         fcs.CreationPath = "takeFromCtx:PathToWriteFileTo";
        ///         testCase.AddTestStep(fcs, TestStage.Execution);
        ///
        ///         BizUnit bizUnit = new BizUnit(testCase, ctx);
        ///         bizUnit.RunTest();
        ///		}
        ///	}		
        ///	</code>
        ///	
        ///	</remarks>
        [Obsolete("BizUnitTestCase has been deprecated. Please investigate the use of public BizUnit(TestCase testCase).")]
        public BizUnit(BizUnitTestCase testCase, Context ctx)
        {
            ArgumentValidation.CheckForNullReference(testCase, "testCase");

            _logger = ctx.Logger;
            LoadObjectModelTestCaseAndInit(testCase, TestGroupPhase.Unknown, ctx);
        }

        /// <summary>
        /// BizUnit constructor.
        /// </summary>
        /// <param name="configStream">The path of the test case file, maybe a relavtive path.</param>
        /// <param name="ctx">The BizUnit _context object may be flowed from an previous test case.</param>
        /// 
        /// <remarks>
        /// The following example demonstrates how to create and call BizUnit. 
        /// Note: the BizUnit _context object may be created and passed into
        /// BizUnit, any _context properties set on the _context object may be
        /// used by BizUnit steps. Context properties may be of any type, i.e. any 
        /// .Net type, of course the consumer of that _context object will need to know
        /// what type to expect. 
        /// Also note that many test steps have the ability to fetch their configuration 
        /// from the BizUnit _context if their configuration is decorated with the 
        /// attribute takeFromCtx.
        /// 
        /// <code escaped="true">
        ///	namespace WoodgroveBank.BVTs
        ///	{
        ///	using System;
        ///	using NUnit.Framework;
        ///	using BizUnit;
        ///
        /// // This is an example of calling BizUnit from NUnit...
        ///	[TestFixture]
        ///	public class SmokeTests
        ///	{
        ///     AddressBook addressBook = new AddressBook("Redmond");
        /// 
        ///     Context ctx = new Context();
        ///     ctx.Add("CorrelationId", "1110023");
        ///     ctx.Add("SomeStateToFlow", "Joe.Blogs@thunderbolt.com");
        ///     ctx.Add("AddressBook", addressBook);
        /// 
        ///		[Test]
        ///		public void Test_04_Adapter_MSMQ()
        ///		{
        ///         // The test case is an embeded resource...
        ///			BizUnit bizUnit = new BizUnit(Assembly.GetExecutingAssembly().GetManifestResourceStream("BizUnit.SampleTests.BizUnitFunctionalTests.Test_04_MQSeriesTest.xml"), ctx);
        ///			bizUnit.RunTest();
        ///		}
        ///	}		
        ///	</code>
        ///	
        ///	</remarks>
        [Obsolete("BizUnitTestCase has been deprecated. Please investigate the use of public BizUnit(TestCase testCase).")]
        public BizUnit(Stream configStream, Context ctx)
        {
            ArgumentValidation.CheckForNullReference(configStream, "configStream");
            ArgumentValidation.CheckForNullReference(ctx, "ctx");

            LoadXmlFromStreamAndInit(configStream, TestGroupPhase.Unknown, ctx);
        }

        /// <summary>
        /// BizUnit constructor for the setup and teardown of a test group.
        /// </summary>
        /// <param name="configFile">The path of the test case file, maybe a relavtive path.</param>
        /// <param name="testGroupPhase">The test group phase (TestGroupPhase.TestGroupSetup|TestGroupPhase.TestGroupTearDown). This 
        /// constructor is used during the initialization or termination of a group of test cases, for example when using the NUnit attributes: [TestFixtureSetUp] or [TestFixtureTearDown].</param>
        /// 
        /// <remarks>
        /// The following example demonstrates how to create and call BizUnit:
        /// 
        /// <code escaped="true">
        ///	namespace WoodgroveBank.BVTs
        ///	{
        ///	using System;
        ///	using NUnit.Framework;
        ///	using BizUnit;
        ///
        /// // This is an example of calling BizUnit from NUnit...
        ///	[TestFixture]
        ///	public class SmokeTests
        ///	{
        ///		[TestFixtureSetUp]
        ///		public void Test_Group_Setup()
        ///		{
        ///			BizUnit bizUnit = new BizUnit(@".\TestCases\Test_Group_Setup.xml", BizUnit.TestGroupPhase.TestGroupSetup);
        ///			bizUnit.RunTest();
        ///		}
        ///		
        ///		...
        ///		
        ///	}		
        ///	</code>
        ///	
        ///	</remarks>
        [Obsolete("BizUnitTestCase has been deprecated. Please investigate the use of public BizUnit(TestCase testCase).")]
        public BizUnit(string configFile, TestGroupPhase testGroupPhase)
        {
            ArgumentValidation.CheckForNullReference(configFile, "configFile");
            ArgumentValidation.CheckForNullReference(testGroupPhase, "_testGroupPhase");

            LoadXmlFromFileAndInit(configFile, testGroupPhase, null);
        }

        /// <summary>
        /// BizUnit constructor for the setup and teardown of a test group.
        /// </summary>
        /// <param name="configFile">The path of the test case file, maybe a relavtive path.</param>
        /// <param name="testGroupPhase">The test group phase (TestGroupPhase.TestGroupSetup|TestGroupPhase.TestGroupTearDown). This 
        /// constructor is used during the initialization or termination of a group of test cases, for example when using the NUnit attributes: [TestFixtureSetUp] or [TestFixtureTearDown].</param>
        /// <param name="ctx">The BizUnit _context object may be flowed from an previous test case.</param>
        /// 
        /// <remarks>
        /// The following example demonstrates how to create and call BizUnit. 
        /// Note: the BizUnit _context object may be created and passed into
        /// BizUnit, any _context properties set on the _context object may be
        /// used by BizUnit steps. Context properties may be of any type, i.e. any 
        /// .Net type, of course the consumer of that _context object will need to know
        /// what type to expect. 
        /// Also note that many test steps have the ability to fetch their configuration 
        /// from the BizUnit _context if their configuration is decorated with the 
        /// attribute takeFromCtx.
        /// 
        /// <code escaped="true">
        ///	namespace WoodgroveBank.BVTs
        ///	{
        ///	using System;
        ///	using NUnit.Framework;
        ///	using BizUnit;
        /// 
        /// // This is an example of calling BizUnit from NUnit...
        ///	[TestFixture]
        ///	public class SmokeTests
        ///	{
        ///     AddressBook addressBook = new AddressBook("Redmond");
        /// 
        ///     Context ctx = new Context();
        ///     ctx.Add("CorrelationId", "1110023");
        ///     ctx.Add("SomeStateToFlow", "Joe.Blogs@thunderbolt.com");
        ///     ctx.Add("AddressBook", addressBook);
        /// 
        ///		[TestFixtureSetUp]
        ///		public void Test_Group_Setup()
        ///		{
        ///			BizUnit bizUnit = new BizUnit(@".\TestCases\Test_Group_Setup.xml", BizUnit.TestGroupPhase.TestGroupSetup, ctx);
        ///			bizUnit.RunTest();
        ///		}
        ///		
        ///		...
        ///		
        ///	}		
        ///	</code>
        ///	
        ///	</remarks>
        [Obsolete("BizUnitTestCase has been deprecated. Please investigate the use of public BizUnit(TestCase testCase).")]
        public BizUnit(string configFile, TestGroupPhase testGroupPhase, Context ctx)
        {
            ArgumentValidation.CheckForNullReference(configFile, "configFile");
            ArgumentValidation.CheckForNullReference(testGroupPhase, "_testGroupPhase");
            ArgumentValidation.CheckForNullReference(ctx, "ctx");

            LoadXmlFromFileAndInit(configFile, testGroupPhase, ctx);
        }

        /// <summary>
        /// BizUnit constructor for the setup and teardown of a test group.
        /// </summary>
        /// <param name="configStream">The path of the test case file, maybe a relavtive path.</param>
        /// <param name="testGroupPhase">The test group phase (TestGroupPhase.TestGroupSetup|TestGroupPhase.TestGroupTearDown). This 
        /// constructor is used during the initialization or termination of a group of test cases, for example when using the NUnit attributes: [TestFixtureSetUp] or [TestFixtureTearDown].</param>
        /// 
        /// <remarks>
        /// The following example demonstrates how to create and call BizUnit:
        /// 
        /// <code escaped="true">
        ///	namespace WoodgroveBank.BVTs
        ///	{
        ///	using System;
        ///	using NUnit.Framework;
        ///	using BizUnit;
        ///
        /// // This is an example of calling BizUnit from NUnit...
        ///	[TestFixture]
        ///	public class SmokeTests
        ///	{
        ///		[TestFixtureSetUp]
        ///		public void Test_Group_Setup()
        ///		{
        ///         // The test case is an embeded resource...
        ///			BizUnit bizUnit = new BizUnit(@".\TestCases\Test_Group_Setup.xml", BizUnit.TestGroupPhase.TestGroupSetup);
        ///			bizUnit.RunTest();
        ///		}
        ///		
        ///		...
        ///		
        ///	}		
        ///	</code>
        ///	
        ///	</remarks>
        [Obsolete("BizUnitTestCase has been deprecated. Please investigate the use of public BizUnit(TestCase testCase).")]
        public BizUnit(Stream configStream, TestGroupPhase testGroupPhase)
        {
            ArgumentValidation.CheckForNullReference(configStream, "configStream");
            ArgumentValidation.CheckForNullReference(testGroupPhase, "_testGroupPhase");

            LoadXmlFromStreamAndInit(configStream, testGroupPhase, null);
        }

        /// <summary>
        /// BizUnit constructor for the setup and teardown of a test group.
        /// </summary>
        /// <param name="configStream">The path of the test case file, maybe a relavtive path.</param>
        /// <param name="testGroupPhase">The test group phase (TestGroupPhase.TestGroupSetup|TestGroupPhase.TestGroupTearDown). This 
        /// constructor is used during the initialization or termination of a group of test cases, for example when using the NUnit attributes: [TestFixtureSetUp] or [TestFixtureTearDown].</param>
        /// <param name="ctx">The BizUnit _context object may be flowed from an previous test case.</param>
        /// 
        /// <remarks>
        /// The following example demonstrates how to create and call BizUnit. 
        /// Note: the BizUnit _context object may be created and passed into
        /// BizUnit, any _context properties set on the _context object may be
        /// used by BizUnit steps. Context properties may be of any type, i.e. any 
        /// .Net type, of course the consumer of that _context object will need to know
        /// what type to expect. 
        /// Also note that many test steps have the ability to fetch their configuration 
        /// from the BizUnit _context if their configuration is decorated with the 
        /// attribute takeFromCtx.
        /// 
        /// <code escaped="true">
        ///	namespace WoodgroveBank.BVTs
        ///	{
        ///	using System;
        ///	using NUnit.Framework;
        ///	using BizUnit;
        ///
        /// // This is an example of calling BizUnit from NUnit...
        ///	[TestFixture]
        ///	public class SmokeTests
        ///	{
        ///     AddressBook addressBook = new AddressBook("Redmond");
        /// 
        ///     Context ctx = new Context();
        ///     ctx.Add("CorrelationId", "1110023");
        ///     ctx.Add("SomeStateToFlow", "Joe.Blogs@thunderbolt.com");
        ///     ctx.Add("AddressBook", addressBook);
        /// 
        ///		[TestFixtureSetUp]
        ///		public void Test_Group_Setup()
        ///		{
        ///         // The test case is an embeded resource...
        ///			BizUnit bizUnit = new BizUnit(@".\TestCases\Test_Group_Setup.xml", BizUnit.TestGroupPhase.TestGroupSetup, ctx);
        ///			bizUnit.RunTest();
        ///		}
        ///		
        ///		...
        ///		
        ///	}		
        ///	</code>
        ///	
        ///	</remarks>
        [Obsolete("BizUnitTestCase has been deprecated. Please investigate the use of public BizUnit(TestCase testCase).")]
        public BizUnit(Stream configStream, TestGroupPhase testGroupPhase, Context ctx)
        {
            ArgumentValidation.CheckForNullReference(configStream, "configStream");
            ArgumentValidation.CheckForNullReference(testGroupPhase, "_testGroupPhase");
            ArgumentValidation.CheckForNullReference(ctx, "ctx");

            LoadXmlFromStreamAndInit(configStream, testGroupPhase, ctx);
        }

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
        ///         var bu = new BizUnit(TestCase.LoadFromFile("DelayTestCaseTest.xaml"));
        ///         
        ///         // Run the test...
        ///         bu.RunTest();
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
        public BizUnit(TestCase testCase)
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
        ///         var bu = new BizUnit(TestCase.LoadFromFile("DelayTestCaseTest.xaml"));
        ///         
        ///         // Run the test...
        ///         bu.RunTest();
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
        public BizUnit(TestCase testCase, Context ctx)
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

        private void LoadObjectModelTestCaseAndInit(BizUnitTestCase testCase, TestGroupPhase phase, Context ctx)
        {
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

            _testGroupPhase = phase;
            _testName = testCase.Name;
            DateTime now = DateTime.Now;

            if (phase == TestGroupPhase.Unknown)
            {
                _logger.TestStart(_testName, now, GetUserName());                
                _context.Add(BizUnitTestCaseStartTime, now);
            }
            else
            {
                _logger.TestGroupStart(_testName, phase, now, GetUserName());
            }

            _testCaseObjectModel = testCase;
        }

        /// <summary>
        /// Gets the BizUnit _context for the current test.
        /// </summary>
        /// 
        /// <remarks>
        /// The following example demonstrates how to create and call BizUnit:
        /// 
        /// <code escaped="true">
        ///	namespace WoodgroveBank.BVTs
        ///	{
        ///	using System;
        ///	using NUnit.Framework;
        ///	using BizUnit;
        ///
        /// // This is an example of calling BizUnit from NUnit...
        ///	[TestFixture]
        ///	public class SmokeTests
        ///	{
        ///     Context ctx;
        /// 
        ///		[TestFixtureSetUp]
        ///		public void Test_Group_Setup()
        ///		{
        ///         // The test case is an embeded resource...
        ///			BizUnit bizUnit = new BizUnit(@".\TestCases\Test_Group_Setup.xml", BizUnit.TestGroupPhase.TestGroupSetup);
        ///         ctx = bizUnit.Ctx;
        /// 
        ///			bizUnit.RunTest();
        ///		}
        ///		
        ///		...
        ///		
        ///	}		
        ///	</code>
        ///	
        ///	</remarks>
        public Context Ctx
        {
            get
            {
                return _context;
            }
        }

        private void LoadXmlFromFileAndInit(
            string configFile, TestGroupPhase phase, Context ctx)
        {
            var testConfig = new XmlDocument();

            try
            {
                testConfig.Load(configFile);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.ERROR, "Failed to load the step configuration file: \"{0}\", exception: {1}", configFile, ex.ToString());
                throw;
            }

            Init(testConfig, phase, ctx);
        }

        private void LoadXmlFromStreamAndInit(
            Stream configStream, TestGroupPhase phase, Context ctx)
        {
            var testConfig = new XmlDocument();

            try
            {
                var sr = new StreamReader(configStream);
                testConfig.LoadXml(sr.ReadToEnd());
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.ERROR, "Failed to load the step configuration stream, exception: {0}", ex.ToString());
                throw;
            }

            Init(testConfig, phase, ctx);
        }

        private void Init(
            XmlDocument testConfig, TestGroupPhase phase, Context ctx)
        {
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

            _testGroupPhase = phase;

            // Get test name...
            XmlNode nameNode = testConfig.SelectSingleNode("/TestCase/@_testName");
            if (null != nameNode)
            {
                _testName = nameNode.Value;
                _context.Add(BizUnitTestCaseName, _testName, true);
            }

            DateTime now = DateTime.Now;

            if (phase == TestGroupPhase.Unknown)
            {
                _logger.TestStart(_testName, now, GetUserName());
                _context.Add(BizUnitTestCaseStartTime, now, true);
            }
            else
            {
                _logger.TestGroupStart(_testName, phase, now, GetUserName());
            }

            // Get test setup / execution / teardown steps
            _setupSteps = testConfig.SelectNodes("/TestCase/TestSetup/*");
            _executeSteps = testConfig.SelectNodes("/TestCase/TestExecution/*");
            _teardownSteps = testConfig.SelectNodes("/TestCase/TestCleanup/*");
        }

        /// <summary>
        /// Executes a test case.
        /// </summary>
        /// <returns>Returns void</returns>
        /// 
        /// <remarks>
        /// The following example demonstrates how to create and call BizUnit:
        /// 
        /// <code escaped="true">
        ///	namespace WoodgroveBank.BVTs
        ///	{
        ///	using System;
        ///	using NUnit.Framework;
        ///	using BizUnit;
        ///
        /// // This is an example of calling BizUnit from NUnit...
        ///	[TestFixture]
        ///	public class SmokeTests
        ///	{
        ///		[Test]
        ///		public void Test_01_Adapter_MSMQ()
        ///		{
        ///			BizUnit bizUnit = new BizUnit(@".\TestCases\Test_01_Adapter_MSMQ.xml");
        ///			bizUnit.RunTest();
        ///		}
        ///	}		
        ///	</code>
        ///	
        ///	</remarks>
        public void RunTest()
        {
            if(null != _xamlTestCase)
            {
                RunTestInternal(_xamlTestCase);
            }
            else
            {
                RunLegacyTestInternal();
            }
        }

        private void RunLegacyTestInternal()
        {
            try
            {
                _context.SetTestName(_testName);

                Setup();
                Execute();
                TearDown();
            }
            finally
            {
                if (null != _logger)
                {
                    _logger.Flush();
                    _logger.Close();
                }
            }

            if (null != _executionException)
            {
                throw _executionException;
            }
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
            var bu = new BizUnit(testCase, context);
            bu.RunTest();
        } 

        private void Setup()
        {
            try
            {
                _logger.TestStageStart(TestStage.Setup, DateTime.Now);
                _context.SetTestStage(TestStage.Setup);

                if (null != _testCaseObjectModel)
                {
                    ExecuteSteps(_testCaseObjectModel.SetupSteps, TestStage.Setup);
                }
                else
                {
                    ExecuteSteps(_setupSteps, TestStage.Setup);
                }
            }
            catch(Exception e)
            {
                _logger.TestStageEnd(TestStage.Setup, DateTime.Now, e);
                throw;
            }

            _logger.TestStageEnd(TestStage.Setup, DateTime.Now, null);
        }

        private void Execute()
        {
            _logger.TestStageStart(TestStage.Execution, DateTime.Now);
            _context.SetTestStage(TestStage.Execution);

            try
            {
                if (null != _testCaseObjectModel)
                {
                    ExecuteSteps(_testCaseObjectModel.ExecutionSteps, TestStage.Execution);
                }
                else
                {
                    ExecuteSteps(_executeSteps, TestStage.Execution);
                }
            }
            catch (Exception e)
            {
                // If we caught an exception on the main test execution, save it, perform cleanup,
                // then throw the exception...
                _executionException = e;
            }

            _logger.TestStageEnd(TestStage.Execution, DateTime.Now, _executionException);
        }

        private void TearDown()
        {
            _logger.TestStageStart(TestStage.Cleanup, DateTime.Now);
            _context.SetTestStage(TestStage.Cleanup);

            try
            {
                if (null != _testCaseObjectModel)
                {
                    ExecuteSteps(_testCaseObjectModel.CleanupSteps, TestStage.Cleanup);
                }
                else
                {
                    ExecuteSteps(_teardownSteps, TestStage.Cleanup);
                }

                _context.Teardown();
            }
            catch (Exception e)
            {
                _logger.TestStageEnd(TestStage.Cleanup, DateTime.Now, e);

                _logger.TestEnd(_testName, DateTime.Now, e);

                if (null != _executionException)
                {
                    throw _executionException;
                }
                else
                {
                    throw;
                }
            }

            _logger.TestStageEnd(TestStage.Cleanup, DateTime.Now, null);

            if (TestGroupPhase.Unknown != _testGroupPhase)
            {
                _logger.TestGroupEnd(_testGroupPhase, DateTime.Now, _executionException);
            }
            else
            {
                _logger.TestEnd(_testName, DateTime.Now, _executionException);
            }
        }

        private void ExecuteSteps(IEnumerable<BizUnitTestStepWrapper> steps, TestStage stage)
        {
            if (null == steps)
            {
                return;
            }

            foreach (BizUnitTestStepWrapper step in steps)
            {
                ExecuteTestStep(step, stage);
            }

            FlushConcurrentQueue(true, stage);
        }

        private void ExecuteSteps(XmlNodeList steps, TestStage stage)
        {
            if (null == steps)
            {
                return;
            }

            foreach (XmlNode stepConfig in steps)
            {
                var stepWrapper = new BizUnitTestStepWrapper(stepConfig);
                ExecuteTestStep(stepWrapper, stage);
            }

            FlushConcurrentQueue(true, stage);
        }

        private void ExecuteTestStep(BizUnitTestStepWrapper stepWrapper, TestStage stage)
        {
            try
            {
                // Should this step be executed concurrently?
                if (stepWrapper.RunConcurrently)
                {
                    _logger.TestStepStart(stepWrapper.TypeName, DateTime.Now, true, stepWrapper.FailOnError);
                    Interlocked.Increment(ref _inflightQueueDepth);
                    ThreadPool.QueueUserWorkItem(new WaitCallback(WorkerThreadThunk), new ConcurrentTestStepWrapper(stepWrapper, _context));
                }
                else
                {
                    _logger.TestStepStart(stepWrapper.TypeName, DateTime.Now, false, stepWrapper.FailOnError);
                    stepWrapper.Execute(_context);
                }
            }
            catch (Exception e)
            {
                _logger.TestStepEnd(stepWrapper.TypeName, DateTime.Now, e);

                if (stepWrapper.FailOnError)
                {
                    if (e is ValidationStepExecutionException)
                    {
                        throw;
                    }
                    else
                    {
                        var tsee = new TestStepExecutionException("BizUnit encountered an error executing a test step", e, stage, _testName, stepWrapper.TypeName);
                        throw tsee;
                    }
                }
            }

            if (!stepWrapper.RunConcurrently)
            {
                _logger.TestStepEnd(stepWrapper.TypeName, DateTime.Now, null);
            }

            FlushConcurrentQueue(false, stage);
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

        static private IValidationStep CreateValidatorStep(string typeName, string assemblyPath)
        {
            return (IValidationStep)ObjectCreator.CreateStep(typeName, assemblyPath);
        }

        static private IContextLoaderStep CreateContextLoaderStep(string typeName, string assemblyPath)
        {
            return (IContextLoaderStep)ObjectCreator.CreateStep(typeName, assemblyPath);
        }

        internal void ExecuteValidator(Stream data, IValidationStepOM validationStep, Context ctx)
        {
            if (null == validationStep)
            {
                return;
            }

            _logger.ValidatorStart(validationStep.GetType().ToString(), DateTime.Now);

            try
            {
                validationStep.ExecuteValidation(data, ctx);
            }
            catch(Exception ex)
            {
                _logger.ValidatorEnd(validationStep.GetType().ToString(), DateTime.Now, ex);

                var vsee = new ValidationStepExecutionException("BizUnit encountered an error executing a validation step", ex, _testName);
                throw vsee;
            }

            _logger.ValidatorEnd(validationStep.GetType().ToString(), DateTime.Now, null);
        }

        internal void ExecuteValidator(Stream data, XmlNode validatorConfig, Context ctx)
        {
            if (null == validatorConfig)
            {
                return;
            }

            XmlNode assemblyPath = validatorConfig.SelectSingleNode("@assemblyPath");
            XmlNode typeName = validatorConfig.SelectSingleNode("@typeName");

            _logger.ValidatorStart(typeName.Value, DateTime.Now);

            try
            {
                IValidationStep v = CreateValidatorStep(typeName.Value, assemblyPath.Value);
                v.ExecuteValidation(data, validatorConfig, ctx);
            }
            catch(Exception ex)
            {
                _logger.ValidatorEnd(typeName.Value, DateTime.Now, ex);
                var vsee = new ValidationStepExecutionException("BizUnit encountered an error executing a validation step", ex, _testName);
                throw vsee;
            }

            _logger.ValidatorEnd(typeName.Value, DateTime.Now, null);
        }

        internal void ExecuteContextLoader(Stream data, IContextLoaderStepOM contextLoaderStep, Context ctx)
        {
            if (null == contextLoaderStep)
            {
                return;
            }

            _logger.ContextLoaderStart(contextLoaderStep.GetType().ToString(), DateTime.Now);

            try
            {
                contextLoaderStep.ExecuteContextLoader(data, ctx);
            }
            catch (Exception ex)
            {
                _logger.ContextLoaderEnd(contextLoaderStep.GetType().ToString(), DateTime.Now, ex);
                throw;
            }

            _logger.ContextLoaderEnd(contextLoaderStep.GetType().ToString(), DateTime.Now, null);
        }

        internal void ExecuteContextLoader(Stream data, XmlNode contextConfig, Context ctx)
        {
            if (null == contextConfig)
            {
                return;
            }

            XmlNode assemblyPath = contextConfig.SelectSingleNode("@assemblyPath");
            XmlNode typeName = contextConfig.SelectSingleNode("@typeName");

            _logger.ContextLoaderStart(typeName.Value, DateTime.Now);

            try
            {
                IContextLoaderStep cd = CreateContextLoaderStep(typeName.Value, assemblyPath.Value);
                cd.ExecuteContextLoader(data, contextConfig, ctx);
            }
            catch(Exception ex)
            {
                _logger.ContextLoaderEnd(typeName.Value, DateTime.Now, ex);
                throw;                
            }

            _logger.ContextLoaderEnd(typeName.Value, DateTime.Now, null);
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
