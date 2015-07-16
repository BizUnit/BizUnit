
namespace BizUnit4Tests
{
    using BizUnit;
    using BizUnitCoreTestSteps;
    using BizUnitCoreTestSteps.Common;
    using FileCreateStep = BizUnitCoreTestSteps.FileCreateStep;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Summary description for FileCreateTests
    /// </summary>
    [TestClass]
    public class FileCreateTests
    {
        public FileCreateTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void FileCreateStepTest()
        {
            var step = new FileCreateStep();
            step.CreationPath = @"..\..\..\Test\BizUnit4Tests\TestData\FileCreateStepTest.testdelxml";
            var dl = new FileDataLoader();
            dl.FilePath = @"..\..\..\Test\BizUnit4Tests\TestData\PurchaseOrder001.xml";
            step.DataSource = dl;
            step.Execute(new Context());

            var readStep = new FileReadMultipleStep();
            readStep.DirectoryPath = @"..\..\..\Test\BizUnit4Tests\TestData\.";
            readStep.SearchPattern = "*.testdelxml";

            var validation = new BizUnitCoreTestSteps.XmlValidationStep();
            var schemaPurchaseOrder = new SchemaDefinition
            {
                XmlSchemaPath =
                    @"..\..\..\Test\BizUnit4Tests\TestData\PurchaseOrder.xsd",
                XmlSchemaNameSpace =
                    "http://SendMail.PurchaseOrder"
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
