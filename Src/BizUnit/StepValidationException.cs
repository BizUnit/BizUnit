
using System;
using BizUnit.Xaml;

namespace BizUnit
{
    /// <summary>
    /// TestStepExecutionException is thrown by BizUnit to indicate a validation step failed.
    /// </summary>
    /// <remarks>The ValidationStepExecutionException is thrown by BizUnit when a validation step fails, the 
    /// framework automatically wraps the exception thrown by the validaiton step with an 
    /// TestStepExecutionException</remarks>

    public class StepValidationException : Exception
    {
        private readonly TestStepBase _testStep;

        /// <summary>
        /// TestStepExecutionException constructor.
        /// </summary>
        /// <param name="message">The message associated with this exception.</param>
        /// <param name="testStep">The name of the BizUnit test step being validated.</param>
        public StepValidationException(string message, TestStepBase testStep)
            : base(message) 
        {
            _testStep = testStep;
        }

        /// <summary>
        /// The name of the test step
        /// </summary>
        public string TestStepName
        {
            get { return _testStep.GetType().ToString(); }
        }
    }
}
