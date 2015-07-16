
using BizUnit.BizUnitOM;

namespace BizUnit.Tests.ObjectModelTests
{
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Summary description for EventLogCheckStepTests
    /// </summary>
    [TestClass]
    public class EventLogCheckStepTests
    {
        [TestMethod]
        public void CheckEventLogEntry()
        {
            TestStepBuilder tsb = new TestStepBuilder("BizUnit.EventLogCheckStep", null);
            object[] args = new object[1];
            args[0] = "Application";
            tsb.SetProperty("EventLog", args);

            args = new object[1];
            args[0] = "VAA FFP";
            tsb.SetProperty("Source", args);

            args = new object[1];
            args[0] = "Error";
            tsb.SetProperty("EventType", args);

            args = new object[1];
            args[0] = 2028;
            tsb.SetProperty("EventId", args);

            args = new object[1];
            args[0] = "FieldValue: '3'";
            tsb.SetProperty("ValidationRegexs", args);

            BizUnitTestCase testCase = new BizUnitTestCase("FileCreateStepTest");
            testCase.AddTestStep(tsb, TestStage.Execution);
             
            BizUnit bizUnit = new BizUnit(testCase);
            bizUnit.RunTest();
        }

        private void WriteEventLogEntry()
        {
            // TODO: ...
        }
    }
}
