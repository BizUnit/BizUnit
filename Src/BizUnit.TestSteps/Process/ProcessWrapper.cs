
using System;

namespace BizUnit.TestSteps.Process
{
    public class ProcessWrapper : IDisposable
    {
        System.Diagnostics.Process _process;
        public ProcessWrapper(System.Diagnostics.Process process)
        {
            _process = process;
        }

        public void Dispose()
        {
            if(null != _process)
            {
                _process.Kill();
                _process = null;
            }
        }
    }
}
