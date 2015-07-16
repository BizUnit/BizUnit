//---------------------------------------------------------------------
// File: HttpPostStep.cs
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
	/// The HttpPostStep is used to post a one way HTTP request.
	/// </summary>
	/// 
	/// <remarks>
	/// The following shows an example of the Xml representation of this test step.
	/// 
	/// <code escaped="true">
	///	<TestStep assemblyPath="" typeName="BizUnit.HttpPostStep">
	///		<SourcePath>.\TestData\InDoc1.xml</SourcePath>	
	///		<DestinationUrl takeFromCtx="HTTPDest">http://localhost/TestFrameworkDemo/BTSHTTPReceive.dll</DestinationUrl>
	///		<RequestTimeout>60000</RequestTimeout>
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
	///	</list>
	///	</remarks>
    [Obsolete("HttpPostStep has been deprecated. Investigate the BizUnit.TestSteps namespace.")]
	public class HttpPostStep : ITestStep
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

				context.LogInfo("HttpRequestResponseStep about to post data from File: {0} to the Url: {1}", sourcePath, destinationUrl );

				// Get the data to post...
				request = StreamHelper.LoadFileToStream(sourcePath);
				byte[] data = request.GetBuffer();

				// Post the data...
				response = HttpHelper.SendRequestData(destinationUrl, data, requestTimeout, context);

				// Dump the respons to the console...
				StreamHelper.WriteStreamToConsole("HttpPostStep response data", response, context);
			}
			finally
			{
				if ( null != request )
				{
					request.Close();
				}

				if ( null != response )
				{
					response.Close();
				}
			}
		}
	}
}
