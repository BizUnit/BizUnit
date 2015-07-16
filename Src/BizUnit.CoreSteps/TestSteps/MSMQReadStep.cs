//---------------------------------------------------------------------
// File: MSMQReadStep.cs
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
using System.Reflection;
using BizUnit.CoreSteps.Utilities;

namespace BizUnit.CoreSteps.TestSteps
{
	/// <summary>
	/// The MSMQReadStep reads a message from an MSMQ queue and optionally validates the contents of it
	/// </summary>
	/// 
	/// <remarks>
	/// The following shows an example of the Xml representation of this test step.
	/// 
	/// <code escaped="true">
	///	<TestStep assemblyPath="" typeName="BizUnit.MSMQReadStep">
	///		<QueuePath>.\Private$\Test01</QueuePath>
	///		<Timeout>2000</Timeout>
	///		
	///		<ContextProperties>
	///			<ContextProperty MSMQProp="CorrelationId" CtxPropName="MSMQ_CorrelationId" />
	///			<ContextProperty MSMQProp="AppSpecific" CtxPropName="MSMQ_AppSpecific" />
	///			<ContextProperty MSMQProp="Label" CtxPropName="MSMQ_Label" />
	///		</ContextProperties>
	///		
	///		<ValidationStep assemblyPath="" typeName="BizUnit.BinaryValidation">
	///			<ComparisonDataPath>.\TestData\ResultDoc1.xml</ComparisonDataPath>
	///		</ValidationStep>
	///
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
	///			<description>The MSMQ queue to read a message from</description>
	///		</item>
	///		<item>
	///			<term>Timeout</term>
	///			<description>The timeout to wait for the message to appear in the queue, in milisecs</description>
	///		</item>
	///		<item>
	///			<term>ValidationStep</term>
	///			<description>The validation step that will be used to validate the contents of the message read from the queue (optional).</description>
	///		</item>
	///		<item>
	///			<term>ContextProperties/ContextProperty</term>
	///			<description>Allows properties from the MSMQ message to be written to the BizUnit context. 
	///			The MSMQProp attribute specifies the property on the MSMQ message, e.g. "CorrelationId", the 
	///			CtxPropName attribute specifies the name of the property to write the value of the MSMQ property to.
	///			<para>Note: All properties on System.Messaging.Message are supported.</para>
	///			<para>(Optional)(One or more)</para></description>
	///		</item>
	///	</list>
	///	</remarks>	
    [Obsolete("MSMQReadStep has been deprecated. Investigate the BizUnit.TestSteps namespace.")]
    public class MSMQReadStep : ITestStep
	{
		/// <summary>
		/// ITestStep.Execute() implementation
		/// </summary>
		/// <param name='testConfig'>The Xml fragment containing the configuration for this test step</param>
		/// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
		public void Execute(XmlNode testConfig, Context context)
		{
			MemoryStream msgData = null;

			try
			{
				// Read test config...
				string queuePath = context.ReadConfigAsString( testConfig, "QueuePath" );
				double timeout = context.ReadConfigAsDouble( testConfig, "Timeout" );
				XmlNode validationConfig = testConfig.SelectSingleNode("ValidationStep");
				XmlNodeList ctxProps = testConfig.SelectNodes("ContextProperties/*");

				context.LogInfo("MSMQReadStep about to read data from queue: {0}", queuePath );

				var queue = new MessageQueue(queuePath);

				// Receive msg from queue...
                Message msg = queue.Receive(TimeSpan.FromMilliseconds(timeout), MessageQueueTransactionType.Single);

				// Dump msg content to console...
				msgData = StreamHelper.LoadMemoryStream(msg.BodyStream );
				StreamHelper.WriteStreamToConsole("MSMQ message data", msgData, context);

				// Validate data...
				try
				{
					msgData.Seek(0, SeekOrigin.Begin);
					context.ExecuteValidator( msgData, validationConfig );
				}
				catch(Exception e)
				{
					throw new ApplicationException("MSMQReadStep message data was not correct!", e);
				}
	
				if ( null != ctxProps && ctxProps.Count > 0 )
				{
					ProcessContextProperties( context, ctxProps, msg );
				}
			}
			finally
			{
				if ( null != msgData )
				{
					msgData.Close();
				}
			}
		}

        private static void ProcessContextProperties(Context context, XmlNodeList props, Message msg)
		{
			foreach( XmlNode prop in props )
			{
				// <ContextProperty MSMQProp="CorrelationId" CtxPropName="MSMQ_CorrelationId" />
				string ctxPropName = prop.SelectSingleNode("@CtxPropName").Value;
				string msmqPropName = prop.SelectSingleNode("@MSMQProp").Value;

				PropertyInfo pi = msg.GetType().GetProperty(msmqPropName);
				object val;
				try
				{
					val = pi.GetValue(msg, null);
				}
				catch(Exception)
				{
					context.LogInfo("The property: \"{0}\" did not have a value set", msmqPropName );
					continue;
				}

				context.LogInfo("Property: \"{0}\", Value: \"{1}\" written to context", msmqPropName, val );
				context.Add( ctxPropName, val );
			}
		}
	}
}
