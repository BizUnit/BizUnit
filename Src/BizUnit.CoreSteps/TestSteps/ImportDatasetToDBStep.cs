//---------------------------------------------------------------------
// File: BAMDeploymentStep.cs
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

namespace BizUnit.CoreSteps.TestSteps
{
	using System;
	using System.IO ;
	using System.Collections;
	using System.Xml;
	using System.Text;
	using System.Data;
	using System.Data.SqlClient ;

	///<summary>
	///ImportDatasetToDBStep imports data stored in a DataSet Xml file into the specified database. Use this step if you want to load
	///a SQL database with dummy data.  
	/// </summary>
	/// 
	///<remarks>
	/// <code escaped="true">
	/// <TestStep assemblyPath="" typeName="BizUnit.ImportDatasetToDBStep">
	///		<DelayBeforeExecution>0</DelayBeforeExecution> 
	///		<ConnectionString>Persist Security Info=False;Integrated Security=SSPI;database=DBname;server=(local);Connect Timeout=30</ConnectionString>
	///		<DatasetReadXmlSchemaPath>C:\DBDtableSchema.xml</DatasetReadXmlSchemaPath>
	///		<DatasetReadXmlPath>C:\DBDtable.xml</DatasetReadXmlPath> 
	///		<DelayBetweenRecordImports>1</DelayBetweenRecordImports> 
	/// </TestStep>
	/// </code>
	/// 
	///	<list type="table">
	///		<listheader>
	///			<term>Tag</term>
	///			<description>Description</description>
	///		</listheader>
	///		<item>
	///			<term>DelayBeforeExecution</term>
	///			<description>The number of seconds to wait before executing the step</description>
	///		</item>
	///		<item>
	///			<term>ConnectionString</term>
	///			<description>The connection string specifying the database to which the DataSet will be imported</description>
	///		</item>
	///		<item>
	///			<term>DatasetReadXmlSchemaPath</term>
	///			<description>File containing the Xml Schema of the table(s) in the DataSet</description>
	///		</item>
	///		<item>
	///			<term>DatasetReadXmlPath</term>
	///			<description>File containing the Xml representation of the data in the DataSet. For datetime columns, you have the option of specifying
	///			getdate() instead of the actual date value. ImportDatasetToDBStep will replace getdate() with the current datetime</description>
	///		</item>
	///		<item>
	///			<term>DelayBetweenRecordImports</term>
	///			<description>The number (in minutes) by which the getdate() specified in a datetime column should differ from the getdate() specified in the 
	///			same datetime column of another record. </description>
	///		</item>
	///	</list>
	/// </remarks>
    [Obsolete("ImportDatasetToDBStep has been deprecated. Investigate the BizUnit.TestSteps namespace.")]
    public class ImportDatasetToDBStep: ITestStep 
	{
		private Hashtable _sqlServerDbType;

		private void LoadTypeHashTable()
		{
			_sqlServerDbType = new Hashtable();
			_sqlServerDbType.Add(typeof(bool).FullName,SqlDbType.Bit);
			_sqlServerDbType.Add(typeof(int).FullName,SqlDbType.Int);
			_sqlServerDbType.Add(typeof(string).FullName,SqlDbType.VarChar);
			_sqlServerDbType.Add(typeof(DateTime).FullName,SqlDbType.DateTime);
			_sqlServerDbType.Add(typeof(System.Int16 ).FullName,SqlDbType.SmallInt );
		}

        /// <summary>
        /// ITestStep.Execute() implementation
        /// </summary>
        /// <param name='testConfig'>The Xml fragment containing the configuration for this test step</param>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public void Execute(XmlNode testConfig, Context context)
		{
			LoadTypeHashTable();
			int delayBeforeCheck = context.ReadConfigAsInt32( testConfig, "DelayBeforeExecution" );			
			string connectionString = context.ReadConfigAsString( testConfig, "ConnectionString" );
			string datasetReadXmlSchemaPath = context.ReadConfigAsString(testConfig, "DatasetReadXmlSchemaPath");
			string datasetReadXmlPath = context.ReadConfigAsString(testConfig, "DatasetReadXmlPath");
			double delayBetweenRecordImport = testConfig.InnerXml.IndexOf("DelayBetweenRecordImports",0,testConfig.InnerXml.Length) != -1? context.ReadConfigAsInt32 (testConfig, "DelayBetweenRecordImports") : 0;

			// Sleep for delay seconds...
			System.Threading.Thread.Sleep(delayBeforeCheck*1000);

			ImportDatasetDataIntoDBTable(connectionString,GetDataSet(datasetReadXmlSchemaPath,datasetReadXmlPath, delayBetweenRecordImport) );
		}

