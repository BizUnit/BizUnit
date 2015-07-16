
namespace BizUnitCoreTestSteps.Common
{
    using System.Collections.ObjectModel;

    ///<summary>
    /// Database row to be validated
    ///</summary>
    public class DbRowToValidate
    {
        ///<summary>
        /// The cells to be validated
        ///</summary>
        public Collection<DbCellToValidate> Cells { get; set; }

        ///<summary>
        /// Default constructor
        ///</summary>
        public DbRowToValidate()
        {
            Cells = new Collection<DbCellToValidate>(); 
        }
    }
}
