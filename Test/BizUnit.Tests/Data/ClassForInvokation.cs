
using System;

namespace BizUnit.Tests.Data
{
    public class ClassForInvokation
    {
        public int AddNumbers(int a, int b)
        {
            Console.WriteLine("AddNumbers!");
            return a + b;
        }

        public string FormatString(string foo, int b)
        {
            Console.WriteLine("FormatString!");
            return String.Format(foo, b);
        }

        public void DoStuff()
        {
            Console.WriteLine("DoStuff!");
        }
    }
}