		private static DataSet GetDataSet(string datasetSchemaFile, string datasetDataFile, double delayBetweenRecordImport)
		{
			//replace the getdate() function in the time place with the current date
			DateTime dt = DateTime.Now ;
			var sr = new StreamReader(datasetDataFile);
			string fileContents = sr.ReadToEnd();
			sr.Close();

			int startIndex = 0;
			bool matchExists = true;
			var htGetDateIndices = new Hashtable();

			while(matchExists)
			{
				int index = fileContents.IndexOf("getdate()",startIndex, fileContents.Length - startIndex);
                if (index == -1)
                {
                    matchExists = false;
                }
                else
                {
                    dt = dt.AddMinutes(delayBetweenRecordImport);
                    string pattern = index + "getdate()" + dt;
                    htGetDateIndices.Add(pattern, dt.GetDateTimeFormats('s')[0]);
                    fileContents = fileContents.Remove(index, 9); //9 is the length of the string "getdate()"
                    fileContents = fileContents.Insert(index, pattern);
                    startIndex = index + 9; //move few positions forward to being the search again. 9 is a logical fit as getdate() is 9 characters
                }
			}

			//for each found key, simply replace the corresponding match in filecontents
            foreach (string getDateIndex in htGetDateIndices.Keys)
            {
                fileContents = fileContents.Replace(getDateIndex, htGetDateIndices[getDateIndex].ToString());
            }
			
			var stringReader = new StringReader(fileContents);
			var ds = new DataSet("DataStore");
			ds.ReadXmlSchema(datasetSchemaFile);
			ds.ReadXml(stringReader);

			stringReader.Close();
			return ds;
		}

		private void ImportDatasetDataIntoDBTable(string connectionString,  DataSet dsToImport)
		{
			// Add all the rows from dsToImport to a new table so that we get a 
			// row status of added for each of the rows. 
			// Note - see if there is a more easier technique to achieve this (perhaps a property setting)
			var ds = new DataSet();			
			
			foreach(DataTable dtImport in dsToImport.Tables)
			{
				var dt = new DataTable(dtImport.TableName);

                foreach (DataColumn column in dtImport.Columns)
                {
                    dt.Columns.Add(new DataColumn(column.ColumnName, column.DataType));
                }

				for(int rowCount=0; rowCount < dtImport.Rows.Count; rowCount++)
				{
					DataRow dr = dt.NewRow(); 
					dr.ItemArray = dtImport.Rows[rowCount].ItemArray;
					dt.Rows.Add(dr);
				}

				ds.Tables.Add(dt.Copy());

				dt.Dispose();
			}

			//prepare the query and load the data adapter
			foreach(DataTable dt in ds.Tables)
			{
				//form the Insert command
				var columnsList =  new StringBuilder();
				string query = "INSERT INTO " + dt.TableName + " " ;
				var command = new SqlCommand();

				foreach(DataColumn dc in dt.Columns)
				{
					if(dc.AutoIncrement == false && dc.ReadOnly == false) //if the call is set to auto-increment or it is read-only (perhaps a computed column)
					{
						var param = new SqlParameter("@"+dc.ColumnName,(SqlDbType)_sqlServerDbType[dc.DataType.FullName])
						                {SourceColumn = dc.ColumnName};
					    command.Parameters.Add(param);
						columnsList.Append(",@"+dc.ColumnName);
					}
				}
				
				//Remove the first comma
				columnsList.Remove(0,1);
				command.CommandText   = query + "(" + columnsList.ToString().Replace('@',' ') + ") VALUES (" + columnsList + ") " ;

				var connection = new SqlConnection(connectionString);
				using ( connection )
				{
					try
					{
						var sqlda = new SqlDataAdapter {InsertCommand = command};
					    sqlda.InsertCommand.Connection = connection;
						sqlda.Update(ds,dt.TableName);
					}
					finally
					{
                        if (connection.State == ConnectionState.Open)
                        {
                            connection.Close();
                        }
					}
				}
			}
		}
	}
}
