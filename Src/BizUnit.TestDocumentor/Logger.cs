
namespace BizUnit.TestDocumentor
{
    using System;

    public class Logger : ILogger
    {
        public void Info(string message, params object[] args)
        {
            System.Console.WriteLine("Info: {0}", string.Format(message, args));
        }

        public void Warning(string message, params object[] args)
        {
            System.Console.WriteLine("Warning: {0}", string.Format(message, args));
        }

        public void Error(Exception ex)
        {
            System.Console.WriteLine("Error: {0}", ex);
        }
    }
}
