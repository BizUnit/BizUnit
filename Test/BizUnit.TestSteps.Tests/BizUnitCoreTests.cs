
using BizUnit.TestSteps.Time;
using BizUnit.Xaml;
using NUnit.Framework;

namespace BizUnit.TestSteps.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestFixture]
    public class BizUnitCoreTests
    {
        [Test]
        public void SerializationV4TestStepsOnly()
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
            btc.BizUnitVersion = "4.0.0.1";

            var fm = new DelayStep {DelayMilliSeconds = 35};
            btc.SetupSteps.Add(fm);

            var bu = new BizUnit(btc);
            bu.RunTest();
        }
    }
}
