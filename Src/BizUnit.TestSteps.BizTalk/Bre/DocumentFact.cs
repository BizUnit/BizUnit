
namespace BizUnit.TestSteps.BizTalk.Bre
{
    public class DocumentFact : Fact
    {
        public string InstanceDocument { get; set; }
        public string SchemaType { get; set; }
        public override string GetFactType
        {
            get { return "DocumentFact"; }
        }
    }
}
