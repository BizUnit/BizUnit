//---------------------------------------------------------------------
// File: DBQueryStep.cs
// 
// Summary: 
//
//---------------------------------------------------------------------
// Copyright (c) 2004-2015, Kevin B. Smith. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------

using System;
using System.Collections;
using System.Xml;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using BizUnit.BizUnitOM;
using BizUnit.Common;

namespace BizUnit.CoreSteps.TestSteps
{
	/// <summary>
	/// The DBQueryStep is used to query a SQL query against a database and validate the values for the row returned.
	/// </summary>
	/// 
	/// <remarks>
	/// The following shows an example of the Xml representation of this test step.
	/// 
	/// <code escaped="true">
	///	<TestStep assemblyPath="" typeName="BizUnit.DBQueryStep">
	///     <NumberOfRowsExpected>1</NumberOfRowsExpected>
	///		<DelayBeforeCheck>1</DelayBeforeCheck>
	///		<ConnectionString>Persist Security Info=False;Integrated Security=SSPI;database=BAMPrimaryImport;server=(local);Connect Timeout=30</ConnectionString>
	/// 
	///		<!-- 
	///		The SQL Query to execute is built by formatting the RawSQLQuery substituting in the 
	///		SQLQueryParam's
	///		-->
	///		<SQLQuery>
	///			<RawSQLQuery>select * from dbo.bam_Event_Completed where EventId = {0}</RawSQLQuery>
	///			<SQLQueryParams>
	///				<SQLQueryParam takeFromCtx="EventId"></SQLQueryParam>
	///			</SQLQueryParams>
	///		</SQLQuery>
	///			
	///		<Rows>
	///			<Columns>
	///			<!-- 
	///			Note: The column names are dependant on the DB schema being checked against.
	///			Adding the attribute isUnique="true" to one of the columns allows it to be 
	///			uniquely selcted in the scenario where multiple rows are returned.
	///			-->
	///					
	///				<EventType>Switch</EventType>
	///				<EventStatus>Completed</EventStatus>
	///				<ProcessorId>JVQFFCCZ0</ProcessorId>
	///				<SchemeId>GF81300000</SchemeId>
	///				<EventFailed>null</EventFailed>
	///				<EventHeld>null</EventHeld>
	///				<EventHoldNotifRec>null</EventHoldNotifRec>
	///			</Columns>
	///		</Rows>	
	///	</TestStep>
	///	</code>
	///	
	///	The ContextManipulator builds a new context item by appeanding the values of multiple context items
	///	<list type="table">
	///		<listheader>
	///			<term>Tag</term>
	///			<description>Description</description>
	///		</listheader>
	///		<item>
	///			<term>DelayBeforeCheck</term>
	///			<description>The number of seconds to wait before executing the step</description>
	///		</item>
	///		<item>
	///			<term>ConnectionString</term>
	///			<description>The connection string used for the DB query</description>
	///		</item>
	///		<item>
	///			<term>NumberOfRowsExpected</term>
	///			<description>The expected number of rows (optional)</description>
	///		</item>	
	///		<item>
	///			<term>SQLQuery/RawSQLQuery</term>
	///			<description>The raw SQL string that will be formatted by substituting in the SQLQueryParam</description>
	///		</item>
	///		<item>
	///			<term>SQLQuery/SQLQueryParams/SQLQueryParam</term>
	///			<description>The parameters to substitute into RawSQLQuery <para>(repeating)</para></description>
	///		</item>
	///		<item>
	///			<term>Rows/Columns</term>
	///			<description>One or more columns which represent the expected query result</description>
	///		</item>
	///		<item>
	///			<term>Rows/Columns/User defined element</term>
	///			<description>The fields that are validated in the response</description>
	///		</item>
	///	</list>
	///	</remarks>
    [Obsolete("DBQueryStep has been deprecated. Investigate the BizUnit.TestSteps namespace.")]
    public class DBQueryStep : ITestStepOM
	{
	    private int _delayBeforeCheck;
	    private string _connectionString;
        private SqlQuery _sqlQuery;
	    private DBRowsToValidate _dbRowsToValidate = new DBRowsToValidate();
	    private int _numberOfRowsExpected;

