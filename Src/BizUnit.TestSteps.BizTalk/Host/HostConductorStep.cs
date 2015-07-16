//---------------------------------------------------------------------
// File: HostConductorStep.cs
// 
// Summary: 
//
//---------------------------------------------------------------------
// Copyright (c) 2004-2011, Kevin B. Smith. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------

using BizUnit.Xaml;

namespace BizUnit.TestSteps.BizTalk.Host
{
	using System;
	using System.Management;

    /// <summary>
	/// The HostConductorStep test step maybe used to start or stop a BizTalk host
	/// </summary>
	/// 
	/// <remarks>
	/// The following shows an example of the Xml representation of this test step.
	/// 
	/// <code escaped="true">
    ///	<TestStep assemblyPath="" typeName="BizUnit.BizTalkSteps.HostConductorStep, BizUnit.BizTalkSteps, Version=3.1.0.0, Culture=neutral, PublicKeyToken=7eb7d82981ae5162">
	///		<Action>start|stop</Action>
    ///		<HostInstanceName>BizTalkServerApplication</HostInstanceName>
    ///		<Server>RecvHost</Server>
    ///     <Logon>zeus\\administrator</Logon>
    ///     <PassWord>appollo*1</PassWord>
    ///     <GrantLogOnAsService>true</GrantLogOnAsService>
	///	</TestStep>
	///	</code>
	///	
	///	<list type="table">
	///		<listheader>
	///			<term>Tag</term>
	///			<description>Description</description>
	///		</listheader>
	///		<item>
	///			<term>HostInstanceName</term>
	///			<description>The name of the host instance to start|stop</description>
	///		</item>
	///		<item>
	///			<term>Action</term>
    ///			<description>A value of start or stop<para>(start|stop)</para></description>
	///		</item>
    ///		<item>
    ///			<term>Server</term>
    ///			<description>The server(s) where the Biztalk host instance is running, a commer delimeted list of servers may be supplied (optional)</description>
    ///		</item>
    ///		<item>
    ///			<term>Logon</term>
    ///			<description>String containing the logon information used by the host instance (optional - unless Server is supplied)</description>
    ///		</item>
    ///		<item>
    ///			<term>PassWord</term>
    ///			<description>String containing the password for the host (optional - unless Logon is supplied)</description>
    ///		</item>
    ///		<item>
    ///			<term>GrantLogOnAsService (optional - unless Logon is supplied)</term>
    ///			<description>Boolean determining whether the 'Log On As Service' privilege should be automatically granted to the specified logon user or not. This flag only has effect when the HostType property is set to In-process</description>
    ///		</item>
    ///	</list>
	///	</remarks>	
    public class HostConductorStep : TestStepBase
	{
	    private string _action;
	    private string _hostName;
	    private string _servers;
	    private string _logon;
	    private string _passWord;
	    private bool _grantLogOnAsService;
        
	    public string Action
	    {
	        get { return _action; }
            set { _action = value; }
	    }

        public string HostInstanceName
        {
            get { return _hostName; }
            set { _hostName = value; }
        }

        public string Servers
        {
            get { return _servers; }
            set { _servers = value; }
        }

        public string Logon
        {
            get { return _logon; }
            set { _logon = value; }
        }

        public string PassWord
        {
            get { return _passWord; }
            set { _passWord = value; }
        }

        public bool GrantLogOnAsService
        {
            get { return _grantLogOnAsService; }
            set { _grantLogOnAsService = value; }
        }

        public override void Execute(Context context)
        {
            var listofServers = _servers.Split(',');
            foreach (var server in listofServers)
            {
                var mo = GetHostInstanceWmiObject(server, _hostName);

                using (mo)
                {
                    if (_action.ToLower() == "start")
                    {
                        if (!string.IsNullOrEmpty(_logon))
                        {
                            var creds = new object[3];
                            creds[0] = _logon;
                            creds[1] = _passWord;
                            creds[2] = _grantLogOnAsService;
                            mo.InvokeMethod("Install", creds);
                        }

                        if (mo["ServiceState"].ToString() == "1")
                        {
                            mo.InvokeMethod("Start", null);
                            context.LogInfo("BizTalk Host Started.");
                        }
                        else
                        {
                            context.LogInfo("BizTalk Host is Started.");
                        }
                    }

                    if (_action.ToLower() == "stop")
                    {
                        if (mo["ServiceState"].ToString() != "0")
                        {
                            mo.InvokeMethod("Stop", null);
                            context.LogInfo("BizTalk Host Stopped.");
                        }
                        else
                        {
                            context.LogInfo("BizTalk Host is Stopped.");
                        }
                    }
                }
            }
        }

        public override void Validate(Context context)
        {
            if (string.IsNullOrEmpty(_action))
            {
                throw new ArgumentNullException("Action is either null or an empty string");
            }

            if (string.IsNullOrEmpty(_hostName))
            {
                throw new ArgumentNullException("HostName is either null or an empty string");
            }

            if (string.IsNullOrEmpty(_servers))
            {
                throw new ArgumentNullException("Servers is either null or an empty string");
            }

            if (null != _logon && 0 < _servers.Length)
            {
                if (string.IsNullOrEmpty(_passWord))
                {
                    throw new ArgumentNullException("PassWord is either null or an empty string");
                }
            }
        }
        
        private static ManagementObject GetHostInstanceWmiObject(string server, string hostName)
		{
            // 2 represents an isolated host and 1 represents an in-process hosts, only an in-process 
            // can be stopped...
			const int hostType = 1;

            var options = new ConnectionOptions
                              {
                                  Impersonation = ImpersonationLevel.Impersonate,
                                  EnablePrivileges = true
                              };

            var searcher = new ManagementObjectSearcher();

            ManagementScope scope = null == server ? new ManagementScope("root\\MicrosoftBizTalkServer", options) : new ManagementScope("\\\\" + server + "\\root\\MicrosoftBizTalkServer", options);

			searcher.Scope = scope;
		
			// Build a Query to enumerate the MSBTS_hostInstance instances 
			var query = new SelectQuery
			                {
			                    QueryString =
			                        String.Format("SELECT * FROM MSBTS_HostInstance where HostName='" + hostName +
			                                      "' AND HostType=" + hostType.ToString())
			                };

            // Set the query for the searcher.
			searcher.Query = query;

			// Execute the query and determine if any results were obtained.
			var queryCol = searcher.Get();			
			var me = queryCol.GetEnumerator();
			me.Reset();

			if ( me.MoveNext() )
			{
				return (ManagementObject)me.Current;
			}

            throw new ApplicationException(string.Format("The WMI object for the Host Instance:{0} could not be retrieved.", hostName ));
		}
	}
}

