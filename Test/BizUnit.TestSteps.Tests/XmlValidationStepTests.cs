
using System;
using BizUnit.TestSteps.ValidationSteps.Xml;
using BizUnit.TestSteps.Common;
using NUnit.Framework;

namespace BizUnit.TestSteps.Tests
{
    /// <summary>
    /// Summary description for XmlValidationStepTests
    /// </summary>
    [TestFixture]
    public class XmlValidationStepTests
    {
        [Test]
        public void XmlValidationStepTest()
        {
            var validation = new XmlValidationStep();
            var schemaPurchaseOrder = new SchemaDefinition
            {
                XmlSchemaPath =
                    @"..\..\TestData\PurchaseOrder.xsd",
                XmlSchemaNameSpace =
                    "http://SendMail.PurchaseOrder"
            };
            validation.XmlSchemas.Add(schemaPurchaseOrder);

            var xpathProductId = new XPathDefinition();
            xpathProductId.Description = "PONumber";
            xpathProductId.XPath = "/*[local-name()='PurchaseOrder' and namespace-uri()='http://SendMail.PurchaseOrder']/*[local-name()='PONumber' and namespace-uri()='']";
            xpathProductId.Value = "12323";
            validation.XPathValidations.Add(xpathProductId);

            var ctx = new Context();
            var data = StreamHelper.LoadFileToStream(@"..\..\TestData\PurchaseOrder001.xml");
            validation.Execute(data, ctx);
        }

        [Test]
        public void XmlValidationStepTest_InvalidXPath()
        {
            var validation = new XmlValidationStep();
            var schemaPurchaseOrder = new SchemaDefinition
            {
                XmlSchemaPath =
                    @"..\..\TestData\PurchaseOrder.xsd",
                XmlSchemaNameSpace =
                    "http://SendMail.PurchaseOrder"
            };
            validation.XmlSchemas.Add(schemaPurchaseOrder);

            var xpathProductId = new XPathDefinition();
            xpathProductId.Description = "PONumber";
            xpathProductId.XPath = "/*[local-name()='PurchaseOrder' and namespace-uri()='http://SendMail.PurchaseOrder']/*[local-name()='PONumber' and namespace-uri()='']";
            xpathProductId.Value = "12323";
            validation.XPathValidations.Add(xpathProductId);

            var ctx = new Context();
            var data = StreamHelper.LoadFileToStream(@"..\..\TestData\PurchaseOrder002_BadXPath.xml");
            try
            {
                Assert.Throws<ApplicationException>(() => validation.Execute(data, ctx));
            }
            catch (ApplicationException aex)
            {
                Assert.AreEqual(
                    @"XmlValidationStep failed, compare 12323 != BADBAD, xpath query used: /*[local-name()='PurchaseOrder' and namespace-uri()='http://SendMail.PurchaseOrder']/*[local-name()='PONumber' and namespace-uri()='']",
                    aex.Message);
            }
        }

        [Test]
        public void XmlValidationStepTest_SchemaValidationFail()
        {
            var validation = new XmlValidationStep();
            var schemaPurchaseOrder = new SchemaDefinition
            {
                XmlSchemaPath =
                    @"..\..\TestData\PurchaseOrder.xsd",
                XmlSchemaNameSpace =
                    "http://SendMail.PurchaseOrder"
            };
            validation.XmlSchemas.Add(schemaPurchaseOrder);

            var xpathProductId = new XPathDefinition();
            xpathProductId.Description = "PONumber";
            xpathProductId.XPath = "/*[local-name()='PurchaseOrder' and namespace-uri()='http://SendMail.PurchaseOrder']/*[local-name()='PONumber' and namespace-uri()='']";
            xpathProductId.Value = "12323";
            validation.XPathValidations.Add(xpathProductId);

            var ctx = new Context();
            var data = StreamHelper.LoadFileToStream(@"..\..\TestData\PurchaseOrder003_SchemaValidationFail.xml");
            try
            {
                Assert.Throws<ValidationStepExecutionException>(() => validation.Execute(data, ctx));
            }
            catch (ValidationStepExecutionException vsee)
            {
                Assert.AreEqual("Failed to validate document instance", vsee.Message);
                Assert.AreEqual(
                    @"The 'http://SendMail.PurchaseOrder:PurchaseOrderBAD' element is not declared.", 
                    vsee.InnerException.Message);
            }
        }

        [Test]
        public void XmlValidationStepTest_SchemaValidationFailMissingElem()
        {
            var validation = new XmlValidationStep();
            var schemaPurchaseOrder = new SchemaDefinition
            {
                XmlSchemaPath =
                    @"..\..\TestData\PurchaseOrder.xsd",
                XmlSchemaNameSpace =
                    "http://SendMail.PurchaseOrder"
            };
            validation.XmlSchemas.Add(schemaPurchaseOrder);

            var xpathProductId = new XPathDefinition();
            xpathProductId.Description = "PONumber";
            xpathProductId.XPath = "/*[local-name()='PurchaseOrder' and namespace-uri()='http://SendMail.PurchaseOrder']/*[local-name()='PONumber' and namespace-uri()='']";
            xpathProductId.Value = "12323";
            validation.XPathValidations.Add(xpathProductId);

            var ctx = new Context();
            var data = StreamHelper.LoadFileToStream(@"..\..\TestData\PurchaseOrder004_SchemaValidationFailMissingElem.xml");
            try
            {
                Assert.Throws<ValidationStepExecutionException>(() => validation.Execute(data, ctx));
            }
            catch (ValidationStepExecutionException vsee)
            {
                Assert.AreEqual("Failed to validate document instance", vsee.Message);
                Assert.AreEqual(
                    @"The element 'PurchaseOrder' in namespace 'http://SendMail.PurchaseOrder' has invalid child element 'Description'. List of possible elements expected: 'CustomerInfo'.", 
                    vsee.InnerException.Message);
            }
        }
    }
}
