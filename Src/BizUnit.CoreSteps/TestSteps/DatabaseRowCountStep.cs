//---------------------------------------------------------------------
// File: DatabaseRowCountStep.cs
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
using System.Xml;
using BizUnit.CoreSteps.Utilities;

namespace BizUnit.CoreSteps.TestSteps
{
	/// <summary>
    /// The DatabaseRowCountStep step uses the supplied connection string, table name and condition to determine the number of rows in the database
    /// and validates this against the exepcted rows.
    /// </summary>
    /// 
    /// <remarks>
    /// The following shows an example of the Xml representation of this test step.
    /// 
    /// <code escaped="true">
    ///	<TestStep assemblyPath="" typeName="BizUnit.DatabaseRowCountStep">
    ///		<ConnectionString>Server=localhost;Database=database;Trusted_Connection=True;</ConnectionString>
    ///		<Table>table</Table>
    ///		<Condition>id=1</Condition>
    ///		<ExpectedRows>100</ExpectedRows>
    ///	</TestStep>
    ///	</code>
    ///	
    ///	<list type="table">
    ///		<listheader>
    ///			<term>Tag</term>
    ///			<description>Description</description>
    ///		</listheader>
    ///		<item>
    ///			<term>ConnectionString</term>
    ///			<description>Database connection string</description>
    ///		</item>
    ///		<item>
    ///			<term>Table</term>
    ///			<description>Table name select rows from</description>
    ///		</item>
    ///		<item>
    ///			<term>Condition</term>
    ///			<description>Condition to use</description>
    ///		</item>
    ///		<item>
    ///			<term>ExpectedRows</term>
    ///			<description>The number of expected rows</description>
    ///		</item>
    ///	</list>
    ///	</remarks>
    [Obsolete("DatabaseRowCountStep has been deprecated. Investigate the BizUnit.TestSteps namespace.")]
    public class DatabaseRowCountStep : ITestStep
    {
		/// <summary>
		/// ITestStep.Execute() implementation
		/// </summary>
		/// <param name='testConfig'>The Xml fragment containing the configuration for this test step</param>
		/// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
		public void Execute(XmlNode testConfig, Context context)
        {
            string connectionString = context.ReadConfigAsString( testConfig, "ConnectionString" );
            string table = context.ReadConfigAsString( testConfig, "Table" );
            string condition = context.ReadConfigAsString( testConfig, "Condition" );
            int expectedRows = context.ReadConfigAsInt32( testConfig, "ExpectedRows" );

            // Build SQL statement
            string sqlStatement = "select count(*) from " + table + " where " + condition ;

            context.LogInfo( "DatabaseRowCountStep connecting to {0}, executing statement {1}", connectionString, sqlStatement ) ;

            // Execute command against specified database
            int rows = DatabaseHelper.ExecuteScalar( connectionString, sqlStatement ) ;

            // Number of rows as expected?
            if ( rows != expectedRows )
            {
                throw new ApplicationException( string.Format( "DatabaseRowCountStep failed, expected {0} rows but found {1} rows", expectedRows, rows ) ) ;
            }
                
            context.LogInfo( "DatabaseRowCountStep found \"{0}\" rows", rows ) ;
        }
	}
}
