
using System;

namespace BizUnit.TestDocumentor
{
    public interface ILogger
    {
        void Info(string message, params object[] args);
        void Warning(string message, params object[] args);
        void Error(Exception ex);
    }
}
