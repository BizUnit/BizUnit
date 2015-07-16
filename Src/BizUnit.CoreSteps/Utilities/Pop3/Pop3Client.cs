//---------------------------------------------------------------------
// File: Pop3Client.cs
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
	using System;
	using System.Collections;
	using System.Net;
	using System.Net.Sockets;
	using System.Text;
	using System.Text.RegularExpressions;

	internal class Pop3Client
	{
		private Pop3Credential _credential;
		private const int Pop3Port = 110;
		private const int MaxBufferReadSize = 256;
		private long _inboxPosition;
		private long _directPosition = -1;
		private Socket _socket;
		private Pop3Message _pop3Message;

		internal Pop3Credential UserDetails
		{
			set { _credential = value; }
			get { return _credential; }
		}

		internal string From
		{
			get { return _pop3Message.From; }
		}

		internal string To
		{
			get { return _pop3Message.To; }
		}

		internal string Subject
		{
			get { return _pop3Message.Subject; }
		}

		internal string Body
		{
			get { return _pop3Message.Body; }
		}

		internal IEnumerator MultipartEnumerator
		{
			get { return _pop3Message.MultipartEnumerator; }
		}

		internal bool IsMultipart
		{
			get { return _pop3Message.IsMultipart; }
		}


		internal Pop3Client(string user, string pass, string server)
		{
			_credential = new Pop3Credential(user,pass,server);
		}

		private Socket GetClientSocket()
		{
			Socket s = null;
			
			try
			{
			    // Get host related information.
#pragma warning disable 612,618
			    IPHostEntry hostEntry = Dns.Resolve(_credential.Server);
#pragma warning restore 612,618

				// Loop through the AddressList to obtain the supported 
				// AddressFamily. This is to avoid an exception that 
				// occurs when the host IP Address is not compatible 
				// with the address family 
				// (typical in the IPv6 case).
				
				foreach(IPAddress address in hostEntry.AddressList)
				{
					var ipe = new IPEndPoint(address, Pop3Port);
				
					var tempSocket = 
						new Socket(ipe.AddressFamily, 
						SocketType.Stream, ProtocolType.Tcp);

					tempSocket.Connect(ipe);

					if(tempSocket.Connected)
					{
						// we have a connection.
						// return this socket ...
						s = tempSocket;
						break;
					}

                    continue;
				}
			}
			catch(Exception e)
			{
				throw new Pop3ConnectException(e.ToString());
			}

			// throw exception if can't connect ...
			if(s == null)
			{
				throw new Pop3ConnectException("Error : connecting to "
					+_credential.Server);
			}
			
			return s;
		}

		//send the data to server
		private void Send(String data) 
		{
			if(_socket == null)
			{
				throw new Pop3MessageException("Pop3 connection is closed");
			}

			try
			{
				// Convert the string data to byte data 
				// using ASCII encoding.
				
				byte[] byteData = Encoding.ASCII.GetBytes(data+"\r\n");
				
				// Begin sending the data to the remote device.
				_socket.Send(byteData);
			}
			catch(Exception e)
			{
				throw new Pop3SendException(e.ToString());
			}
		}

		private string GetPop3String()
		{
			if(_socket == null)
			{
				throw new 
					Pop3MessageException("Connection to POP3 server is closed");
			}

			var buffer = new byte[MaxBufferReadSize];
			string line;

			try
			{
				int byteCount = 
					_socket.Receive(buffer,buffer.Length,0);

				line = 
					Encoding.ASCII.GetString(buffer, 0, byteCount);
			}
			catch(Exception e)
			{
				throw new Pop3ReceiveException(e.ToString());
			}

			return line;
		}

		private void LoginToInbox()
		{
		    // send username ...
			Send("user "+_credential.User);
		
			// get response ...
			string returned = GetPop3String();

			if( !returned.Substring(0,3).Equals("+OK") )
			{
				throw new Pop3LoginException("login not excepted");
			}

			// send password ...
			Send("pass "+_credential.Pass);

			// get response ...
			returned = GetPop3String();

			if( !returned.Substring(0,3).Equals("+OK") )
			{
				throw new 
					Pop3LoginException("login/password not accepted");
			}
		}

		internal long MessageCount
		{
			get 
			{
				long count = 0;
			
				if(_socket==null)
				{
					throw new Pop3MessageException("Pop3 server not connected");
				}

				Send("stat");

				string returned = GetPop3String();

				// if values returned ...
				if( Regex.Match(returned,
					@"^.*\+OK[ |	]+([0-9]+)[ |	]+.*$").Success )
				{
						// get number of emails ...
						count = long.Parse( Regex
						.Replace(returned.Replace("\r\n","")
						, @"^.*\+OK[ |	]+([0-9]+)[ |	]+.*$" ,"$1") );
				}

				return(count);
			}
		}


		internal void CloseConnection()
		{			
			Send("quit");

			_socket = null;
			_pop3Message = null;
		}

		internal bool DeleteEmail()
		{
			bool ret = false;

			Send("dele "+_inboxPosition);

			string returned = GetPop3String();

			if( Regex.Match(returned,
				@"^.*\+OK.*$").Success )
			{
				ret = true;
			}

			return ret;
		}

		internal bool NextEmail(long directPosition)
		{
			bool ret;

			if( directPosition >= 0 )
			{
				_directPosition = directPosition;
				ret = NextEmail();
			}
			else
			{
				throw new Pop3MessageException("Position less than zero");
			}

			return ret;
		}

		internal bool NextEmail()
		{
		    long pos;

			if(_directPosition == -1)
			{
				if(_inboxPosition == 0)
				{
					pos = 1;
				}
				else
				{
					pos = _inboxPosition + 1;
				}
			}
			else
			{
				pos = _directPosition+1;
				_directPosition = -1;
			}

			// send username ...
			Send("list "+pos);
		
			// get response ...
			var returned = GetPop3String();

			// if email does not exist at this position
			// then return false ...

			if( returned.Substring(0,4).ToUpper().Equals("-ERR") )
			{
				return false;
			}

			_inboxPosition = pos;

			// strip out CRLF ...
			string[] noCr = returned.Split(new[]{ '\r' });

			// get size ...
			string[] elements = noCr[0].Split(new[]{ ' ' });

			long size = long.Parse(elements[2]);

			// ... else read email data
			_pop3Message = new Pop3Message(_inboxPosition,size,_socket);

			return true;
		}

		internal void OpenInbox()
		{
			// get a socket ...
			_socket = GetClientSocket();

			// get initial header from POP3 server ...
			string header = GetPop3String();

			if( !header.Substring(0,3).Equals("+OK") )
			{
				throw new Exception("Invalid initial POP3 response");
			}
		
			// send login details ...
			LoginToInbox();
		}
	}
}
