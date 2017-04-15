
using BizUnit.Core.TestBuilder;
using BizUnit.TestSteps.Common;
using BizUnit.TestSteps.DataLoaders.File;
using BizUnit.TestSteps.File;
using BizUnit.TestSteps.ValidationSteps.Xml;
using NUnit.Framework;
using System.IO;

namespace BizUnit.TestSteps.Tests.File
{
    /// <summary>
    /// Summary description for FileCreateTests
    /// </summary>
    [TestFixture]
    public class CreateTests
    {
        [Test]
        public void CreateFileTest()
        {
            var step = new CreateStep();
            step.CreationPath = Path.Combine(TestContext.CurrentContext.TestDirectory, @"TestData\FileCreateStepTest.testdelxml");
            var dl = new FileDataLoader();
            dl.FilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, @"TestData\PurchaseOrder001.xml");
            step.DataSource = dl;
            step.Execute(new Context());

            var readStep = new FileReadMultipleStep();
            readStep.DirectoryPath = Path.Combine(TestContext.CurrentContext.TestDirectory, @"TestData\.");
            readStep.SearchPattern = "*.testdelxml";

            var validation = new XmlValidationStep();
            var schemaPurchaseOrder = new SchemaDefinition
            {
                XmlSchemaPath =
                    Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\TestData\PurchaseOrder.xsd"),
                XmlSchemaNameSpace = "http://SendMail.PurchaseOrder"
            };
            validation.XmlSchemas.Add(schemaPurchaseOrder);

            var xpathProductId = new XPathDefinition();
            xpathProductId.Description = "PONumber";
            xpathProductId.XPath = "/*[local-name()='PurchaseOrder' and namespace-uri()='http://SendMail.PurchaseOrder']/*[local-name()='PONumber' and namespace-uri()='']";
            xpathProductId.Value = "12323";
            validation.XPathValidations.Add(xpathProductId);

            readStep.SubSteps.Add(validation);

            readStep.Execute(new Context());
        }
    }
}
