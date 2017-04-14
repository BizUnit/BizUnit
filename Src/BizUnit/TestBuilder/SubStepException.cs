
using System;

namespace BizUnit.TestBuilder
{
    /// <summary>
    /// SubStepException maybe thrown by BizUnit sub steps to indicate a failure of the sub step.
    /// </summary>
    /// <remarks>SubStepException maybe thrown by BizUnit sub steps to indicate a failure of the sub step.</remarks>
    public class SubStepException : Exception
    {
        private readonly SubStepBase _subStep;

        /// <summary>
        /// TestStepExecutionException constructor.
        /// </summary>
        /// <param name="message">The message associated with this exception.</param>
        /// <param name="subStep">The name of the BizUnit test step being validated.</param>
        public SubStepException(string message, SubStepBase subStep)
            : base(message) 
        {
            _subStep = subStep;
        }

        /// <summary>
        /// TestStepExecutionException constructor.
        /// </summary>
        /// <param name="message">The message associated with this exception.</param>
        /// <param name="subStep">The name of the BizUnit test step being validated.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public SubStepException(string message, SubStepBase subStep, params object[] args)
            : base(string.Format(message, args))
        {
            _subStep = subStep;
        }
        
        /// <summary>
                 /// The name of the test step
                 /// </summary>
        public string SubStepName
        {
            get { return _subStep.GetType().ToString(); }
        }
    }
}
