
namespace BizUnit.TestSteps.Ssis
{
    ///<summary>
    /// SSIS Package variable
    ///</summary>
    public class PackageVariable
    {
        ///<summary>
        /// The name of the SSIS variable
        ///</summary>
        public string Name { get; set; }

        ///<summary>
        /// The value to assign to the SSIS variable
        ///</summary>
        public object Value { get; set; }
    }
}
 