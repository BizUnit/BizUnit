
using BizUnit.TestBuilder;
using BizUnit.Common;
using Azure.TestSteps.Redis.Common;
using BizUnit.ExtensionMethods;

namespace Azure.TestSteps.Redis
{
    public class RedisPublishStep : RedisBaseStep
    {
        public DataLoaderBase Data { get; set; }

        public override void Execute(Context context)
        {
            Connect();
            _database.Publish(Topic, Data.Load(context).GetAsString());
        }

        public override void Validate(Context context)
        {            
            ArgumentValidation.CheckForEmptyString(Topic, "Topic");
        }
    }
}
