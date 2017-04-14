
using Azure.TestSteps.Redis;
using NUnit.Framework;
using BizUnit.TestBuilder;
using BizUnit;
using BizUnit.TestSteps.DataLoaders;

namespace Azure.TestSteps.Tests
{
    [TestFixture]
    public class PublishSubscribeTest
    {
        private static string RedisConnectionString = "kevinsmi.redis.cache.windows.net:6379,password=ilv+DGpz01rx+PI3qjcDSn4OjlF1iXTmE5OD6d3mlBw=,ssl=False,abortConnect=False";

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