	    public int DelayBeforeExecution
        {
            set { _delayBeforeCheck = value; }
            get { return _delayBeforeCheck; }
        }

        public string ConnectionString
        {
            set { _connectionString = value; }
            get { return _connectionString; }
        }

        [BizUnitParameterFormatter("BizUnit.SqlQueryParamFormatter")]
        public SqlQuery SQLQuery
        {
            set { _sqlQuery = value; }
            get { return _sqlQuery; }
        }

        [BizUnitParameterFormatter("BizUnit.DBRowsToValidateParamFormatter")]
        public DBRowsToValidate DBRowsToValidate
	    {
	        set { _dbRowsToValidate = value; }
            get { return _dbRowsToValidate; }
	    }

	    public int NumberOfRowsExpected
	    {
	        set { _numberOfRowsExpected = value; }
            get { return _numberOfRowsExpected; }
	    }

        /// <summary>
		/// ITestStep.Execute() implementation
		/// </summary>
		/// <param name='testConfig'>The Xml fragment containing the configuration for this test step</param>
		/// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
		public void Execute(XmlNode testConfig, Context context)
		{
			_delayBeforeCheck = context.ReadConfigAsInt32( testConfig, "DelayBeforeCheck" );			
			_connectionString = context.ReadConfigAsString( testConfig, "ConnectionString" );
			var queryConfig = testConfig.SelectSingleNode( "SQLQuery" );
            _sqlQuery = SqlQuery.BuildSQLQuery(queryConfig, context);
            _numberOfRowsExpected = context.ReadConfigAsInt32(testConfig, "NumberOfRowsExpected", true);			

            var rowCollection = testConfig.SelectSingleNode("Rows");
            var bamValidationRows = rowCollection.SelectNodes("*");
            foreach (XmlNode bamValidationRow in bamValidationRows)
            {
                var drtv = new DBRowToValidate();
                
                var bamValidationCols = bamValidationRow.SelectNodes("*");
                foreach (XmlNode bamValidationCol in bamValidationCols)
                {
                    bool isUnique = context.ReadConfigAsBool(bamValidationCol, "@isUnique", true);
                    string colName = bamValidationCol.LocalName;
                    string colValue = bamValidationCol.InnerText;
                    var dctv = new DBCellToValidate(colName, colValue);
                    if (isUnique)
                    {
                        drtv.AddUniqueCell(dctv);
                    }
                    else
                    {
                        drtv.AddCell(dctv);
                    }
                }

                _dbRowsToValidate.AddRow(drtv);
            }

            Execute(context);
		}

		private static int ValidateData( object dbData, string targetValue, ref string dbDataStringValue)
		{
			dbDataStringValue = Convert.ToString(dbData);

			switch( dbData.GetType().ToString() )
			{
				case( "System.DateTime" ):
					var dbDt = (DateTime)dbData;
					var targetDt = Convert.ToDateTime(targetValue);
					return targetDt.CompareTo( dbDt );

				case( "System.DBNull" ):
					dbDataStringValue = "null";
					return targetValue.CompareTo( "null" );

				case( "System.String" ):
					dbDataStringValue = (string)dbData;
					return targetValue.CompareTo( (string)dbData );
                     
				case( "System.Int16" ):
					var dbInt16 = (Int16)dbData;
					var targetInt16 = Convert.ToInt16(targetValue);
					return targetInt16.CompareTo( dbInt16 );

				case( "System.Int32" ):
					var dbInt32 = (Int32)dbData;
					var targetInt32 = Convert.ToInt32(targetValue);
					return targetInt32.CompareTo( dbInt32 );

				case( "System.Int64" ):
					var dbInt64 = (Int64)dbData;
					var targetInt64 = Convert.ToInt64(targetValue);
					return targetInt64.CompareTo( dbInt64 );

				case( "System.Double" ):
					var dbDouble = (Double)dbData;
					var targetDouble = Convert.ToDouble(targetValue);
					return targetDouble.CompareTo( dbDouble );

				case( "System.Decimal" ):
					var dbDecimal = (Decimal)dbData;
					var targetDecimal = Convert.ToDecimal(targetValue);
					return targetDecimal.CompareTo( dbDecimal );

				case( "System.Boolean" ):
					var dbBoolean = (Boolean)dbData;
					var targetBoolean = Convert.ToBoolean(targetValue);
					return targetBoolean.CompareTo( dbBoolean );

				case( "System.Byte" ):
					var dbByte = (Byte)dbData;
					var targetByte = Convert.ToByte(targetValue);
					return targetByte.CompareTo( dbByte );

				case( "System.Char" ):
					var dbChar = (Char)dbData;
					var targetChar = Convert.ToChar(targetValue);
					return targetChar.CompareTo( dbChar );

				case( "System.SByte" ):
					var dbSByte = (SByte)dbData;
					var targetSByte = Convert.ToSByte(targetValue);
					return targetSByte.CompareTo( dbSByte );

				default:
					throw new ApplicationException(string.Format("Unsupported type: {0}", dbData.GetType()) );
			}
		}

