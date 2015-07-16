
namespace BizUnit.SsisSteps
{
    using Microsoft.SqlServer.Dts.Runtime;

    public class SsisEventHandler : IDTSEvents
    {
        private Context _context;

        public SsisEventHandler(Context context)
        {
            _context = context;    
        }
        public void OnPreValidate(Executable exec, ref bool fireAgain)
        {
        }

        public void OnPostValidate(Executable exec, ref bool fireAgain)
        {
        }

        public void OnPreExecute(Executable exec, ref bool fireAgain)
        {
        }

        public void OnPostExecute(Executable exec, ref bool fireAgain)
        {
        }

        public void OnWarning(DtsObject source, int warningCode, string subComponent, string description, string helpFile, int helpContext, string idofInterfaceWithError)
        {
            _context.LogWarning("OnWarning: warningCode: {0}, subComponent: {1}, description: {2}", warningCode, subComponent, description);
        }

        public void OnInformation(DtsObject source, int informationCode, string subComponent, string description, string helpFile, int helpContext, string idofInterfaceWithError, ref bool fireAgain)
        {
            _context.LogInfo("OnWarning: informationCode: {0}, subComponent: {1}, description: {2}", informationCode, subComponent, description);
        }

        public bool OnError(DtsObject source, int errorCode, string subComponent, string description, string helpFile, int helpContext, string idofInterfaceWithError)
        {
            _context.LogInfo("OnWarning: errorCode: {0}, subComponent: {1}, description: {2}", errorCode, subComponent, description);
            return false;
        }

        public void OnTaskFailed(TaskHost taskHost)
        {
        }

        public void OnProgress(TaskHost taskHost, string progressDescription, int percentComplete, int progressCountLow, int progressCountHigh, string subComponent, ref bool fireAgain)
        {
        }

        public bool OnQueryCancel()
        {
            return true;
        }

        public void OnBreakpointHit(IDTSBreakpointSite breakpointSite, BreakpointTarget breakpointTarget)
        {
        }

        public void OnExecutionStatusChanged(Executable exec, DTSExecStatus newStatus, ref bool fireAgain)
        {
        }

        public void OnVariableValueChanged(DtsContainer DtsContainer, Variable variable, ref bool fireAgain)
        {
        }

        public void OnCustomEvent(TaskHost taskHost, string eventName, string eventText, ref object[] arguments, string subComponent, ref bool fireAgain)
        {
        }
    }
}
