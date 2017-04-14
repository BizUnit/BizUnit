
using BizUnit.Common;
using BizUnit.ExtensionMethods;
using BizUnit.TestBuilder;
using System.IO;

namespace BizUnit.TestSteps.DataLoaders
{
    public class StringDataLoader : DataLoaderBase
    {
        public string Data { get; set; }

        public override Stream Load(Context context)
        {
            return Data.GetAsStream();
        }

        public override void Validate(Context context)
        {
            ArgumentValidation.CheckForEmptyString(Data, "Data");
        }
    }
}