		private static DataSet FillDataSet(string connectionString, string sqlQuery)
		{
			var connection = new SqlConnection(connectionString);
			var ds = new DataSet();

			using ( connection )
			{
				var adapter = new SqlDataAdapter
				                  {
				                      SelectCommand = new SqlCommand(sqlQuery, connection)
				                  };
			    adapter.Fill( ds );
				return ds;
			}
		}

		private static string BuildSqlQuery( XmlNode queryConfig, Context context )
		{
			var rawSqlQuery = context.ReadConfigAsString( queryConfig, "RawSQLQuery" );			
			var sqlParams = queryConfig.SelectNodes( "SQLQueryParams/*" );

			if ( null != sqlParams )
			{
				var paramArray = new ArrayList();
				//context

				foreach(XmlNode sqlParam in sqlParams)
				{
					var p = context.ReadConfigAsString( sqlParam, "." );
					paramArray.Add( p );
				}

				var paramObjs = (object[])paramArray.ToArray(typeof(object));
				return string.Format( rawSqlQuery, paramObjs );
			}

            return rawSqlQuery;
		}

	    public void Execute(Context context)
	    {
            context.LogInfo("Using database connection string: {0}", _connectionString);
	        var sqlQueryToExecute = _sqlQuery.GetFormattedSqlQuery();
            context.LogInfo("Executing database query: {0}", sqlQueryToExecute);

            // Sleep for delay seconds...
            System.Threading.Thread.Sleep(_delayBeforeCheck * 1000);


            var ds = FillDataSet(_connectionString, sqlQueryToExecute);
	        
            if(_numberOfRowsExpected != ds.Tables[0].Rows.Count)
            {
                throw new ApplicationException(string.Format("Number of rows expected to be returned by the query does not match the value specified in the teststep. Number of rows the NnumberOfRowsExpected were: {0}, actual: {1}", _numberOfRowsExpected, ds.Tables[0].Rows.Count));
            }

            context.LogInfo("NumberOfRowsExpected: {0}, actual number returned: {1}", _numberOfRowsExpected, ds.Tables[0].Rows.Count);

            if (0 == _numberOfRowsExpected)
            {
                return;
            }

            if (null != DBRowsToValidate.RowsToValidate)
            {
                foreach (var row in DBRowsToValidate.RowsToValidate)
                {
                    DataRow bamDbRow;

                    // Get the element which has the unique flag on...
                    var uniqueCell = row.UniqueCell;
                    if (null != uniqueCell)
                    {
                        var bamDbRowArray =
                            ds.Tables[0].Select(
                                string.Format("{0} = '{1}'", uniqueCell.ColumnName, uniqueCell.ExpectedValue));
                        bamDbRow = bamDbRowArray[0];
                    }
                    else
                    {
                        bamDbRow = ds.Tables[0].Rows[0];
                    }

                    var cells = row.Cells;
                    foreach (var cell in cells)
                    {
                        var dbData = bamDbRow[cell.ColumnName];
                        var dbDataStringValue = "";
                        if (0 == ValidateData(dbData, cell.ExpectedValue, ref dbDataStringValue))
                        {
                            context.LogInfo("Validation succeeded for field: {0} equals: {1}", cell.ColumnName,
                                            dbDataStringValue);
                        }
                        else
                        {
                            throw new Exception(
                                String.Format(
                                    "Validation failed for field: {0}. Expected value: {1}, actual value: {2}",
                                    cell.ColumnName, cell.ExpectedValue, dbDataStringValue));
                        }
                    }
                }
            }
	    }

