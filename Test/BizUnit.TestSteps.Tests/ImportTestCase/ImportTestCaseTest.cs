
using BizUnit.TestSteps.Common;
using BizUnit.TestSteps.DataLoaders.File;
using BizUnit.TestSteps.File;
using BizUnit.TestSteps.ValidationSteps.Xml;
using BizUnit.Xaml;
using NUnit.Framework;

namespace BizUnit.TestSteps.Tests.ImportTestCase
{
    [TestFixture]
    public class ImportTestCaseTest
    {
        [Test]
        public void ImportSingleTestCaseTest()
        {
            TestHelper.DeleteFile("ImportSingleTestCaseTest.xml");

            // Create the first test case i a helper method...
            var testCase1 = BuildFirstTestCase();

            // Create the second test case and import the first test case into it...
            var testCase2 = new TestCase {Name = "Copy First File Test"};

            var createFileStep = new CreateStep {CreationPath = @"File2.xml"};
            var dl = new FileDataLoader
                         {
                             FilePath = @"..\..\TestData\PurchaseOrder001.xml"
                         };
            createFileStep.DataSource = dl;

            testCase2.ExecutionSteps.Add(createFileStep);

            var import = new ImportTestCaseStep {TestCase = testCase1};
            testCase2.ExecutionSteps.Add(import);
           
            // Create a validating read step...
            var validatingFileReadStep = new FileReadMultipleStep
                               {
                                   DirectoryPath = @".",
                                   SearchPattern = "File*.xml",
                                   ExpectedNumberOfFiles = 2
                               };

            var validation = new XmlValidationStep();
            var schemaPurchaseOrder = new SchemaDefinition
            {
                XmlSchemaPath =
                    @"..\..\TestData\PurchaseOrder.xsd",
                XmlSchemaNameSpace =
                    "http://SendMail.PurchaseOrder"
            };
            validation.XmlSchemas.Add(schemaPurchaseOrder);

            var xpathProductId = new XPathDefinition
                                     {
                                         Description = "PONumber",
                                         XPath =
                                             "/*[local-name()='PurchaseOrder' and namespace-uri()='http://SendMail.PurchaseOrder']/*[local-name()='PONumber' and namespace-uri()='']",
                                         Value = "12323"
                                     };
            validation.XPathValidations.Add(xpathProductId);
            validatingFileReadStep.SubSteps.Add(validation);
            testCase2.ExecutionSteps.Add(validatingFileReadStep);

            // Run the second test case...
            var bizUnit = new BizUnit(testCase2);
            bizUnit.RunTest();

            TestCase.SaveToFile(testCase2, "ImportSingleTestCaseTest.xml");
        }

        private TestCase BuildFirstTestCase()
        {
            var testCase1 = new TestCase {Name = "Copy First File Test"};

            var step = new CreateStep();
            step.CreationPath = @"File1.xml";
            var dl = new FileDataLoader();
            dl.FilePath = @"..\..\TestData\PurchaseOrder001.xml";
            step.DataSource = dl;
            step.Execute(new Context());

            testCase1.ExecutionSteps.Add(step);
            return testCase1;
        }
    }
}
