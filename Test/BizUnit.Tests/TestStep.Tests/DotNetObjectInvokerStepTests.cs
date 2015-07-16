

namespace BizUnit.Tests.TestStep.Tests
{
    using System.Xml;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Summary description for DotNetObjectInvokerStepTests
    /// </summary>
    [TestClass]
    public class DotNetObjectInvokerStepTests
    {
        [TestMethod]
        public void Invoke_AddNumbers()
        {
            ITestStep dnoi = new DotNetObjectInvokerStep();
            BizUnit bu = new BizUnit(@"..\..\..\Test\BizUnit.Tests\Data\DummyConfig.xml");
            Context ctx = bu.Ctx;
            XmlNode config = BizUnitTestUtils.LoadTestStepConfig("Data", "DotNetObjectInvokerStep-Test001.xml");
            dnoi.Execute(config, ctx);
        }

        [TestMethod]
        public void Invoke_FormatString()
        {
            ITestStep dnoi = new DotNetObjectInvokerStep();
            BizUnit bu = new BizUnit(@"..\..\..\Test\BizUnit.Tests\Data\DummyConfig.xml");
            Context ctx = bu.Ctx;
            XmlNode config = BizUnitTestUtils.LoadTestStepConfig("Data", "DotNetObjectInvokerStep-Test002.xml");
            dnoi.Execute(config, ctx);
        }

        [TestMethod]
        public void Invoke_DoStuff()
        {
            ITestStep dnoi = new DotNetObjectInvokerStep();
            BizUnit bu = new BizUnit(@"..\..\..\Test\BizUnit.Tests\Data\DummyConfig.xml");
            Context ctx = bu.Ctx;
            XmlNode config = BizUnitTestUtils.LoadTestStepConfig("Data", "DotNetObjectInvokerStep-Test003.xml");
            dnoi.Execute(config, ctx);
        }

        [TestMethod]
        public void Invoke_FormatStringParamFromCtx()
        {
            ITestStep dnoi = new DotNetObjectInvokerStep();
            BizUnit bu = new BizUnit(@"..\..\..\Test\BizUnit.Tests\Data\DummyConfig.xml");
            Context ctx = bu.Ctx;
            ctx.Add("NumberToPrint", "<int>2</int>");
            XmlNode config = BizUnitTestUtils.LoadTestStepConfig("Data", "DotNetObjectInvokerStep-Test004.xml");
            dnoi.Execute(config, ctx);
        }
    }
}
