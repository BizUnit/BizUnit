
using BizUnit.BizTalkSteps;

namespace BizUnit.Tests.TestStep.Tests
{
    using System.Threading;
    using System.Xml;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class HostConductorStepTests
    {
        [TestMethod]
        public void StartThenStopWithCreds()
        {
            ExecuteStartOrStop("HostConductorStep-StartUsingCreds.xml");
            Thread.Sleep(100);
            ExecuteStartOrStop("HostConductorStep-StopUsingCreds.xml");
        }

        [TestMethod]
        public void StartThenStopWithOutCreds()
        {
            ExecuteStartOrStop("HostConductorStep-StartNoCreds.xml");
            Thread.Sleep(100); 
            ExecuteStartOrStop("HostConductorStep-StopNoCreds.xml");
        }

        private static void ExecuteStartOrStop(string testScript)
        {
            ITestStep hcs = new HostConductorStep();
            BizUnit bu = new BizUnit(@"..\..\..\Test\BizUnit.Tests\Data\DummyConfig.xml");
            Context ctx = bu.Ctx;
            XmlNode config = BizUnitTestUtils.LoadTestStepConfig("Data", testScript);
            hcs.Execute(config, ctx);
        }
    }
}
