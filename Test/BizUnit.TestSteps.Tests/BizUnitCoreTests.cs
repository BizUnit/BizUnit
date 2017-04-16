
using BizUnit.TestSteps.Time;
using BizUnit.Core.TestBuilder;
using NUnit.Framework;
using BizUnit.Core.Utilites;
using BizUnit.Core;
using BizUnit.TestSteps.File;
using BizUnit.TestSteps.DataLoaders.File;
using System.IO;

namespace BizUnit.TestSteps.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestFixture]
    public class BizUnitCoreTests
    {
        [Test]
        public void SampleTestCase()
        {
            var sourceDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData");
            var sourceFilePath = Path.Combine(sourceDirectory, "PurchaseOrder001.xml");
            var targetDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestArea");
            var targetFilePath = Path.Combine(targetDirectory, "FileCreateStepTest.xml");

            var fds = new DeleteStep();
            fds.FilePathsToDelete.Add(targetFilePath);

            var fcs = new CreateStep();
            fcs.CreationPath = targetFilePath;
            var dl = new FileDataLoader();
            dl.FilePath = sourceFilePath;
            fcs.DataSource = dl;

            var frs = new FileReadStep();
            frs.DirectoryPath = targetDirectory;
            frs.SearchPattern = "*.xml";
            frs.Timeout = 3000;
            frs.DeleteFile = true;

            var tc = new TestCase();
            tc.SetupSteps.Add(fds);
            tc.ExecutionSteps.Add(fcs);
            tc.ExecutionSteps.Add(frs);
            tc.CleanupSteps.Add(fds);

            var testRunner = new TestRunner(tc);
            testRunner.Run();

            TestCase.SaveToFile(
                tc,
                Path.Combine(
                    TestContext.CurrentContext.TestDirectory,
                    @"..\..\",
                    "TestCases",
                    "SampleTest.xaml"),
                true);
        }

        [Test]
        public void SampleXamlTestCase()
        {
            var xamlTestCase = Path.Combine(
                TestContext.CurrentContext.TestDirectory,
                @"..\..\",
                "TestCases",
                "SampleTest.xaml");

            // Example running a test case from a XAML test case
            var tc = TestCase.LoadFromFile(xamlTestCase);
            var runner = new TestRunner(tc);
            runner.Run();
        }

        [Test]
        public void SerializationTestStepsOnly()
        {
            var btc = new TestCase();
            btc.Name = "Serialization Test";

            var fm = new DelayStep();
            fm.DelayMilliSeconds = 35;
            btc.SetupSteps.Add(fm);

            string testCase = TestCase.Save(btc);
            var btcNew = TestCase.LoadXaml(testCase);
        }

        [Test]
        public void ExecuteTestCase()
        {
            var btc = new TestCase();
            btc.Name = "Serialization Test";
            btc.Description = "Test to blah blah blah, yeah really!";
            btc.BizUnitVersion = "5.0.0.0";

            var fm = new DelayStep {DelayMilliSeconds = 35};
            btc.SetupSteps.Add(fm);

            var bu = new TestRunner(btc);
            bu.Run();
        }
    }
}
