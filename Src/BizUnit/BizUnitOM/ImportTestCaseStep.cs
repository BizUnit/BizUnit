
namespace BizUnit
{
    sealed public class ImportTestCaseStep : TestStepBase
    {
        public string TestCasePath { get; set; }

        public BizUnitTestCaseXaml GetTestCase()
        {
            return BizUnitTestCaseXaml.LoadFromFile(TestCasePath);
        }

        public override void Execute(Context context)
        {
            ; // no op
        }

        public override void Validate(Context context)
        {
            ;
        }
    }
}
