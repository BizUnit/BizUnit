
namespace BizUnitTests
{
    using BizUnit;
    using BizUnit.BizUnitOM;
    using BizUnit.Tests;
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Summary description for ContextLoaderStepBuilderTests
    /// </summary>
    [TestClass]
    public class ContextLoaderStepBuilderTests 
    {
        [TestMethod]
        public void XmlContextLoader_Positive()
        {
            ContextLoaderStepBuilder clsb = new ContextLoaderStepBuilder("BizUnit.XmlContextLoader", null);
            object[] args = new object[2];
            args[0] = "MyContextKey";
            args[1] = "*[local-name()='PurchaseOrder' and namespace-uri()='http://SendMail.PurchaseOrder']/*[local-name()='PONumber' and namespace-uri()='']";
            clsb.SetProperty("XPathExpressions", args);

            Context ctx = new Context();
            Stream data = ResourceLoaderHelper.GetResourceDataAsStream("Data", "PurchaseOrder001.xml");

            clsb.ContextLoaderStep.Validate(ctx);
            clsb.ContextLoaderStep.ExecuteContextLoader(data, ctx);

            Assert.AreEqual("PONumber_0", ctx.GetValue("MyContextKey"));
        }
    }
}
