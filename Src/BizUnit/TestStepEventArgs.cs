
using System;

namespace BizUnit
{
    public class TestStepEventArgs : EventArgs
    {
        internal TestStepEventArgs(TestStage stage, string testCaseName, string testStepTypeName)
        {
            Stage = stage;
            TestCaseName = testCaseName;
            TestStepTypeName = testStepTypeName;
        }

        public TestStage Stage { get; private set; }
        public string TestCaseName { get; private set; }
        public string TestStepTypeName { get; private set; }
    }
}
