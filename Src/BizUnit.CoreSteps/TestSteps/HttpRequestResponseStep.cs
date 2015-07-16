//---------------------------------------------------------------------
// File: HttpRequestResponseStep.cs
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
using System.IO;
using System.Xml;
using BizUnit.CoreSteps.Utilities;

namespace BizUnit.CoreSteps.TestSteps
{
	/// <summary>
	/// The HttpRequestResponseStep is used to post a two way HTTP request-response.
	/// </summary>
	/// 
	/// <remarks>
	/// The following shows an example of the Xml representation of this test step.
	/// 
	/// <code escaped="true">
	///	<TestStep assemblyPath="" typeName="BizUnit.HttpRequestResponseStep">
	///		<SourcePath>.\TestData\InDoc1.xml</SourcePath>
	///		<DestinationUrl>http://localhost/TestFrameworkDemo/BTSHTTPReceive.dll?ReqResp</DestinationUrl>
	///		<RequestTimeout>10000</RequestTimeout>
	///
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
	///	<list type="table">
	///		<listheader>
	///			<term>Tag</term>
	///			<description>Description</description>
	///		</listheader>
	///		<item>
	///			<term>SourcePath</term>
	///			<description>The location of the data to be posted over HTTP</description>
	///		</item>
	///		<item>
	///			<term>DestinationUrl</term>
	///			<description>The Url which the data will be posted to</description>
	///		</item>
	///		<item>
	///			<term>RequestTimeout</term>
	///			<description>The length of time to wait to the HTTP return code</description>
	///		</item>
	///		<item>
	///			<term>ValidationStep</term>
	///			<description>Determines which validation test step to use, the HTTP response will be validated using this step<para>(Optional)</para></description>
	///		</item>
	///	</list>
	///	</remarks>
    [Obsolete("HttpRequestResponseStep has been deprecated. Investigate the BizUnit.TestSteps namespace.")]
	public class HttpRequestResponseStep : ITestStep
	{
		/// <summary>
		/// ITestStep.Execute() implementation
		/// </summary>
		/// <param name='testConfig'>The Xml fragment containing the configuration for this test step</param>
		/// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
		public void Execute(XmlNode testConfig, Context context)
		{
			MemoryStream request = null;
			MemoryStream response = null;

			try
			{
				// read test config...
				string sourcePath = context.ReadConfigAsString( testConfig, "SourcePath" );
				string destinationUrl = context.ReadConfigAsString( testConfig, "DestinationUrl" );
				int requestTimeout = context.ReadConfigAsInt32( testConfig, "RequestTimeout" );
				XmlNode validationConfig = testConfig.SelectSingleNode("ValidationStep");

				context.LogInfo("HttpRequestResponseStep about to post data from File: {0} to the Url: {1}", sourcePath, destinationUrl );

				// Get the data to post...
				request = StreamHelper.LoadFileToStream(sourcePath);
				byte[] data = request.GetBuffer();

				// Post the data...
				response = HttpHelper.SendRequestData(destinationUrl, data, requestTimeout, context);

				// Dump the respons to the console...
				StreamHelper.WriteStreamToConsole("HttpRequestResponseStep response data", response, context);

				// Validate the response...
				try
				{
					response.Seek(0, SeekOrigin.Begin);
					context.ExecuteValidator( response, validationConfig );
				}
				catch(Exception e)
				{
					throw new ApplicationException("HttpRequestResponseStep response stream was not correct!", e);
				}
			}
			finally
			{
				if ( null != response )
				{
					response.Close();
				}

				if ( null != request )
				{
					request.Close();
				}
			}
		}
	}
}
