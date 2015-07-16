//---------------------------------------------------------------------
// File: MQSeriesPutStep.cs
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
	using System.IO;

	/// <summary>
	/// The MQSeriesPutStep test step writes data to an MQ Series queue.
	/// </summary>
	/// 
	/// <remarks>
	/// Note: this test step requires amqmdnet.dll, this is present in MQ Patch Level > CSD07
	/// <para>Also, this test step is packaged in the assembly: BizUnit.MQSeriesSteps.dll</para>
	/// <para>The following shows an example of the Xml representation of this test step.</para>
	/// 
	/// <code escaped="true">
    ///	<TestStep assemblyPath="" typeName="BizUnit.MQSeriesSteps.MQSeriesPutStep, BizUnit.MQSeriesSteps", Version=3.1.0.0, Culture=neutral, PublicKeyToken=7eb7d82981ae5162">
	///		<QueueManager>QM_server</QueueManager>
	///		<Queue>QueueName</Queue>
	///		<SourcePath>.\TestData\InDoc1.txt</SourcePath>
	///	</TestStep>
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
	///			<term>SourcePath</term>
	///			<description>The location of the FILE containing the data that will be written to the queue.</description>
	///		</item>
	///	</list>
	///	</remarks>	
    [Obsolete("MQSeriesPutStep has been deprecated.")]
    public class MQSeriesPutStep : ITestStep
	{
		/// <summary>
		/// ITestStep.Execute() implementation
		/// </summary>
		/// <param name='testConfig'>The Xml fragment containing the configuration for this test step</param>
		/// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
		public void Execute(System.Xml.XmlNode testConfig, Context context)
		{
			string queueManager = testConfig.SelectSingleNode("QueueManager").InnerText;
			string queue = testConfig.SelectSingleNode("Queue").InnerText;
			string path = testConfig.SelectSingleNode("SourcePath").InnerText;

			var reader = new StreamReader(path);
			string testData = reader.ReadToEnd();

			context.LogData("MSMQ input message:", testData);

			MQSeriesHelper.WriteMessage(queueManager, queue, testData, context);
		}
	}
}