        public void Validate(Context context)
	    {
            // delayBeforeCheck - optional

            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new ArgumentNullException("ConnectionString is either null or of zero length");
            }
            _connectionString = context.SubstituteWildCards(_connectionString);

            if (null == _sqlQuery)
            {
                throw new ArgumentNullException("ConnectionString is either null or of zero length");
            }

            _sqlQuery.Validate(context);
        }
	}

    public class DBRowsToValidate
    {
        private IList<DBRowToValidate> _rowsToValidate = new List<DBRowToValidate>();

        public DBRowsToValidate() {}

        public DBRowsToValidate(object[] validationArgs)
        {
            ArgumentValidation.CheckForNullReference(validationArgs, "validationArgs");
            if (validationArgs.Length == 0)
            {
                throw new ArgumentException(
                    "The array objParams must be contain at least two objects, i.e. a column name and an expected value");
            }

            var drtv = new DBRowToValidate();
            for (int c = 0; c < validationArgs.Length; c += 2 )
            {
                drtv.AddCell(new DBCellToValidate((string)validationArgs[c], (string)validationArgs[c + 1]));    
            }
            _rowsToValidate.Add(drtv);
        }

        public void AddRow(DBRowToValidate dbRowToValidate)
        {
            _rowsToValidate.Add(dbRowToValidate);
        }

        public IList<DBRowToValidate> RowsToValidate
        {
            set { _rowsToValidate = value; }
            get { return _rowsToValidate; }
        }
    }

    public class DBRowToValidate
    {
        private readonly IList<DBCellToValidate> _cells = new List<DBCellToValidate>();
        private readonly DBCellToValidate _uniqueCell;

        public DBRowToValidate() {}

        public DBRowToValidate(DBCellToValidate uniqueCell)
        {
            _uniqueCell = uniqueCell;
        }

        public DBCellToValidate UniqueCell
        {
            get
            {
                return _uniqueCell;
            }
        }

        public void AddCell(DBCellToValidate cell)
        {
            ArgumentValidation.CheckForNullReference(cell, "cell");

            _cells.Add(cell);
        }

        public void AddUniqueCell(DBCellToValidate cell)
        {
            ArgumentValidation.CheckForNullReference(cell, "cell");

            _cells.Add(cell);
        }

        public IList<DBCellToValidate> Cells
        {
            get
            {
                return _cells;
            }
        }
    }

    public class DBCellToValidate
    {
        private readonly string _columnName;
        private readonly string _expectedValue;

        public DBCellToValidate(string columnName, string expectedValue)
        {
            ArgumentValidation.CheckForEmptyString(columnName, "columnName");
            ArgumentValidation.CheckForEmptyString(expectedValue, "expectedValue");

            _columnName = columnName;
            _expectedValue = expectedValue;
        }

        public string ColumnName
        {
            get { return _columnName; }
        }

        public string ExpectedValue
        {
            get { return _expectedValue; }
        }
    }

    public class DBRowsToValidateParamFormatter : ITestStepParameterFormatter
    {
        public object[] FormatParameters(Type type, object[] args, Context ctx)
        {
            object[] retVal;

            if (typeof(DBRowsToValidate) == type)
            {
                object[] argsFetchedFromCtx = new object[args.Length];
                int c = 0;
                foreach (object arg in args)
                {
                    argsFetchedFromCtx[c++] = ctx.ReadArgument(arg);
                }

                retVal = new object[1];
                retVal[0] = new DBRowsToValidate(argsFetchedFromCtx);
            }
            else
            {
                throw new ApplicationException(
                    string.Format("The type {0} is not supported in DBRowsToValidateParamFormatter", type.FullName));
            }

            return retVal;
        }
    }
}
