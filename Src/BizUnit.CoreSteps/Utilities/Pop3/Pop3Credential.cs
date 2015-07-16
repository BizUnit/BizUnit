//---------------------------------------------------------------------
// File: Pop3Credential.cs
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

namespace BizUnit.CoreSteps.Utilities.Pop3
{
	/// <summary>
	/// Summary description for Credentials.
	/// </summary>
	internal class Pop3Credential
	{
	    private readonly string[] _sendStrings = { "user", "pass" };

		internal string[] SendStrings
		{
			get { return _sendStrings; }
		}

	    internal string User { get; set; }
	    internal string Pass { get; set; }
	    internal string Server { get; set; }

	    internal Pop3Credential(string user, string pass, string server)
		{
			User = user;
			Pass = pass;
			Server = server;
		}

		internal Pop3Credential()
		{
			User = null;
			Pass = null;
			Server = null;
		}
	}
}
