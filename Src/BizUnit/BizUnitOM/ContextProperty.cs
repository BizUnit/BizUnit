
using System;

namespace BizUnit.BizUnitOM
{
    [Obsolete("ContextProperty has been deprecated. Please investigate the use of BizUnit.Xaml.TestCase.")]
    public class ContextProperty
    {
        public string Name { get; set; }

        public object GetPropertyValue(Context ctx)
        {
            if(null != ctx)
            {
                return ctx.GetObject(Name);
            }

            return null;
        }
    }
}
