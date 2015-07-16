//---------------------------------------------------------------------
// File: MSMQCreateQueueStep.cs
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

namespace BizUnit.CoreSteps.TestSteps
{
	using System.Xml;
	using System.Messaging;

	/// <summary>
	/// The MSMQCreateQueueStep creates one or more new MSMQ queues
	/// </summary>
	/// 
	/// <remarks>
	/// The following shows an example of the Xml representation of this test step.
	/// 
	/// <code escaped="true">
	///	<TestStep assemblyPath="" typeName="BizUnit.MSMQCreateQueueStep">
	///		<QueuePath transactional="true">.\Private$\Test01</QueuePath>
	///		<QueuePath transactional="true">.\Private$\Test02</QueuePath>
	///	</TestStep>
	///	</code>
	///	
	///	<list type="table">
	///		<listheader>
	///			<term>Tag</term>
	///			<description>Description</description>
	///		</listheader>
	///		<item>
	///			<term>QueueName</term>
	///			<description>The name of the MSMQ queue to create, e.g. .\Private$\Test02 <para>(one or more)</para></description>
	///		</item>
	///		<item>
	///			<term>QueueName/@transactional</term>
	///			<description>If true, the queue created will be transactional</description>
	///		</item>
	///	</list>
	///	</remarks>	
    [Obsolete("MSMQCreateQueueStep has been deprecated. Investigate the BizUnit.TestSteps namespace.")]
    public class MSMQCreateQueueStep : ITestStep
	{
		/// <summary>
		/// ITestStep.Execute() implementation
		/// </summary>
		/// <param name='testConfig'>The Xml fragment containing the configuration for this test step</param>
		/// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
		public void Execute(XmlNode testConfig, Context context)
		{
			XmlNodeList queues = testConfig.SelectNodes( "*" );

			foreach( XmlNode queue in queues)
			{
				string queuePath = queue.InnerText;
				bool transactional = context.ReadConfigAsBool(testConfig, "QueuePath/@transactional");

				MessageQueue.Create(queuePath, transactional);

				context.LogInfo( "The queue: \"{0}\" was created successfully.", queuePath );
			}
		}
	}
}
