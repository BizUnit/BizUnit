
namespace BizUnit.Tests.ObjectModelTests
{
    using System.Reflection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using BizUnitOM;

    /// <summary>
    /// Summary description for TestStepBuilderTests
    /// </summary>
    [TestClass]
    public class TestStepBuilderTests
    {
        [TestMethod]
        public void Create_FileCreateStep_AndSetSimpleTypes()
        {
            TestStepBuilder tsb = new TestStepBuilder("BizUnit.FileCreateStep", null);
            object[] args = new object[1];
            args[0] = @"..\..\..\Test\BizUnit.Tests\Data\LoadGenScript001.xml";
            tsb.SetProperty("SourcePath", args);

            args = new object[1];
            args[0] = @"..\..\..\Test\BizUnit.Tests\Out\Data_%Guid%.xml";
            tsb.SetProperty("CreationPath", args);

            string testDirectory = @"..\..\..\Test\BizUnit.Tests\Out";
            FileHelper.EmptyDirectory(testDirectory, "*.xml");

            Assert.AreEqual(FileHelper.NumberOfFilesInDirectory(testDirectory, "*.xml"), 0);

            BizUnitTestCase testCase = new BizUnitTestCase("Create_FileCreateStep_AndSetSimpleTypes");

            // Add the test step builder to the test case...
            testCase.AddTestStep(tsb, TestStage.Execution);

            BizUnit bizUnit = new BizUnit(testCase);
            bizUnit.RunTest();

            Assert.AreEqual(FileHelper.NumberOfFilesInDirectory(testDirectory, "*.xml"), 1);
        }


        [TestMethod]
        public void Create_FileCreateStep_TakeFromCtx()
        {
            Context ctx = new Context();
            ctx.Add("PathToWriteFileTo", @"..\..\..\Test\BizUnit.Tests\Out\Data_%Guid%.xml");

            TestStepBuilder tsb = new TestStepBuilder("BizUnit.FileCreateStep", null);
            object[] args = new object[1];
            args[0] = @"..\..\..\Test\BizUnit.Tests\Data\LoadGenScript001.xml";
            tsb.SetProperty("SourcePath", args);

            args = new object[1];
            args[0] = "takeFromCtx:PathToWriteFileTo";
            tsb.SetProperty("CreationPath", args);

            string testDirectory = @"..\..\..\Test\BizUnit.Tests\Out";
            FileHelper.EmptyDirectory(testDirectory, "*.xml");

            Assert.AreEqual(FileHelper.NumberOfFilesInDirectory(testDirectory, "*.xml"), 0);

            BizUnitTestCase testCase = new BizUnitTestCase("Create_FileCreateStep_TakeFromCtx");

            // Add the test step builder to the test case...
            testCase.AddTestStep(tsb, TestStage.Execution);

            BizUnit bizUnit = new BizUnit(testCase, ctx);
            bizUnit.RunTest();

            Assert.AreEqual(FileHelper.NumberOfFilesInDirectory(testDirectory, "*.xml"), 1);
        }
        
        [TestMethod]
        public void Create_FileDeleteStep_AndSetstringArray()
        {
            BizUnitTestCase testCase = new BizUnitTestCase("Create_FileDeleteStep_AndSetstringArray");

            // Create a file in the output dir...
            TestStepBuilder tsb1 = new TestStepBuilder("BizUnit.FileCreateStep", null);
            object[] args = new object[1];
            args[0] = @"..\..\..\Test\BizUnit.Tests\Data\LoadGenScript001.xml";
            tsb1.SetProperty("SourcePath", args);

            args = new object[1];
            args[0] = @"..\..\..\Test\BizUnit.Tests\Out\Data_File1.xml";
            tsb1.SetProperty("CreationPath", args);

            // Add the test step builder to the test case...
            testCase.AddTestStep(tsb1, TestStage.Execution);

            // Create a file in the output dir...
            TestStepBuilder tsb2 = new TestStepBuilder("BizUnit.FileCreateStep", null);
            args = new object[1];
            args[0] = @"..\..\..\Test\BizUnit.Tests\Data\LoadGenScript001.xml";
            tsb2.SetProperty("SourcePath", args);

            args = new object[1];
            args[0] = @"..\..\..\Test\BizUnit.Tests\Out\Data_File2.xml";
            tsb2.SetProperty("CreationPath", args);

            // Add the test step builder to the test case...
            testCase.AddTestStep(tsb2, TestStage.Execution);

            TestStepBuilder tsb3 = new TestStepBuilder("BizUnit.FileDeleteStep", null);
            args = new object[2];
            args[0] = @"..\..\..\Test\BizUnit.Tests\Out\Data_File1.xml";
            args[1] = @"..\..\..\Test\BizUnit.Tests\Out\Data_File2.xml";
            tsb3.SetProperty("FilesToDeletePath", args);

            string testDirectory = @"..\..\..\Test\BizUnit.Tests\Out";
            // Add the test step builder to the test case...
            testCase.AddTestStep(tsb3, TestStage.Execution);

            FileHelper.EmptyDirectory(testDirectory, "*.xml");

            Assert.AreEqual(FileHelper.NumberOfFilesInDirectory(testDirectory, "*.xml"), 0);

            BizUnit bizUnit = new BizUnit(testCase);
            bizUnit.RunTest();

            Assert.AreEqual(FileHelper.NumberOfFilesInDirectory(testDirectory, "*.xml"), 0);
        }

