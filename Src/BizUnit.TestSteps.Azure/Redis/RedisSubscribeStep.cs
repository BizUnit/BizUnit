
using BizUnit.Core.TestBuilder;
using BizUnit.Core.Common;
using System;
using StackExchange.Redis;
using System.Collections.Concurrent;
using BizUnit.TestSteps.Azure.Redis.Common;

namespace BizUnit.TestSteps.Azure.Redis
{
    public class RedisSubscribeStep : RedisBaseStep
    {
        private Action<RedisChannel, RedisValue> _trigger;
        private ISubscriber _subscriber;
        ConcurrentQueue<RedisValue> _data;
        internal static string RedisSubsciptionResultsKeyName = "RedisSubsciptionResults";

        public RedisValue GetNextValue()
        {
            RedisValue val;
            _data.TryDequeue(out val);
            return val;
        }
        public string SubscriptionResultCtxKey { get; set; }

        public override void Execute(Context context)
        {
            _data = new ConcurrentQueue<RedisValue>();
            context.Add(SubscriptionResultCtxKey, _data);

            Connect();
            Subscribe(context);
        }

        public override void Validate(Context context)
        {
            if (string.IsNullOrEmpty(SubscriptionResultCtxKey))
                SubscriptionResultCtxKey = RedisSubsciptionResultsKeyName;
            
            ArgumentValidation.CheckForEmptyString(Topic, "Topic");
        }

        private void Subscribe(Context context)
        {
            _subscriber = _connection.GetSubscriber();
            _trigger = (channel, value) => 
            {
                _data.Enqueue(value);
            };

            _subscriber.Subscribe(Topic, _trigger);
            context.Add(Guid.NewGuid().ToString(), new RedisSubscriptionWrapper(Topic, _subscriber));
        }
    }
}
