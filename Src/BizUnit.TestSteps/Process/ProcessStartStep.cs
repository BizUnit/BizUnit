
using BizUnit.Common;
using BizUnit.TestBuilder;
using System;
using System.Diagnostics;
using System.IO;

namespace BizUnit.TestSteps.Process
{
    public class ProcessStartStep : TestStepBase
    {
        public string ProcessPath { get; set; }
        public string Arguments { get; set; }

        public override void Execute(Context context)
        {
            var pi = new ProcessStartInfo();
            pi.FileName = ProcessPath;
            pi.Arguments = Arguments;
            pi.WorkingDirectory = Path.GetDirectoryName(ProcessPath);
            context.LogInfo("About to start process: '{0}', arguments: '{1}'", ProcessPath, Arguments);
            var proc = new ProcessWrapper(System.Diagnostics.Process.Start(pi));
            context.Add(Guid.NewGuid().ToString(), proc);
        }

        public override void Validate(Context context)
        {
            ArgumentValidation.CheckForEmptyString(ProcessPath, "ProcessPath");
            // Arguments - optional
        }
    }
}
