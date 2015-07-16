
using System;
using System.Diagnostics;
using System.IO;
using BizUnit.TestSteps.DataLoaders.File;
using BizUnit.TestSteps.File;
using BizUnit.TestSteps.Time;
using BizUnit.Xaml;
using NUnit.Framework;

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

            TestCase.SaveToFile(tc, "DelayTestCaseTest.xaml");
            var bu = new BizUnit(TestCase.LoadFromFile("DelayTestCaseTest.xaml"));

            sw = new Stopwatch();
            sw.Start();

            bu.RunTest();

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
            var bizUnit = new BizUnit(testCase);
            bizUnit.RunTest();

            // Save Test Case
            TestCase.SaveToFile(testCase, "DelaySampleTest.xaml");
        }

        [Test]
        public void LoadSampleTest()
        {
            DelaySampleTest();

            // Load Test Case
            var testCase = TestCase.LoadFromFile("DelaySampleTest.xaml");

            // Create test steps...
            var dataLoader = new FileDataLoader {FilePath = @"TestData\InputPO.xaml"};

            var fileCreate = new CreateStep {CreationPath = @"C\InputFile", DataSource = dataLoader};

            testCase.ExecutionSteps.Add(fileCreate);

            // Save Test Case
            TestCase.SaveToFile(testCase, "ExtendedDelaySampleTest.xaml");
        }

        private static void DeleteFiles()
        {
            TestHelper.DeleteFile("DelaySampleTest.xaml");
            TestHelper.DeleteFile("ExtendedDelaySampleTest.xaml");
            TestHelper.DeleteFile("DelayTestCaseTest.xaml");
        }
    }
}