        [TestMethod]
        public void Create_FileValidateStep_SetPropsAndValidation()
        {
            BizUnitTestCase testCase = new BizUnitTestCase("Create_FileValidateStep_SetPropsAndValidation");

            // create a file...
            TestStepBuilder createFileStep = new TestStepBuilder("BizUnit.FileCreateStep", null);
            object[] args = new object[1];
            args[0] = @"..\..\..\Test\BizUnit.Tests\Data\PurchaseOrder001.xml";
            createFileStep.SetProperty("SourcePath", args);

            args = new object[1];
            args[0] = @"..\..\..\Test\BizUnit.Tests\Out\Data_%Guid%.xml";
            createFileStep.SetProperty("CreationPath", args);

            // Read and validate file...
            TestStepBuilder tsb = new TestStepBuilder("BizUnit.FileValidateStep", null);
            args = new object[1];
            args[0] = "1000";
            tsb.SetProperty("Timeout", args);

            args[0] = @"..\..\..\Test\BizUnit.Tests\Out";
            tsb.SetProperty("Directory", args);

            args[0] = "*.*";
            tsb.SetProperty("SearchPattern", args);

            args[0] = "true";
            tsb.SetProperty("DeleteFile", args);

            ValidationStepBuilder tssb = new ValidationStepBuilder("BizUnit.XmlValidationStepEx", null);
            args = new object[1];
            args[0] = @"..\..\..\Test\BizUnit.Tests\Data\PurchaseOrder.xsd";
            tssb.SetProperty("XmlSchemaPath", args);

            args[0] = @"http://SendMail.PurchaseOrder";
            tssb.SetProperty("XmlSchemaNameSpace", args);

            args = new object[2];
            args[0] = "*[local-name()='PurchaseOrder' and namespace-uri()='http://SendMail.PurchaseOrder']/*[local-name()='PONumber' and namespace-uri()='']";
            args[1] = "PONumber_0";
            tssb.SetProperty("XPathValidations", args);

            // set the validation step
            tsb.ValidationStepBuilder = tssb;

            // Add the steps...
            testCase.AddTestStep(createFileStep, TestStage.Execution);
            testCase.AddTestStep(tsb, TestStage.Execution);

            string testDirectory = @"..\..\..\Test\BizUnit.Tests\Out";
            FileHelper.EmptyDirectory(testDirectory, "*.xml");

            Assert.AreEqual(FileHelper.NumberOfFilesInDirectory(testDirectory, "*.xml"), 0);

            BizUnit bizUnit = new BizUnit(testCase);
            bizUnit.RunTest();

            Assert.AreEqual(FileHelper.NumberOfFilesInDirectory(testDirectory, "*.xml"), 0);
        }

        [TestMethod]
        public void Check_PropertyInfo_String()
        {
            TestStepBuilder tsb1 = new TestStepBuilder("BizUnit.FileCreateStep", null);
            PropertyInfo pi = tsb1.GetPropertyInfo("SourcePath");
            Assert.AreEqual(pi.PropertyType, typeof(System.String));

            pi = tsb1.GetPropertyInfo("CreationPath");
            Assert.AreEqual(pi.PropertyType, typeof(System.String));
        }

        [TestMethod]
        public void Check_PropertyInfo_Various()
        {
            TestStepBuilder tsb1 = new TestStepBuilder("BizUnit.FileValidateStep", null);
            PropertyInfo pi = tsb1.GetPropertyInfo("Timeout");
            Assert.AreEqual(pi.PropertyType, typeof(System.Double));

            pi = tsb1.GetPropertyInfo("ValidationStep");
            Assert.AreEqual(pi.PropertyType, typeof(IValidationStepOM));

            pi = tsb1.GetPropertyInfo("ContextLoaderStep");
            Assert.AreEqual(pi.PropertyType, typeof(IContextLoaderStepOM));

            pi = tsb1.GetPropertyInfo("DeleteFile");
            Assert.AreEqual(pi.PropertyType, typeof(System.Boolean));

            pi = tsb1.GetPropertyInfo("SearchPattern");
            Assert.AreEqual(pi.PropertyType, typeof(System.String));
        }
    }
}
