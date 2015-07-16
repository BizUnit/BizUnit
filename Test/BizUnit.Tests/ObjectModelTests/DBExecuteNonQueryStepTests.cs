
namespace BizUnit.Tests.ObjectModelTests
{
    using System;
    using BizUnitOM;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Summary description for DBExecuteNonQueryStepTests
    /// </summary>
    [TestClass]
    public class DBExecuteNonQueryStepTests
    {
        [TestMethod]
        public void DBExecuteNonQueryStep_Create()
        {
            const string ConnectionString = "FooBar";
            const int DelayBeforeExecution = 3;
            const int NumberOfRowsAffected = 5;

            TestStepBuilder tsb = new TestStepBuilder("BizUnit.DBExecuteNonQueryStep", null);
            object[] args = new object[1];
            args[0] = ConnectionString;
            tsb.SetProperty("ConnectionString", args);

            args = new object[1];
            args[0] = DelayBeforeExecution;
            tsb.SetProperty("DelayBeforeExecution", args);

            args = new object[1];
            args[0] = NumberOfRowsAffected;
            tsb.SetProperty("NumberOfRowsAffected", args);

            args = new object[3];
            args[0] = "INSERT INTO TABLE (COLUMN1, COLUMN2) VALUES (VALUE1, {0},{1})";
            args[1] = "Foo";
            args[2] = (Int32)32;
            tsb.SetProperty("SQLQuery", args);


            Context context = new Context();
            ITestStepOM testStep = tsb.TestStepOM;
            testStep.Validate(context);

            DBExecuteNonQueryStep dbe = testStep as DBExecuteNonQueryStep;
            Assert.IsNotNull(dbe);
            Assert.AreEqual(dbe.ConnectionString, ConnectionString);
            Assert.AreEqual(dbe.DelayBeforeExecution, DelayBeforeExecution);
            Assert.AreEqual(dbe.NumberOfRowsAffected, NumberOfRowsAffected);
            Assert.AreEqual(dbe.SQLQuery.GetFormattedSqlQuery(),
                            "INSERT INTO TABLE (COLUMN1, COLUMN2) VALUES (VALUE1, Foo,32)");
        }
    }
}
