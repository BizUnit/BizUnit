
using System;
using BizUnit.Core.TestBuilder;
using NUnit.Framework;

namespace BizUnit.TestSteps.Tests.File
{
    using System.IO;
    using DataLoaders.File;
    using BizUnit.TestSteps.File;
    using BizUnit.Core.Utilites;
    using BizUnit.Core;

    /// <summary>
    /// Summary description for DeleteTest
    /// </summary>
    [TestFixture]
    public class DeleteTests
    {
        [Test]
        public void DeleteFileTest()
        {
            var step = new CreateStep();
            step.CreationPath = Path.Combine(TestContext.CurrentContext.TestDirectory, @"TestData\DeleteTest_FileToBeDeleted.xml");
            var dl = new FileDataLoader();
            dl.FilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, @"TestData\PurchaseOrder001.xml");
            step.DataSource = dl;
            step.Execute(new Context());

            var deleteStep = new DeleteStep();
            deleteStep.FilePathsToDelete.Add(Path.Combine(TestContext.CurrentContext.TestDirectory, @"TestData\DeleteTest_FileToBeDeleted.xml"));
            deleteStep.Execute(new Context());

            try
            {
                var deletedFile = System.IO.File.Open(Path.Combine(TestContext.CurrentContext.TestDirectory, @"TestData\DeleteTest_FileToBeDeleted.xml"), 
                    FileMode.Open,
                    FileAccess.Read);
            }
            catch (System.IO.FileNotFoundException)
            {
                ; // Expected!                
            }
        }

        [Test]
        public void DeleteFileByWildCardTest()
        {
            var step = new CreateStep();
            step.CreationPath = Path.Combine(TestContext.CurrentContext.TestDirectory, @"TestData\DeleteTest_FileToBeDeleted1.wildCardTestxml");
            var dl = new FileDataLoader();
            dl.FilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, @"TestData\PurchaseOrder001.xml");
            step.DataSource = dl;
            step.Execute(new Context());

            step.CreationPath = Path.Combine(TestContext.CurrentContext.TestDirectory, @"TestData\DeleteTest_FileToBeDeleted2.wildCardTestxml");
            step.Execute(new Context());

            var deleteStep = new DeleteStep();
            deleteStep.FilePathsToDelete.Add(Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\TestData\*.wildCardTestxml"));
            deleteStep.Execute(new Context());

            try
            {
                var deletedFile = System.IO.File.Open(Path.Combine(TestContext.CurrentContext.TestDirectory, @"TestData\DeleteTest_FileToBeDeleted.wildCardTestxml"), 
                    FileMode.Open,
                    FileAccess.Read);
            }
            catch (System.IO.FileNotFoundException)
            {
                ; // Expected!                
            }
        }

        [Test]
        public void TestCaseValidationTest()
        {
            var step = new CreateStep();
            var tc = new TestCase();
            tc.ExecutionSteps.Add(step);
            Assert.Throws<ArgumentNullException>(() => { var bu = new TestRunner(tc); });            
        }
    }
}
