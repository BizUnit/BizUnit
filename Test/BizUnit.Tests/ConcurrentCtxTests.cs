
namespace BizUnit.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class ConcurrentCtxTests
    {
        [TestMethod]
        public void SetValuesOnDefaulAndConcurrentCtxObjs()
        {
            Logger logger = new Logger();
            BizUnit bizUnit =
                new BizUnit(@"..\..\..\Test\BizUnit.Tests\Data\ConcurrentCtxTest001.xml");
            Context ctx = new Context(bizUnit, logger);

            ctx.Add("Ctx1-Val1", 1);

            Logger concurrentLogger = new Logger();
            Context ctxConcurrent = ctx.CloneForConcurrentUse(concurrentLogger);

            ctxConcurrent.Add("Ctx2-Val1", 24);
            ctx.Add("Ctx1-Val2", 2);
            ctxConcurrent.Add("Ctx2-Val2", 25);

            Assert.AreEqual((int)ctx.GetObject("Ctx1-Val1"), 1);
            Assert.AreEqual((int)ctx.GetObject("Ctx1-Val2"), 2);
            Assert.AreEqual((int)ctx.GetObject("Ctx2-Val1"), 24);
            Assert.AreEqual((int)ctx.GetObject("Ctx2-Val2"), 25);

            Assert.AreEqual((int)ctxConcurrent.GetObject("Ctx1-Val1"), 1);
            Assert.AreEqual((int)ctxConcurrent.GetObject("Ctx1-Val2"), 2);
            Assert.AreEqual((int)ctxConcurrent.GetObject("Ctx2-Val1"), 24);
            Assert.AreEqual((int)ctxConcurrent.GetObject("Ctx2-Val2"), 25);
        }

        [TestMethod]
        public void ReplaceValueOnConcurrentCtxObjs()
        {
            Logger logger = new Logger();
            BizUnit bizUnit =
                new BizUnit(@"..\..\..\Test\BizUnit.Tests\Data\ConcurrentCtxTest001.xml");
            Context ctx = new Context(bizUnit, logger);

            ctx.Add("Ctx1-Val1", 1);

            Logger concurrentLogger = new Logger();
            Context ctxConcurrent = ctx.CloneForConcurrentUse(concurrentLogger);

            ctxConcurrent.Add("Ctx1-Val1", 2, true);
            ctx.Add("Ctx1-Val1", 3, true);
            ctxConcurrent.Add("Ctx1-Val1", 4, true);

            Assert.AreEqual((int)ctx.GetObject("Ctx1-Val1"), 4);
            Assert.AreEqual((int)ctxConcurrent.GetObject("Ctx1-Val1"), 4);
        }

        [TestMethod]
        public void FlowCtxBetweenTests()
        {
            BizUnit bizUnit1 =
                new BizUnit(@"..\..\..\Test\BizUnit.Tests\Data\ConcurrentCtxTest001.xml");

            Context ctx = bizUnit1.Ctx;
            ctx.Add("SomeProperty", 32);
            string testName1 = ctx.GetValue("BizUnitTestCaseName");

            BizUnit bizUnit2 =
                new BizUnit(@"..\..\..\Test\BizUnit.Tests\Data\ConcurrentCtxTest002.xml", ctx);

            Context ctx2 = bizUnit2.Ctx;
            string testName2 = ctx2.GetValue("BizUnitTestCaseName");

            Assert.AreNotEqual(testName1, testName2);
            Assert.AreEqual(32, (Int32)ctx2.GetObject("SomeProperty"));
        }

// TODO: Test case is missing for some reason...
//        [TestMethod]
        public void FactStuff()
        {
            Object[] factObjArray = null;

            // Initialize facts...

            Context ctx = new Context();
            ctx.Add("Fact1", 32);
            ctx.Add("Fact2", factObjArray);

            BizUnit bizUnit = new BizUnit(@"..\..\..\Test\BizUnit.Tests\Data\FactBasedRuleEngineStepTest.xml", ctx);
            bizUnit.RunTest();
        }

        [TestMethod]
        public void AccessEmptyValue()
        {
            BizUnit bu = new BizUnit(@"..\..\..\Test\BizUnit.Tests\Data\DummyConfig.xml");

            Context ctx = bu.Ctx;
            object obj = ctx.GetValue("NotPresent");
            obj = ctx.GetObject("NotPresent");
        }
    }
}
