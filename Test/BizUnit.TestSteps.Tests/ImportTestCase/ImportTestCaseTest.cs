
using BizUnit.TestBuilderteps.Common;
using BizUnit.TestBuilderteps.DataLoaders.File;
using BizUnit.TestBuilderteps.File;
using BizUnit.TestBuilderteps.ValidationSteps.Xml;
using BizUnit.TestBuilder;
using NUnit.Framework;
using System.IO;

namespace BizUnit.TestBuilderteps.Tests.ImportTestCase
{
    [TestFixture]
    public class ImportTestCaseTest
    {
        [Test]
        public void ImportSingleTestCaseTest()
        {
            TestHelper.DeleteFile(Path.Combine(TestContext.CurrentContext.TestDirectory, "ImportSingleTestCaseTest.xml"));

            // Create the first test case i a helper method...
            var testCase1 = BuildFirstTestCase();

            // Create the second test case and import the first test case into it...
            var testCase2 = new TestCase {Name = "Copy First File Test"};

            var createFileStep = new CreateStep {CreationPath = Path.Combine(TestContext.CurrentContext.TestDirectory, @"File2.xml") };
            var dl = new FileDataLoader
                         {
                             FilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, Path.Combine(TestContext.CurrentContext.TestDirectory, @"TestData\PurchaseOrder001.xml"))
                         };
            createFileStep.DataSource = dl;

            testCase2.ExecutionSteps.Add(createFileStep);

            var import = new ImportTestCaseStep {TestCase = testCase1};
            testCase2.ExecutionSteps.Add(import);
           
            // Create a validating read step...
            var validatingFileReadStep = new FileReadMultipleStep
                               {
                                   DirectoryPath = TestContext.CurrentContext.TestDirectory,
                                   SearchPattern = "File*.xml",
                                   ExpectedNumberOfFiles = 2
                               };

            var validation = new XmlValidationStep();
            var schemaPurchaseOrder = new SchemaDefinition
            {
                XmlSchemaPath = Path.Combine(TestContext.CurrentContext.TestDirectory, @"TestData\PurchaseOrder.xsd"),
                XmlSchemaNameSpace = "http://SendMail.PurchaseOrder"
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
            var bizUnit = new TestRunner(testCase2);
            bizUnit.Run();

            TestCase.SaveToFile(testCase2, Path.Combine(TestContext.CurrentContext.TestDirectory, "ImportSingleTestCaseTest.xml"));
        }

        private TestCase BuildFirstTestCase()
        {
            var testCase1 = new TestCase {Name = "Copy First File Test"};

            var step = new CreateStep();
            step.CreationPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "File1.xml");
            var dl = new FileDataLoader();
            dl.FilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, @"TestData\PurchaseOrder001.xml");
            step.DataSource = dl;
            step.Execute(new Context());

            testCase1.ExecutionSteps.Add(step);
            return testCase1;
        }
    }
}
