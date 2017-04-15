
using BizUnit.TestSteps.Azure.Redis;
using NUnit.Framework;
using BizUnit.Core.TestBuilder;
using BizUnit.TestSteps.DataLoaders;
using BizUnit.Core;

namespace Azure.TestSteps.Tests
{
    [TestFixture]
    public class PublishSubscribeTest
    {
        private static string RedisConnectionString = "[Redis Connection String here...]";

        [Test]
        public void SimplePubSubTest()
        {
            var rss = new RedisSubscribeStep();
            rss.ConnectionString = RedisConnectionString;
            rss.Topic = "SimplePubSubTest";
            rss.RunConcurrently = true;

            var rps = new RedisPublishStep();
            rps.ConnectionString = RedisConnectionString;
            rps.Topic = "SimplePubSubTest";
            var dl = new StringDataLoader() { Data = "SimplePubSubTest-1" };
            rps.Data = dl;

            var test = new TestCase();
            test.SetupSteps.Add(rss);
            test.ExecutionSteps.Add(rps);
            var tr = new TestRunner(test);
            tr.Run();
        }
    }
}
