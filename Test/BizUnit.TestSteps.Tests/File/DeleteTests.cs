
using System;
using BizUnit.TestSteps.File;
using BizUnit.Xaml;
using NUnit.Framework;

namespace BizUnit.TestSteps.Tests.File
{
    using System.IO;
    using DataLoaders.File;

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
            step.CreationPath = @"..\..\TestData\DeleteTest_FileToBeDeleted.xml";
            var dl = new FileDataLoader();
            dl.FilePath = @"..\..\TestData\PurchaseOrder001.xml";
            step.DataSource = dl;
            step.Execute(new Context());

            var deleteStep = new DeleteStep();
            deleteStep.FilePathsToDelete.Add(@"..\..\TestData\DeleteTest_FileToBeDeleted.xml");
            deleteStep.Execute(new Context());

            try
            {
                var deletedFile = System.IO.File.Open(@"..\..\TestData\DeleteTest_FileToBeDeleted.xml", FileMode.Open,
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
            step.CreationPath = @"..\..\TestData\DeleteTest_FileToBeDeleted1.wildCardTestxml";
            var dl = new FileDataLoader();
            dl.FilePath = @"..\..\TestData\PurchaseOrder001.xml";
            step.DataSource = dl;
            step.Execute(new Context());

            step.CreationPath = @"..\..\TestData\DeleteTest_FileToBeDeleted2.wildCardTestxml";
            step.Execute(new Context());

            var deleteStep = new DeleteStep();
            deleteStep.FilePathsToDelete.Add(@"..\..\TestData\*.wildCardTestxml");
            deleteStep.Execute(new Context());

            try
            {
                var deletedFile = System.IO.File.Open(@"..\..\TestData\DeleteTest_FileToBeDeleted.wildCardTestxml", FileMode.Open,
                                    FileAccess.Read);
            }
            catch (System.IO.FileNotFoundException)
            {
                ; // Expected!                
            }
        }

        [ExpectedException(typeof(ArgumentNullException))]
        [Test]
        public void TestCaseValidationTest()
        {
            var step = new CreateStep();
            var tc = new TestCase();
            tc.ExecutionSteps.Add(step);
            var bu = new BizUnit(tc);
            bu.RunTest();
        }
    }
}
