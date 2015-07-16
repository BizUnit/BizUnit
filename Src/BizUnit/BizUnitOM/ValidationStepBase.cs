
namespace BizUnit
{
    using System.IO;

    public abstract class ValidationStepBase
    {
        /// <summary>
        /// Called by the BizUnit framework to execute the validation test step
        /// </summary>
        /// 
        /// <param name='data'>The stream cintaining the data to be validated.</param>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public abstract void  ExecuteValidation(Stream data, Context context);

        /// <summary>
        /// Called by the BizUnit framework to validate that the validation step has been correctly configured
        /// </summary>
        public abstract void Validate(Context context);
    }
}
