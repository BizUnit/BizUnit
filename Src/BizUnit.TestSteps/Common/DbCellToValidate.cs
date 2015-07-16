
namespace BizUnitCoreTestSteps.Common
{
    ///<summary>
    /// Database cell to be validated
    ///</summary>
    public class DbCellToValidate
    {
        ///<summary>
        /// Default constructor
        ///</summary>
        public DbCellToValidate() {}

        ///<summary>
        /// The name of the cell to validate
        ///</summary>
        public string ColumnName { get; set; }

        ///<summary>
        /// the expected value of the cell to validate
        ///</summary>
        public string ExpectedValue { get; set; }
    }
}
