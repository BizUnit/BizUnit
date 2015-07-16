//---------------------------------------------------------------------
// File: MQSeriesGetStep.cs
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

namespace BizUnit.MQSeriesSteps
{
	using System.Xml;

	/// <summary>
	/// The MQSeriesGetStep test step reads data from a specified MQ Series queue.
	/// </summary>
	/// 
	/// <remarks>
	/// Note: this test step requires amqmdnet.dll, this is present in MQ Patch Level > CSD07
	/// <para>Also, this test step is packaged in the assembly: BizUnit.MQSeriesSteps.dll</para>
	/// <para>The following shows an example of the Xml representation of this test step.</para>
	/// 
	/// <code escaped="true">
    ///	<TestStep assemblyPath="" typeName="BizUnit.MQSeriesSteps.MQSeriesGetStep, BizUnit.MQSeriesSteps", Version=3.1.0.0, Culture=neutral, PublicKeyToken=7eb7d82981ae5162">
	/// 	<QueueManager>QM_amachine</QueueManager>
	/// 	<Queue>QUEUE_007</Queue>
	/// 	<WaitTimeout>30</WaitTimeout>		<!-- in seconds, -1 = wait forever -->
	/// 	
	///		<!-- Note: Validation step could be any generic validation step -->	
	///		<ValidationStep assemblyPath="" typeName="BizUnit.XmlValidationStep">
	///			<XmlSchemaPath>.\TestData\PurchaseOrder.xsd</XmlSchemaPath>
	///			<XmlSchemaNameSpace>http://SendMail.PurchaseOrder</XmlSchemaNameSpace>
	///			<XPathList>
	///				<XPathValidation query="/*[local-name()='PurchaseOrder' and namespace-uri()='http://SendMail.PurchaseOrder']/*[local-name()='PONumber' and namespace-uri()='']">PONumber_0</XPathValidation>
	///			</XPathList>
	///		</ValidationStep>			
	/// </TestStep>
	///	</code>
	///	
	///	<list type="table">
	///		<listheader>
	///			<term>Tag</term>
	///			<description>Description</description>
	///		</listheader>
	///		<item>
	///			<term>QueueManager</term>
	///			<description>The name of the MQ Series queue manager</description>
	///		</item>
	///		<item>
	///			<term>Queue</term>
	///			<description>The name of the MQ Series queue to read from.</description>
	///		</item>
	///		<item>
	///			<term>WaitTimeout</term>
	///			<description>The time to wait for the message, after which if the message is not found the test step will fail, in seconds. Use -1 to wait forever.</description>
	///		</item>
	///		<item>
	///			<term>ValidationStep</term>
	///			<description>The validation step to used to validate the data read from the queue. (optional)</description>
	///		</item>
	///	</list>
	///	</remarks>	
    [Obsolete("MQSeriesGetStep has been deprecated.")]
    public class MQSeriesGetStep : ITestStep
	{
		/// <summary>
		/// ITestStep.Execute() implementation
		/// </summary>
		/// <param name='testConfig'>The Xml fragment containing the configuration for this test step</param>
		/// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
		public void Execute(XmlNode testConfig, Context context)
		{
			string queueManager = context.ReadConfigAsString(testConfig, "QueueManager");
			string queue = context.ReadConfigAsString(testConfig, "Queue");
			int waitTimeout = context.ReadConfigAsInt32(testConfig, "WaitTimeout");
			XmlNode validationConfig = testConfig.SelectSingleNode("ValidationStep");

		    string message = MQSeriesHelper.ReadMessage(queueManager, queue, waitTimeout, context);

            context.LogData("MQSeries output message:", message);

            context.ExecuteValidator(StreamHelper.LoadMemoryStream(message), validationConfig);
		}
	}
}
