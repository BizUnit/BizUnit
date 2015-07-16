
namespace BizUnit
{
    using System.IO;

    public abstract class SubStepBase
    {
        public abstract Stream Execute(Stream data, Context context);
    }
}
