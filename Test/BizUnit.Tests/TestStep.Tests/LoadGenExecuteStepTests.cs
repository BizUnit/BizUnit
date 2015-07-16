
namespace BizUnit.Tests.TestStep.Tests
{
    using System.Xml;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using LoadGenSteps;

    /// <summary>
    /// Summary description for LoadGenExecuteStepTests
    /// </summary>
    [TestClass]
    public class LoadGenExecuteStepTests
    {
        [TestMethod]
        public void LoadGenStepTest()
        {
            ITestStep dnoi = new LoadGenExecuteStep();
            BizUnit bu = new BizUnit(@"..\..\..\Test\BizUnit.Tests\Data\DummyConfig.xml");
            Context ctx = bu.Ctx;
            XmlNode config = BizUnitTestUtils.LoadTestStepConfig("Data", "LoadGenExecuteStep-Test001.xml");
            dnoi.Execute(config, ctx);
        }
    }
}
