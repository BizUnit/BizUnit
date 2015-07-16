
namespace BizUnit.Tests.TestStep.Tests
{
    using System.IO;
    using System.Xml;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Summary description for XmlContextLoaderTests
    /// </summary>
    [TestClass]
    public class XmlContextLoaderTests
    {
        [TestMethod]
        public void LoadContextFromData()
        {
            Stream data = BizUnitTestUtils.GetResourceDataAsStream("Data", "XmlContextLoader-InputData.xml");
            IContextLoaderStep cl = new XmlContextLoader();
            BizUnit bu = new BizUnit(@"..\..\..\Test\BizUnit.Tests\Data\DummyConfig.xml");
            Context ctx = bu.Ctx;
            XmlNode config = BizUnitTestUtils.LoadContextLoaderStepConfig("Data", "XmlContextLoader-LoadContext.xml");
            cl.ExecuteContextLoader(data, config, ctx);

            Assert.AreEqual(ctx.GetValue("FirstName"), "John");
            Assert.AreEqual(ctx.GetValue("LastName"), "Doe");
        }
    }
}
