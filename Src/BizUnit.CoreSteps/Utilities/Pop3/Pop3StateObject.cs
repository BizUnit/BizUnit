//---------------------------------------------------------------------
// File: Pop3StateObject.cs
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
	using System.Net.Sockets;
	using System.Text;

	/// <summary>
	/// Holds the current state of the client
	/// socket.
	/// </summary>

	internal class Pop3StateObject
	{
		// Client socket.
		internal Socket WorkSocket;
		
		// Size of receive buffer.
		internal const int BufferSize = 256;
		
		// Receive buffer.
		internal byte[] Buffer = new byte[BufferSize];
		
		// Received data string.
		internal StringBuilder Sb = new StringBuilder();
	}
}
