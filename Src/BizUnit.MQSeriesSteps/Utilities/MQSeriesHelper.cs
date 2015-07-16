//---------------------------------------------------------------------
// File: MQSeriesHelper.cs
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

namespace BizUnit
{
	using System;
	using  System.Text;
	using IBM.WMQ; // $:\Program Files\IBM\WebSphere MQ\bin\amqmdnet.dll - Requires Patch Level > CSD07

	/// <summary>
	/// Summary description for MQSeriesHelper.
	/// </summary>
	public class MQSeriesHelper
	{
		/// <summary>
		/// Helper method to read a message from an MQ Series queue
		/// </summary>
		/// 
		/// <param name="queueManagerName">The name of the MQ Series queue manager</param>
		/// <param name="queueName">The name of the MQ Series queue to read from</param>
		/// <param name="waitDelay">The time to wait for the message to be read from the queue</param>
		/// <param name="context">The BizUnit context object which holds state and is passed between test steps</param>
		/// <returns>String containing the data from the MQ series message</returns>
		static public string ReadMessage(string queueManagerName, string queueName, int waitDelay, Context context)
		{
			byte[] msgID;

			return ReadMessage(queueManagerName, queueName, waitDelay, context, out msgID);
		}

		/// <summary>
		/// Helper method to read a message from an MQ Series queue
		/// </summary>
		/// 
		/// <param name="queueManagerName">The name of the MQ Series queue manager</param>
		/// <param name="queueName">The name of the MQ Series queue to read from</param>
		/// <param name="waitDelay">The time to wait for the message to be read from the queue</param>
		/// <param name="context">The BizUnit context object which holds state and is passed between test steps</param>
		/// <param name="msgID">[out] the MQ Series message ID</param>
		/// <returns>String containing the data from the MQ series message</returns>
		static public string ReadMessage(string queueManagerName, string queueName, int waitDelay, Context context, out byte[] msgID)
		{
			MQQueueManager queueManager = null;
			MQQueue receiveQueue = null;
			string message = null;

			try 
			{
				context.LogInfo("Opening queue manager: \"{0}\"", queueManagerName);
				queueManager = new MQQueueManager(queueManagerName);
				
				context.LogInfo("Opening queue: \"{0}\"", queueName);
				receiveQueue = queueManager.AccessQueue(queueName, MQC.MQOO_INPUT_AS_Q_DEF + MQC.MQOO_FAIL_IF_QUIESCING);
			
				MQMessage mqMsg = new MQMessage();
				MQGetMessageOptions mqMsgOpts = new MQGetMessageOptions();
				mqMsgOpts.WaitInterval = waitDelay*1000;  
				mqMsgOpts.Options = MQC.MQGMO_WAIT;

				context.LogInfo("Reading message from queue '{0}'.", queueName);

				receiveQueue.Get(mqMsg,mqMsgOpts);

				if(mqMsg.Format.CompareTo(MQC.MQFMT_STRING)==0)
				{
					mqMsg.Seek(0);
					message = System.Text.UTF8Encoding.UTF8.GetString(mqMsg.ReadBytes(mqMsg.MessageLength));
					msgID = mqMsg.MessageId;
				}
				else
				{
					throw new NotSupportedException(string.Format("Unsupported message format: '{0}' read from queue: {1}.", mqMsg.Format, queueName));
				}
			}
			finally
			{
				if (receiveQueue != null)
				{
					receiveQueue.Close();
				}

				if (queueManager != null)
				{
					queueManager.Close();
				}
			}

			return message;
		}

		/// <summary>
		/// Helper method to write a message to an MQ Series queue
		/// </summary>
		/// 
		/// <param name="queueManagerName">The name of the MQ Series queue manager</param>
		/// <param name="queueName">The name of the MQ Series queue to read from</param>
		/// <param name="message">The MQ Series queue</param>
		/// <param name="context">The BizUnit context object which holds state and is passed between test steps</param>
		static public void WriteMessage(string queueManagerName, string queueName, string message, Context context)
		{
			WriteMessage(queueManagerName, queueName, message, null, context);
		}

		/// <summary>
		/// Helper method to write a message to an MQ Series queue
		/// </summary>
		/// 
		/// <param name="queueManagerName">The name of the MQ Series queue manager</param>
		/// <param name="queueName">The name of the MQ Series queue to read from</param>
		/// <param name="message">The MQ Series queue</param>
		/// <param name="correlId">The correlation ID to be set on the new message</param>
		/// <param name="context">The BizUnit context object which holds state and is passed between test steps</param>
		static public void WriteMessage(string queueManagerName, string queueName, string message, byte[] correlId, Context context)
		{
			MQQueueManager queueManager = null;
			MQQueue sendQueue = null;
			MQMessage mqMessage;
			MQPutMessageOptions mqPutMsgOpts;
			
			try
			{
				context.LogInfo("Opening queue manager: \"{0}\"", queueManagerName);
				queueManager = new MQQueueManager(queueManagerName);

				context.LogInfo("Opening queue: '{0}'.", queueName);
				sendQueue = queueManager.AccessQueue(queueName, MQC.MQOO_OUTPUT + MQC.MQOO_FAIL_IF_QUIESCING );
				
				mqMessage = new MQMessage();
				byte[] data = ConvertToBytes(message);
				mqMessage.Write(data);
				mqMessage.Format = MQC.MQFMT_STRING;
				mqPutMsgOpts = new MQPutMessageOptions();

				context.LogInfo("Writing {0} byte message to queue '{1}'.", data.Length, queueName);

				if (correlId != null)
				{
					mqMessage.CorrelationId = correlId;
				}
				
				sendQueue.Put( mqMessage, mqPutMsgOpts );
			}
			finally
			{
				if (sendQueue != null)
				{
					sendQueue.Close();
				}

				if (queueManager != null)
				{
					queueManager.Close();
				}
			}
		}

		private static byte[] ConvertToBytes(string str)
		{
			byte[] data = null;
			if (null != str)
			{
				data = UTF8Encoding.UTF8.GetBytes(str);
			}

			return data;
		}
	}
}
