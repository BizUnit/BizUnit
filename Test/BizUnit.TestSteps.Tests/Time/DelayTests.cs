
using System;
using System.Diagnostics;
using System.IO;
using BizUnit.TestSteps.DataLoaders.File;
using BizUnit.TestSteps.File;
using BizUnit.TestSteps.Time;
using BizUnit.Core.TestBuilder;
using NUnit.Framework;
using BizUnit.Core.Utilites;
using BizUnit.Core;

namespace BizUnit.TestSteps.Tests.Time
{
    /// <summary>
    /// Summary description for DelayTest
    /// </summary>
    [TestFixture]
    public class DelayTests
    {
        [Test]
        public void DelayTest()
        {
            int stepDelayDuration = 500;
            var step = new DelayStep();
            step.DelayMilliSeconds = stepDelayDuration;

            var sw = new Stopwatch();
            sw.Start();

            step.Execute(new Context());

            var actualDuration = sw.ElapsedMilliseconds;
            Console.WriteLine("Observed delay: {0}", actualDuration);
            Assert.AreEqual(stepDelayDuration, actualDuration, 20);

            stepDelayDuration = 5;
            step.DelayMilliSeconds = stepDelayDuration;

            sw = new Stopwatch();
            sw.Start();

            step.Execute(new Context());

            actualDuration = sw.ElapsedMilliseconds;
            Console.WriteLine("Observed delay: {0}", actualDuration);
            Assert.AreEqual(actualDuration, stepDelayDuration, 20);
        }

        [Test]
        public void DelayTestCaseTest()
        {
            DeleteFiles();
            int stepDelayDuration = 500;
            var step = new DelayStep();
            step.DelayMilliSeconds = stepDelayDuration;

            var sw = new Stopwatch();
            sw.Start();

            step.Execute(new Context());

            var actualDuration = sw.ElapsedMilliseconds;
            Console.WriteLine("Observed delay: {0}", actualDuration);
            Assert.AreEqual(stepDelayDuration, actualDuration, 20);

            stepDelayDuration = 5;
            step.DelayMilliSeconds = stepDelayDuration;

            var tc = new TestCase();
            tc.ExecutionSteps.Add(step);

            TestCase.SaveToFile(tc, Path.Combine(TestContext.CurrentContext.TestDirectory, "DelayTestCaseTest.xaml"));
            var bu = new TestRunner(TestCase.LoadFromFile(Path.Combine(TestContext.CurrentContext.TestDirectory, "DelayTestCaseTest.xaml")));

            sw = new Stopwatch();
            sw.Start();

            bu.Run();

            actualDuration = sw.ElapsedMilliseconds;
            Console.WriteLine("Observed delay: {0}", actualDuration);
            Assert.AreEqual(actualDuration, stepDelayDuration, 20);
        }

        [Test]
        public void DelaySampleTest()
        {
            DeleteFiles();

            // Create the test case
            var testCase = new TestCase();

            // Create test steps...
            var delayStep = new DelayStep {DelayMilliSeconds = 500};

            // Add test steps to the required test stage
            testCase.ExecutionSteps.Add(delayStep);

            // Create a new instance of BizUnit and run the test
            var bizUnit = new TestRunner(testCase);
            bizUnit.Run();

            // Save Test Case
            TestCase.SaveToFile(testCase, Path.Combine(TestContext.CurrentContext.TestDirectory, "DelaySampleTest.xaml"));
        }

        [Test]
        public void LoadSampleTest()
        {
            DelaySampleTest();

            // Load Test Case
            var testCase = TestCase.LoadFromFile(Path.Combine(TestContext.CurrentContext.TestDirectory, "DelaySampleTest.xaml"));

            // Create test steps...
            var dataLoader = new FileDataLoader {FilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, @"TestData\InputPO.xaml") };

            var fileCreate = new CreateStep {CreationPath = Path.Combine(TestContext.CurrentContext.TestDirectory, @"InputFile"), DataSource = dataLoader};

            testCase.ExecutionSteps.Add(fileCreate);

            // Save Test Case
            TestCase.SaveToFile(testCase, Path.Combine(TestContext.CurrentContext.TestDirectory, "ExtendedDelaySampleTest.xaml"));
        }

        private static void DeleteFiles()
        {
            TestHelper.DeleteFile(Path.Combine(TestContext.CurrentContext.TestDirectory, "DelaySampleTest.xaml"));
            TestHelper.DeleteFile(Path.Combine(TestContext.CurrentContext.TestDirectory, "ExtendedDelaySampleTest.xaml"));
            TestHelper.DeleteFile(Path.Combine(TestContext.CurrentContext.TestDirectory, "DelayTestCaseTest.xaml"));
        }
    }
}
