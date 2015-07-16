using System;
using System.Collections;
using BizUnit;
using BizUnit.BizUnitOM;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace BizUnitTests
{
    [TestClass]
    public class ContextDisposableTests
    {
        private MockRepository mockery;

        public ContextDisposableTests()
        {
            this.mockery = new MockRepository();
        }

        [TestMethod]
        public void TestDispose()
        {
            Context context = new Context();
            context.DisposeMembersOnTestCaseCompletion = true;
            IDisposable disposable = mockery.DynamicMock<IDisposable>();
            IEnumerable notDisposable = mockery.DynamicMock<IEnumerable>();

            context.Add("DisposableClassKey", disposable);
            context.Add("NotDisposableClassKey", notDisposable);

            using(mockery.Record())
            {
                disposable.Dispose();
                LastCall.Repeat.Once();
            }

            using(mockery.Playback())
            {
                BizUnit.BizUnit bizUnit = new BizUnit.BizUnit(new BizUnitTestCase("TestDispose"), context);
                bizUnit.RunTest();
            }
        }

        [TestMethod, ExpectedException(typeof(ApplicationException))]
        public void TestDisposeWithException()
        {
            Context context = new Context();
            context.DisposeMembersOnTestCaseCompletion = true;
            IDisposable disposable = mockery.DynamicMock<IDisposable>();
            IEnumerable notDisposable = mockery.DynamicMock<IEnumerable>();

            context.Add("DisposableClassKey", disposable);
            context.Add("NotDisposableClassKey", notDisposable);

            using (mockery.Record())
            {
                disposable.Dispose();
                LastCall.Throw(new ApplicationException("Exception thrown during dispose.")).Repeat.Once();
            }

            using (mockery.Playback())
            {
                BizUnit.BizUnit bizUnit = new BizUnit.BizUnit(new BizUnitTestCase("TestDisposeWithException"), context);
                bizUnit.RunTest();
            }
        }

        [TestMethod]
        public void TestDisposeWithNoDisposables()
        {
            Context context = new Context();
            context.DisposeMembersOnTestCaseCompletion = true;
            IEnumerable notDisposable = mockery.DynamicMock<IEnumerable>();

            using (mockery.Record())
            {
                context.Add("NotDisposableClassKey", notDisposable);
            }

            using (mockery.Playback())
            {
                BizUnit.BizUnit bizUnit = new BizUnit.BizUnit(new BizUnitTestCase("TestDisposeWithNoDisposables"), context);
                bizUnit.RunTest();
            }
        }

        [TestMethod]
        public void TestNotDispose()
        {
            Context context = new Context();
            context.DisposeMembersOnTestCaseCompletion = false;
            IDisposable disposable = mockery.DynamicMock<IDisposable>();

            context.Add("DisposableClassKey", disposable);

            using (mockery.Record())
            {
                disposable.Dispose();
                LastCall.Repeat.Never();
            }

            using (mockery.Playback())
            {
                BizUnit.BizUnit bizUnit = new BizUnit.BizUnit(new BizUnitTestCase("TestNotDispose"), context);
                bizUnit.RunTest();
            }
        }

        [TestMethod]
        public void TestDefaultNoDispose()
        {
            Context context = new Context();
            IDisposable disposable = mockery.DynamicMock<IDisposable>();

            context.Add("DisposableClassKey", disposable);

            using (mockery.Record())
            {
                disposable.Dispose();
                LastCall.Repeat.Never();
            }

            using (mockery.Playback())
            {
                BizUnit.BizUnit bizUnit = new BizUnit.BizUnit(new BizUnitTestCase("TestDefaultNoDispose"), context);
                bizUnit.RunTest();
            }
        }

        [TestMethod]
        public void TestDisposeWithSteps()
        {
            Context context = new Context();
            context.DisposeMembersOnTestCaseCompletion = true;
            IDisposable disposable = mockery.DynamicMock<IDisposable>();
            IEnumerable notDisposable = mockery.DynamicMock<IEnumerable>();

            context.Add("DisposableClassKey", disposable);
            context.Add("NotDisposableClassKey", notDisposable);

            BizUnitTestCase testCase = new BizUnitTestCase("TestDisposeWithSteps");
            ITestStepOM mockStep1 = mockery.DynamicMock<ITestStepOM>();
            testCase.AddTestStep(mockStep1, TestStage.Setup);
            ITestStepOM mockStep2 = mockery.DynamicMock<ITestStepOM>();
            testCase.AddTestStep(mockStep2, TestStage.Execution);
            ITestStepOM mockStep3 = mockery.DynamicMock<ITestStepOM>();
            testCase.AddTestStep(mockStep3, TestStage.Cleanup);

            using (mockery.Record())
            {
                mockStep1.Execute(context);
                LastCall.Repeat.Once();

                mockStep2.Execute(context);
                LastCall.Repeat.Once();

                mockStep3.Execute(context);
                LastCall.Repeat.Once();

                disposable.Dispose();
                LastCall.Repeat.Once();
            }

            using (mockery.Playback())
            {
                BizUnit.BizUnit bizUnit = new BizUnit.BizUnit(testCase, context);
                bizUnit.RunTest();
            }
        }
    }
}
