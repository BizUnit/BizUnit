
namespace BizUnit.Tests
{
    using System;
    using Rhino.Mocks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Summary description for ILoggerTests
    /// </summary>
    [TestClass]
    public class ILoggerTests
    {
        [TestMethod]
        public void Test_02_FILECopyWithBinaryValidation_MockLogger()
        {
            MockRepository mocks = new MockRepository();

            ILogger logger = mocks.CreateMock<ILogger>();

            logger.TestStart(null, DateTime.Now, null);            
            LastCall.On(logger).Repeat.Once().IgnoreArguments();

            // S E T U P
            logger.TestStageStart(TestStage.Setup, DateTime.Now);
            LastCall.On(logger).Repeat.Once().IgnoreArguments();

            logger.TestStageEnd(TestStage.Setup, DateTime.Now, null);
            LastCall.On(logger).Repeat.Once().IgnoreArguments();

            // E X E C U T E 
            logger.TestStageStart(TestStage.Execution, DateTime.Now);
            LastCall.On(logger).Repeat.Once().IgnoreArguments();

            logger.TestStepStart(null, DateTime.Now, true, true);
            LastCall.On(logger).Repeat.Once().IgnoreArguments();

            logger.TestStepEnd(null, DateTime.Now, null);
            LastCall.On(logger).Repeat.Once().IgnoreArguments();
            
            logger.TestStepStart(null, DateTime.Now, true, true);
            LastCall.On(logger).Repeat.Once().IgnoreArguments();

            logger.Log(LogLevel.INFO, null);
            LastCall.On(logger).Repeat.Any().IgnoreArguments();

            logger.LogData(null, null);
            LastCall.On(logger).Repeat.Once().IgnoreArguments();

            logger.ValidatorStart(null, DateTime.Now);
            LastCall.On(logger).Repeat.Once().IgnoreArguments();

            logger.Log(LogLevel.INFO, null);
            LastCall.On(logger).Repeat.Any().IgnoreArguments();

            logger.ValidatorEnd(null, DateTime.Now, null);
            LastCall.On(logger).Repeat.Once().IgnoreArguments();

            logger.TestStepEnd(null, DateTime.Now, null);
            LastCall.On(logger).Repeat.Once().IgnoreArguments();

            logger.TestStageEnd(TestStage.Execution, DateTime.Now, null);
            LastCall.On(logger).Repeat.Once().IgnoreArguments();

            // C L E A N U P
            logger.TestStageStart(TestStage.Cleanup, DateTime.Now);
            LastCall.On(logger).Repeat.Once().IgnoreArguments();

            logger.TestStepStart(null, DateTime.Now, true, true);
            LastCall.On(logger).Repeat.Once().IgnoreArguments();

            logger.TestStepEnd(null, DateTime.Now, null);
            LastCall.On(logger).Repeat.Once().IgnoreArguments();

            logger.TestStageEnd(TestStage.Cleanup, DateTime.Now, null);
            LastCall.On(logger).Repeat.Once().IgnoreArguments();

            logger.TestEnd(null, DateTime.Now, null);
            LastCall.On(logger).Repeat.Once().IgnoreArguments();

            logger.Flush();
            logger.Close();

            mocks.ReplayAll();

            Context ctx = new Context(logger);
            BizUnit bizUnit = new BizUnit(@"..\..\..\Test\BizUnit.Tests\TestCases\Test_02_FILECopyWithBinaryValidation.xml", ctx);
            bizUnit.RunTest();

            mocks.VerifyAll();
        }
        
        [TestMethod]
        public void Test_04_NegativeTest_MockLogger()
        {
            MockRepository mocks = new MockRepository();

            ILogger logger = mocks.CreateMock<ILogger>();

            logger.TestStart(null, DateTime.Now, null);            
            LastCall.On(logger).Repeat.Once().IgnoreArguments();

            // S E T U P
            logger.TestStageStart(TestStage.Setup, DateTime.Now);
            LastCall.On(logger).Repeat.Once().IgnoreArguments();

            logger.TestStageEnd(TestStage.Setup, DateTime.Now, null);
            LastCall.On(logger).Repeat.Once().IgnoreArguments();

            // E X E C U T E 
            logger.TestStageStart(TestStage.Execution, DateTime.Now);
            LastCall.On(logger).Repeat.Once().IgnoreArguments();

            logger.TestStepStart(null, DateTime.Now, true, true);
            LastCall.On(logger).Repeat.Once().IgnoreArguments();

            logger.TestStepEnd(null, DateTime.Now, null);
            LastCall.On(logger).Repeat.Once().IgnoreArguments();
            
            logger.TestStepStart(null, DateTime.Now, true, true);
            LastCall.On(logger).Repeat.Once().IgnoreArguments();

            logger.Log(LogLevel.INFO, null);
            LastCall.On(logger).Repeat.Any().IgnoreArguments();

            logger.LogData(null, null);
            LastCall.On(logger).Repeat.Once().IgnoreArguments();

            logger.ValidatorStart(null, DateTime.Now);
            LastCall.On(logger).Repeat.Once().IgnoreArguments();

            logger.Log(LogLevel.INFO, null);
            LastCall.On(logger).Repeat.Any().IgnoreArguments();

            logger.Log(LogLevel.ERROR, null, null);
            LastCall.On(logger).Repeat.Once().IgnoreArguments();

            logger.LogException(null);
            LastCall.On(logger).Repeat.Once().IgnoreArguments();

            logger.ValidatorEnd(null, DateTime.Now, null);
            LastCall.On(logger).Repeat.Once().IgnoreArguments();

            logger.TestStepEnd(null, DateTime.Now, null);
            LastCall.On(logger).Repeat.Once().IgnoreArguments();

            logger.TestStageEnd(TestStage.Execution, DateTime.Now, null);
            LastCall.On(logger).Repeat.Once().IgnoreArguments();

            // C L E A N U P
            logger.TestStageStart(TestStage.Cleanup, DateTime.Now);
            LastCall.On(logger).Repeat.Once().IgnoreArguments();

            logger.TestStepStart(null, DateTime.Now, true, true);
            LastCall.On(logger).Repeat.Once().IgnoreArguments();

            logger.TestStepEnd(null, DateTime.Now, null);
            LastCall.On(logger).Repeat.Once().IgnoreArguments();

            logger.TestStageEnd(TestStage.Cleanup, DateTime.Now, null);
            LastCall.On(logger).Repeat.Once().IgnoreArguments();

            logger.TestEnd(null, DateTime.Now, null);
            LastCall.On(logger).Repeat.Once().IgnoreArguments();

            logger.Flush();
            logger.Close();

            mocks.ReplayAll();

            Context ctx = new Context(logger);
            BizUnit bizUnit = new BizUnit(@"..\..\..\Test\BizUnit.Tests\TestCases\Test_04_NegativeTest.xml", ctx);

            try
            {
                bizUnit.RunTest();
            }
            catch(Exception) {}

            mocks.VerifyAll();
        }
    }
}
