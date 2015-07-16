//---------------------------------------------------------------------
// File: DBQueryReturnXmlStep.cs
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
	using System.IO;
	using System.Collections;
	using System.Xml;
	using System.Data;
	using System.Data.SqlClient;

	/// <summary>
	/// The DBQueryReturnXmlStep queries a database and returns the response in Xml. It incorporates the context loader and 
	/// validation sub steps and is useful for testing scenarios where BizTalk communicates with SQL server via the SQL adapter
	/// </summary>
	/// 
	/// <remarks>
	/// The following shows an example of the Xml representation of this test step.
	/// 
	/// <code escaped="true">
	///	<TestStep assemblyPath="" typeName="BizUnit.DBQueryReturnXmlStep">
	///		<DelayBeforeCheck>1</DelayBeforeCheck>
	///		<ConnectionString>Persist Security Info=False;Integrated Security=SSPI;database=DBname;server=(local);Connect Timeout=30</ConnectionString>
	///		<RootElement>RootNodeName</RootElement>
	///		<AllowEmpty>true|false</AllowEmpty>
	///		<!-- 
	///		The SQL Query to execute is built by formatting the RawSQLQuery substituting in the 
	///		SQLQueryParam's
	///		-->
	///		<SQLQuery>
	///			<!-- The query that returns Xml (probably using For Xml) -->
	///			<RawSQLQuery>Exec stordeprocedure</RawSQLQuery>
	///			<SQLQueryParams>
	///				<SQLQueryParam takeFromCtx="EventId"></SQLQueryParam>
	///			</SQLQueryParams>
	///		</SQLQuery>
	///		
	///		<!-- The Context loader will act upon the xml returned from the above Sql Statement  -->
	///		<ContextLoaderStep assemblyPath="" typeName="BizUnit.XmlContextLoader">
	///			<XPath contextKey="HTTP_Url">/def:html/def:body/def:p[2]/def:form</XPath>
	///			<XPath contextKey="ActionID">/def:html/def:body/def:p[2]/def:form/def:input[3]</XPath>
	///			<XPath contextKey="ActionType">/def:html/def:body/def:p[2]/def:form/def:input[4]</XPath>
	///			<XPath contextKey="HoldEvent">/def:html/def:body/def:p[2]/def:form/def:input[2]</XPath>
	///		</ContextLoaderStep>
	///
	///	    <!-- Note: Validation step could be any generic validation step -->	
	///		<ValidationStep assemblyPath="" typeName="BizUnit.XmlValidationStep">
	///			<XmlSchemaPath>.\TestData\PurchaseOrder.xsd</XmlSchemaPath>
	///			<XmlSchemaNameSpace>http://SendMail.PurchaseOrder</XmlSchemaNameSpace>
	///			<XPathList>
	///				<XPathValidation query="/*[local-name()='PurchaseOrder' and namespace-uri()='http://SendMail.PurchaseOrder']/*[local-name()='PONumber' and namespace-uri()='']">PONumber_0</XPathValidation>
	///			</XPathList>
	///		</ValidationStep>			
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
    ///			<term>RootElement</term>
	///			<description>The name of the element which will serve as the root element for the returned Xml</description>
	///		</item>
	///		<item>
	///			<term>AllowEmpty</term>
	///			<description>Boolean value specifying whether an error should be raised if no Xml is returned. Specify true to raise an error</description>
	///		</item>
	///		<item>
	///			<term>SQLQuery/RawSQLQuery</term>
	///			<description>The raw SQL string that will be formatted by substituting in the SQLQueryParam</description>
	///		</item>
	///		<item>
	///			<term>SQLQuery/SQLQueryParams/SQLQueryParam</term>
	///			<description>The parameters to substitute into RawSQLQuery <para>(repeating)</para></description>
	///		</item>
	///	</list>
	///	</remarks>
    [Obsolete("DBQueryReturnXmlStep has been deprecated. Investigate the BizUnit.TestSteps namespace.")]
	public class DBQueryReturnXmlStep : ITestStep
	{
		/// <summary>
		/// ITestStep.Execute() implementation
		/// </summary>
		/// <param name='testConfig'>The Xml fragment containing the configuration for this test step</param>
		/// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>		
        public void Execute(XmlNode testConfig, Context context)
		{
			int delayBeforeCheck = context.ReadConfigAsInt32( testConfig, "DelayBeforeCheck" );			
			string connectionString = context.ReadConfigAsString( testConfig, "ConnectionString" );
			string rootElement = context.ReadConfigAsString( testConfig, "RootElement" );
			bool allowEmpty = context.ReadConfigAsBool ( testConfig, "AllowEmpty" );
			XmlNode queryConfig = testConfig.SelectSingleNode( "SQLQuery" );
			string sqlQuery = BuildSqlQuery( queryConfig, context );
			XmlNode validationConfig = testConfig.SelectSingleNode("ValidationStep");
			XmlNode contextConfig = testConfig.SelectSingleNode("ContextLoaderStep");

			context.LogInfo("Using database connection string: {0}", connectionString);
			context.LogInfo("Executing database query : {0}", sqlQuery );

			// Sleep for delay seconds...
			System.Threading.Thread.Sleep(delayBeforeCheck*1000);
			
			string xml = GetXmlData(connectionString, sqlQuery);

			context.LogInfo("Xml returned : {0}", xml );

			if(xml != null && xml.Trim().Length > 0)
			{				 
				//prepare to execute context loader
				byte [] buffer = System.Text.Encoding.ASCII.GetBytes("<" + rootElement +">" + xml + "</" + rootElement +  ">");
				MemoryStream data = null;

				try
				{
					data = new MemoryStream(buffer);

					data.Seek(0, SeekOrigin.Begin);
					context.ExecuteContextLoader( data, contextConfig );

					data.Seek(0, SeekOrigin.Begin);
					context.ExecuteValidator( data, validationConfig );
				}
				finally
				{
					if ( null != data )
					{
						data.Close();
					}
				}
			}
            else if (!allowEmpty && (xml == null || xml.Trim().Length > 0))
            {
                throw new Exception("Response was expected.No Xml returned.");
            }
            else if (allowEmpty)
            {
                context.LogWarning("No Xml was returned from the DB Query. AllowEmpty has been set to true and hence no error has been raised");
            }
		}

		private static string GetXmlData(string connectionString, string sqlQuery)
		{
			var connection = new SqlConnection(connectionString);

			using ( connection )
			{
                object obj; 
                
                try
				{
					connection.Open();
					var comm= new SqlCommand(sqlQuery, connection);
					obj = comm.ExecuteScalar ();
				}
				finally
				{
					if(connection.State == ConnectionState.Open)
						connection.Close();
				}

                if (obj != null)
                {
                    return obj.ToString();
                }

                return null;
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
					string p = context.ReadConfigAsString( sqlParam, "." );
					paramArray.Add( p );
				}

				var paramObjs = (object[])paramArray.ToArray(typeof(object));

				return string.Format( rawSqlQuery, paramObjs );
			}
			
			return rawSqlQuery;
		}
	}
}
