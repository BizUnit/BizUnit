

using BizUnit.BizUnitOM;

namespace BizUnit.Tests.ObjectModelTests
{
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Summary description for ObjectModelExecutionTests
    /// </summary>
    [TestClass]
    public class ObjectModelExecutionTests
    {
        [TestMethod]
        public void FileCreateStepTest()
        {
            string testDirectory = @"..\..\..\Test\BizUnit.Tests\Out";
            FileHelper.EmptyDirectory(testDirectory, "*.xml");
            
            Assert.AreEqual(FileHelper.NumberOfFilesInDirectory(testDirectory, "*.xml"), 0);

            BizUnitTestCase testCase = new BizUnitTestCase("FileCreateStepTest");

            FileCreateStep fcs = new FileCreateStep();
            fcs.SourcePath = @"..\..\..\Test\BizUnit.Tests\Data\LoadGenScript001.xml";
            fcs.CreationPath = testDirectory + @"\Data_%Guid%.xml";
            testCase.AddTestStep(fcs, TestStage.Execution);

            BizUnit bizUnit = new BizUnit(testCase);
            bizUnit.RunTest();

            Assert.AreEqual(FileHelper.NumberOfFilesInDirectory(testDirectory, "*.xml"), 1);
        }

        [TestMethod]
        public void FileCreateStep_FileDeleteMultipleStepTest()
        {
            string testDirectory = @"..\..\..\Test\BizUnit.Tests\Out";
            FileHelper.EmptyDirectory(testDirectory, "*.xml");

            Assert.AreEqual(FileHelper.NumberOfFilesInDirectory(testDirectory, "*.xml"), 0);

            BizUnitTestCase testCase = new BizUnitTestCase("FileCreateStep_FileDeleteMultipleStepTest");

            FileCreateStep fcs = new FileCreateStep();
            fcs.SourcePath = @"..\..\..\Test\BizUnit.Tests\Data\LoadGenScript001.xml";
            fcs.CreationPath = testDirectory + @"\Data_%Guid%.xml";
            testCase.AddTestStep(fcs, TestStage.Execution);

            FileDeleteMultipleStep fds = new FileDeleteMultipleStep();
            fds.Directory = testDirectory;
            fds.SearchPattern = "*.xml";
            testCase.AddTestStep(fds, TestStage.Execution);

            BizUnit bizUnit = new BizUnit(testCase);
            bizUnit.RunTest();

            Assert.AreEqual(FileHelper.NumberOfFilesInDirectory(testDirectory, "*.xml"), 0);
        }

        [TestMethod]
        public void FileCreateStepTest_Negative()
        {
            string testDirectory = @"..\..\..\Test\BizUnit.Tests\Out";
            FileHelper.EmptyDirectory(testDirectory, "*.xml");

            Assert.AreEqual(FileHelper.NumberOfFilesInDirectory(testDirectory, "*.xml"), 0);

            BizUnitTestCase testCase = new BizUnitTestCase("FileCreateStepTest_Negative");

            FileCreateStep fcs = new FileCreateStep();
            fcs.SourcePath = @"C:\GarbageDirectory__NoOneWouldHaveADirCalledThisSurely\LoadGenScript001.xml";
            fcs.CreationPath = testDirectory + @"\Data_%Guid%.xml";
            testCase.AddTestStep(fcs, TestStage.Execution);

            BizUnit bizUnit = new BizUnit(testCase);

            bool exceptionCaught = false;

            try
            {
                bizUnit.RunTest();
            }
            catch(DirectoryNotFoundException)
            {
                exceptionCaught = true;
            }

            Assert.IsTrue(exceptionCaught);
            Assert.AreEqual(FileHelper.NumberOfFilesInDirectory(testDirectory, "*.xml"), 0);
        }

        [TestMethod]
        public void ObjectModelMixedWithConfigTest()
        {
            string config = ResourceLoaderHelper.GetResourceData("Data", "FileMoveConfig.xml");
            string testDirectory = @"..\..\..\Test\BizUnit.Tests\Out";
            FileHelper.EmptyDirectory(testDirectory, "*.xml");

            Assert.AreEqual(FileHelper.NumberOfFilesInDirectory(testDirectory, "*.xml"), 0);

            BizUnitTestCase testCase = new BizUnitTestCase("ObjectModelMixedWithConfigTest");

            // Add an object model defined BizUnit step...
            FileCreateStep fcs = new FileCreateStep();
            fcs.SourcePath = @"..\..\..\Test\BizUnit.Tests\Data\LoadGenScript001.xml";
            fcs.CreationPath = testDirectory + @"\InDoc1.xml";
            testCase.AddTestStep(fcs, TestStage.Execution);

            // Add a config defined BizUnit step...
            FileMoveStep fms = new FileMoveStep();
            testCase.AddTestStep(fms, config, TestStage.Execution);

            BizUnit bizUnit = new BizUnit(testCase);
            bizUnit.RunTest();

            Assert.AreEqual(FileHelper.NumberOfFilesInDirectory(testDirectory, "*.xml"), 1);
        }
    }
}
