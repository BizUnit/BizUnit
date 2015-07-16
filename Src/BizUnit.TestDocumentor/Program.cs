
using System;

namespace BizUnit.TestDocumentor
{
    using BizUnitDocumentor;

    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length != 6)
            {
                Usage(args);
                return;
            }

            var documentor = new DocumentBuilder();

            documentor.GenerateDocumentation(args[0], args[1], args[2], args[3], args[4], Convert.ToBoolean(args[5]));
        }

        private static void Usage(string[] args)
        {
            Console.WriteLine("Usage:");
            Console.WriteLine(string.Format("{0} arguments were supplied.", args.Length));
            Console.WriteLine("BizUnit.TestDocumentor.exe [Test Report Template] [Category Template] [Test Case Template] [BizUnit Test Directory] [Output File Name (.XML)] [Recursive]");
        }
    }
}
