
using BizUnit.Core.TestBuilder;
using BizUnit.Core.Common;
using BizUnit.TestSteps.Azure.Redis.Common;
using BizUnit.Core.ExtensionMethods;

namespace BizUnit.TestSteps.Azure.Redis
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
