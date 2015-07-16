
using System;

namespace BizUnit.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Summary description for BizUnitTests
    /// </summary>
    [TestClass]
    public class BizUnitTests
    {

        [TestMethod]
        public void EventTest()
        {
            Console.WriteLine(Environment.CurrentDirectory);
            BizUnit bizUnit =
                new BizUnit(@"..\..\..\Test\BizUnit.Tests\Data\BizUnitEventTest001.xml");

            bizUnit.TestStepStartEvent += TestStepStart;
            bizUnit.TestStepStartEvent += TestStepStart;

            bizUnit.TestStepStopEvent += TestStepStop;

            bizUnit.RunTest();
        }

        void TestStepStart(object obj, TestStepEventArgs e)
        {
            ;
        }

        void TestStepStop(object obj, TestStepEventArgs e)
        {
            ;
        }
    }
}
