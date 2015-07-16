//---------------------------------------------------------------------
// File: MSMQQueuePurgeStep.cs
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
	/// The MSMQQueuePurgeStep purges an MSMQ queue
	/// </summary>
	/// 
	/// <remarks>
	/// The following shows an example of the Xml representation of this test step.
	/// 
	/// <code escaped="true">
	///	<TestStep assemblyPath="" typeName="BizUnit.MSMQQueuePurgeStep">
	///		<QueuePath>.\Private$\Test01</QueuePath>
	///		<QueuePath>.\Private$\Test02</QueuePath>
	///	</TestStep>
	///	</code>
	///	
	///	<list type="table">
	///		<listheader>
	///			<term>Tag</term>
	///			<description>Description</description>
	///		</listheader>
	///		<item>
	///			<term>QueuePath</term>
	///			<description>The MSMQ queue to purge, multiple entries maybe specified</description>
	///		</item>
	///	</list>
	///	</remarks>	
    [Obsolete("MSMQQueuePurgeStep has been deprecated. Investigate the BizUnit.TestSteps namespace.")]
    public class MSMQQueuePurgeStep : ITestStep
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

				var q = new MessageQueue(queuePath);
				q.Purge();

				context.LogInfo( "MSMQQueuePurgeStep has purged the queue: {0}", queuePath );
			}
		}
	}
}
