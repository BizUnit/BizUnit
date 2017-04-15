
using StackExchange.Redis;
using System;

namespace BizUnit.TestSteps.Azure.Redis.Common
{
    public class RedisSubscriptionWrapper : IDisposable
    {
        ISubscriber _subscriber;
        RedisChannel _channel;

        public RedisSubscriptionWrapper(RedisChannel channel, ISubscriber subscriber)
        {
            _subscriber = subscriber;
            _channel = channel;
        }

        public void Dispose()
        {
            if (null != _subscriber)
            {
                _subscriber.Unsubscribe(_channel);
                _subscriber = null;
            }
        }
    }
}
