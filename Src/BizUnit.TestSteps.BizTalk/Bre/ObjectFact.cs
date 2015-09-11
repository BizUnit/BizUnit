
namespace BizUnit.TestSteps.BizTalk.Bre
{
    public class ObjectFact : Fact
    {
        public string Type { get; set; }
        public string AssemblyPath { get; set; }
        public string Args { get; set; }
        public override string GetFactType
        {
            get { return "ObjectFact"; }
        }
    }
}
