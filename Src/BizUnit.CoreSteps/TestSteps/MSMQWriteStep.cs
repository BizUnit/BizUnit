//---------------------------------------------------------------------
// File: MSMQWriteStep.cs
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
using System.Messaging;
using BizUnit.CoreSteps.Utilities;

namespace BizUnit.CoreSteps.TestSteps
{
	/// <summary>
	/// The MSMQWriteStep writes a message to an MSMQ queue and optionally validates the contents of it
	/// </summary>
	/// 
	/// <remarks>
	/// The following shows an example of the Xml representation of this test step.
	/// 
	/// <code escaped="true">
	///	<TestStep assemblyPath="" typeName="BizUnit.MSMQWriteStep">
	///		<SourcePath>.\TestData\InDoc1.xml</SourcePath>
	///		<QueuePath>.\Private$\Test01</QueuePath>
	///		<MessageLabel>MSMQ_To_MSMQ_Test_Test_01</MessageLabel>
	///		<CorrelationId>1234</CorrelationId>
	///		<AppSpecific>5678</AppSpecific>
    ///		<UseTransactions>true</UseTransactions>
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
	///			<description>The FILE containing the data to be written to an MSMQ queue</description>
	///		</item>
	///		<item>
	///			<term>QueuePath</term>
	///			<description>The MSMQ queue to write a new message to</description>
	///		</item>
	///		<item>
	///			<term>MessageLabel</term>
	///			<description>The MSMQ label to associate with the new message</description>
	///		</item>
	///		<item>
	///			<term>CorrelationId</term>
	///			<description>The MSMQ CorrelationId to associate with the new message</description>
	///		</item>
	///		<item>
	///			<term>AppSpecific</term>
	///			<description>The MSMQ AppSpecific property to associate with the new message (int32)</description>
	///		</item>
    ///		<item>
    ///			<term>UseTransactions</term>
    ///			<description>Defaults to true, when using transactions the message will be written to the queue using MessageQueueTransactionType.Single, if set to false MessageQueueTransactionType.None will be used (optional)</description>
    ///		</item>
    ///	</list>
	///	</remarks>	
    [Obsolete("MSMQWriteStep has been deprecated. Investigate the BizUnit.TestSteps namespace.")]
	public class MSMQWriteStep : ITestStep
	{
	    private MessageQueueTransactionType _transactionType = MessageQueueTransactionType.Single;

		/// <summary>
		/// ITestStep.Execute() implementation
		/// </summary>
		/// <param name='testConfig'>The Xml fragment containing the configuration for this test step</param>
		/// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
		public void Execute(XmlNode testConfig, Context context)
		{			
			MemoryStream msgStr = null;

			try
			{
			    // read test config...
			    string sourcePath = context.ReadConfigAsString(testConfig, "SourcePath");
			    string queuePath = context.ReadConfigAsString(testConfig, "QueuePath");
			    string messageLabel = context.ReadConfigAsString(testConfig, "MessageLabel");
			    string correlationId = context.ReadConfigAsString(testConfig, "CorrelationId", true);
			    int appSpecific = context.ReadConfigAsInt32(testConfig, "AppSpecific", true);
			    object objUseTransactions = context.ReadConfigAsObject(testConfig, "UseTransactions", true);

			    if (null != objUseTransactions)
			    {
			        if (!Convert.ToBoolean(objUseTransactions))
			        {
                        _transactionType = MessageQueueTransactionType.None;
			        }
			    }
			
		        context.LogInfo("MSMQWriteStep about to write data from File: {0} to the queue: {1}", sourcePath, queuePath );

				var queue = new MessageQueue(queuePath);
				var msg = new Message();

				msgStr = StreamHelper.LoadFileToStream(sourcePath);
				msg.BodyStream = msgStr;
				msg.UseDeadLetterQueue = true;
	
				if ( null != correlationId )
				{
					msg.CorrelationId = correlationId;
				}
				msg.AppSpecific = appSpecific;

                queue.Send(msg, messageLabel, _transactionType);
			}
			finally
			{
				if ( null != msgStr )
				{
					msgStr.Close();
				}
			}
		}
	}
}
