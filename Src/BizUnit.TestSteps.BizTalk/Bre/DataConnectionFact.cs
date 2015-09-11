
namespace BizUnit.TestSteps.BizTalk.Bre
{
    public class DataConnectionFact : Fact
    {
        public string ConnectionString { get; set; }
        public string Dataset { get; set; }
        public string TableName { get; set; }
        public override string GetFactType
        {
            get { return "DataConnectionFact"; }
        }
    }
}
