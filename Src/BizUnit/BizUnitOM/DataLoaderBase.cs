
namespace BizUnit
{
    using System.IO;

    public abstract class DataLoaderBase
    {
        public abstract Stream Load(Context context);
    }
}
