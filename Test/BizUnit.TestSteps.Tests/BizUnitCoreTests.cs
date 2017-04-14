
using BizUnit.TestBuilderteps.Time;
using BizUnit.TestBuilder;
using NUnit.Framework;

namespace BizUnit.TestBuilderteps.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestFixture]
    public class BizUnitCoreTests
    {
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
