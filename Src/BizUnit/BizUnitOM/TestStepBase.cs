
namespace BizUnit
{
    using System;
    using System.ComponentModel;

    public abstract class TestStepBase
    {
        public bool RunConcurrently { get; set; }
        public bool FailOnError { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Exception ExecuteException { get; set; }

        public abstract void Execute(Context context);
        public abstract void Validate(Context context);

        public TestStepBase()
        {
            FailOnError = true;
        }
    }
}
