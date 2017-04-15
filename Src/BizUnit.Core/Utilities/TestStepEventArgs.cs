
using System;

namespace BizUnit.Core.Utilites
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'TestStepEventArgs'
    public class TestStepEventArgs : EventArgs
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'TestStepEventArgs'
    {
        internal TestStepEventArgs(TestStage stage, string testCaseName, string testStepTypeName)
        {
            Stage = stage;
            TestCaseName = testCaseName;
            TestStepTypeName = testStepTypeName;
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'TestStepEventArgs.Stage'
        public TestStage Stage { get; private set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'TestStepEventArgs.Stage'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'TestStepEventArgs.TestCaseName'
        public string TestCaseName { get; private set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'TestStepEventArgs.TestCaseName'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'TestStepEventArgs.TestStepTypeName'
        public string TestStepTypeName { get; private set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'TestStepEventArgs.TestStepTypeName'
    }
}
