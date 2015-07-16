
namespace BizUnit.TestSteps.Sql
{
    ///<summary>
    /// Database cell to be validated
    ///</summary>
    public class DbCellToValidate
    {
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
