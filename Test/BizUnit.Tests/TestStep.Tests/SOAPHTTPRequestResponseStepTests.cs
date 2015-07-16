
namespace BizUnit.Tests.TestStep.Tests
{
    using System.Xml;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Summary description for SOAPHTTPRequestResponseStepTests
    /// </summary>
    [TestClass]
    public class SOAPHTTPRequestResponseStepTests
    {
        [TestMethod]
        public void CallVoidMethod()
        {
            ITestStep dnoi = new SOAPHTTPRequestResponseStep();
            BizUnit bu = new BizUnit(@"..\..\..\Test\BizUnit.Tests\Data\DummyConfig.xml");
            Context ctx = bu.Ctx;
            XmlNode config = BizUnitTestUtils.LoadTestStepConfig("Data", "SOAPHTTPRequestResponseStep-Test001.xml");
            dnoi.Execute(config, ctx);
        }

        [TestMethod]
        public void GetStockQuoteTest()
        {
            ITestStep dnoi = new SOAPHTTPRequestResponseStep();
            BizUnit bu = new BizUnit(@"..\..\..\Test\BizUnit.Tests\Data\DummyConfig.xml");
            Context ctx = bu.Ctx;
            XmlNode config = BizUnitTestUtils.LoadTestStepConfig("Data", "SOAPHTTPRequestResponseStep-Test002.xml");
            dnoi.Execute(config, ctx);
        }
    }
}
