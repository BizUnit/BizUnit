
namespace BizUnitCoreTestSteps
{
    using System.IO;
    using BizUnit;

    public class FileDataLoader : DataLoaderBase
    {
        public string FilePath { get; set; }

        public override Stream Load(Context context)
        {
            return StreamHelper.LoadFileToStream(FilePath);
        }
    }
}
