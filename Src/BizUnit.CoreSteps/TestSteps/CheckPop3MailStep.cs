//---------------------------------------------------------------------
// File: CheckPop3MailStep.cs
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
using System.Collections;
using System.Xml;
using BizUnit.CoreSteps.Utilities.Pop3;

namespace BizUnit.CoreSteps.TestSteps
{
	/// <summary>
	/// The CheckPop3MailStep reads email from a POP3 server.
	/// </summary>
	/// 
	/// <remarks>
	/// The following shows an example of the Xml representation of this test step.
	/// 
	/// <code escaped="true">
	/// <TestStep assemblyPath="" typeName="BizUnit.CheckPop3MailStep">
	///		<DelayBeforeCheck>0</DelayBeforeCheck> <!-- Optional, seconds to delay performing check -->
	///		<Server>localhost</Server>
	///		<User>david.regan@virtualbank.com</User>
	///		<Password>Password</Password>
	///		<From>KYC@virtualbank.com</From>
	///		<Subject>My task cancelled</Subject>
	///		<ShowBody>true|false</ShowBody> <!-- default false -->
	/// </TestStep>
	/// </code>
	/// 
	///	<list type="table">
	///		<listheader>
	///			<term>Tag</term>
	///			<description>Description</description>
	///		</listheader>
	///		<item>
	///			<term>DelayBeforeCheck</term>
	///			<description>The delay before checking the POP3 server (optional)</description>
	///		</item>
	///		<item>
	///			<term>Server</term>
	///			<description>The POP3 server</description>
	///		</item>
	///		<item>
	///			<term>User</term>
	///			<description>The user name</description>
	///		</item>
	///		<item>
	///			<term>Password</term>
	///			<description>The password for the user name</description>
	///		</item>
	///		<item>
	///			<term>From</term>
	///			<description>The Tracking directory for BizTalk Server</description>
	///		</item>
	///		<item>
	///			<term>Subject</term>
	///			<description>The subject of the email to read</description>
	///		</item>
	///		<item>
	///			<term>ShowBody</term>
	///			<description>true|false <para>(optional, default = false)</para></description>
	///		</item>
	///	</list>
	///	</remarks>
    [Obsolete("CheckPop3MailStep has been deprecated. Investigate the BizUnit.TestSteps namespace.")]
	public class CheckPop3MailStep : ITestStep
	{
		/// <summary>
		/// Execute() implementation
		/// </summary>
		/// <param name='testConfig'>The Xml fragment containing the configuration for this test step</param>
		/// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
		public void Execute(XmlNode testConfig, Context context)
		{
			int delayBeforeCheck = context.ReadConfigAsInt32(testConfig, "DelayBeforeCheck", true);
			string server = context.ReadConfigAsString(testConfig, "Server");
			string user = context.ReadConfigAsString(testConfig, "User");
			string password = context.ReadConfigAsString(testConfig, "Password");
			string from = null;
			bool showBody = false;
			string subject = null;
			int attachments = -1;
			bool found = false;

			if (testConfig.SelectSingleNode("ShowBody") != null)
			{
				showBody = context.ReadConfigAsBool(testConfig, "ShowBody");
			}

			if (testConfig.SelectSingleNode("From") != null)
			{
				from = context.ReadConfigAsString(testConfig, "From");
			}

			if (testConfig.SelectSingleNode("Subject") != null)
			{
				subject = context.ReadConfigAsString(testConfig, "Subject");
			}

			if (testConfig.SelectSingleNode("Attachments") != null)
			{
				attachments = context.ReadConfigAsInt32(testConfig, "Attachments");
			}

			if ( delayBeforeCheck > 0 )
			{
				context.LogInfo("Waiting for {0} seconds before checking the mail.", delayBeforeCheck);
				System.Threading.Thread.Sleep(delayBeforeCheck*1000);
			}			

			var email = new Pop3Client(user, password, server);
			email.OpenInbox();

			try
			{
				while( email.NextEmail())
				{
					if (email.To == user && (email.From == from || from == null) && (email.Subject == subject || subject == null))
					{
						if (attachments > 0 && email.IsMultipart)
						{
							int a = 0;
							IEnumerator enumerator = email.MultipartEnumerator;
							while(enumerator.MoveNext())
							{
								var multipart = (Pop3Component)
									enumerator.Current;
								if( multipart.IsBody )
								{
									if (showBody)
									{
										context.LogData("Multipart body", multipart.Data);
									}
								}
								else
								{
									context.LogData("Attachment name", multipart.Name);
									a++;
								}
							}
							if (attachments == a)
							{
								found = true;
								break;
							}
						}
						else
						{
							if (showBody)
							{
								context.LogData("Single body", email.Body);
							}
							found = true;
							break;
						}							
					}
				}


				if (!found)
				{
					throw new Exception("Failed to find email message");
				}
				else
				{
					email.DeleteEmail();
				}
			}
			finally
			{
				email.CloseConnection();		
			}
		}
	}
}
