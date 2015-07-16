
namespace BizUnit.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Summary description for BizUnitTests
    /// </summary>
    [TestClass]
    public class XmlTestCaseTests
    {
//        [ClassInitialize]
        static public void SetUp(TestContext context)
        {
            Console.WriteLine(Environment.CurrentDirectory);
            BizUnit bizUnit = new BizUnit(@"..\..\..\Test\BizUnit.Tests\TestCases\Test_Setup.xml", TestGroupPhase.TestGroupSetup);
            bizUnit.RunTest();
        }

//        [ClassCleanup]
        static public void TearDown()
        {
            BizUnit bizUnit = new BizUnit(@"..\..\..\Test\BizUnit.Tests\TestCases\Test_TearDown.xml", TestGroupPhase.TestGroupTearDown);
            bizUnit.RunTest();
        }

        [TestMethod]
        public void Test_01_FILECopyWithXmlValidation()
        {
            BizUnit bizUnit = new BizUnit(@"..\..\..\Test\BizUnit.Tests\TestCases\Test_01_FILECopyWithXmlValidation.xml");
            bizUnit.RunTest();
        }

        [TestMethod]
        public void Test_02_FILECopyWithBinaryValidation()
        {
            BizUnit bizUnit = new BizUnit(@"..\..\..\Test\BizUnit.Tests\TestCases\Test_02_FILECopyWithBinaryValidation.xml");
            bizUnit.RunTest();
        }

        [TestMethod]
        public void Test_03_RegExTest()
        {
            BizUnit bizUnit = new BizUnit(@"..\..\..\Test\BizUnit.Tests\TestCases\Test_03_RegExTest.xml");
            bizUnit.RunTest();
        }

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void Test_04_NegativeTest()
        {
            BizUnit bizUnit = new BizUnit(@"..\..\..\Test\BizUnit.Tests\TestCases\Test_04_NegativeTest.xml");
            bizUnit.RunTest();
        }
    }
}
