
namespace BizUnit.Tests.TestStep.Tests
{
    using System.Xml;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    /// <summary>
    /// Summary description for ContextManipulatorStepTests
    /// </summary>
    [TestClass]
    public class ContextManipulatorStepTests
    {
        [TestMethod]
        public void CtxManipulationTest1()
        {
            ITestStep dnoi = new ContextManipulatorStep();
            BizUnit bu = new BizUnit(@"..\..\..\Test\BizUnit.Tests\Data\DummyConfig.xml");
            Context ctx = bu.Ctx;

            ctx.Add("HoldEvent", "Stop");
            ctx.Add("ActionId", "33");
            ctx.Add("ActionType", "Terminate");

            XmlNode config = BizUnitTestUtils.LoadTestStepConfig("Data", "ContextManipulatorStep-Test001.xml");
            dnoi.Execute(config, ctx);

            string orderItem = ctx.GetValue("OrderItem");
            Assert.AreEqual(orderItem, "holdEvent=Stop; actionId=33; actionType=Terminate;");
        }
    }
}
