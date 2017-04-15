
using BizUnit.Core.Common;
using BizUnit.Core.ExtensionMethods;
using BizUnit.Core.TestBuilder;
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
