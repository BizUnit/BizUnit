
using BizUnit.Common;
using BizUnit.TestBuilder;
using StackExchange.Redis;

namespace Azure.TestSteps.Redis.Common
{
    public abstract class RedisBaseStep : TestStepBase
    {
        protected ConnectionMultiplexer _connection;
        protected IDatabase _database;

        public string ConnectionString { get; set; }
        public RedisChannel Topic { get; set; }

        protected void Connect()
        {
            ArgumentValidation.CheckForEmptyString(ConnectionString, "CsonnectionString");

            _connection = ConnectionMultiplexer.Connect(ConnectionString);
            _database = _connection.GetDatabase();

            ArgumentValidation.CheckForNullReference(_connection, "_connection");
            ArgumentValidation.CheckForNullReference(_database, "_database");
        }
    }
}
