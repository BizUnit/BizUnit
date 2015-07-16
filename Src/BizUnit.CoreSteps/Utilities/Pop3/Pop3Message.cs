//---------------------------------------------------------------------
// File: Pop3Message.cs
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
	using System.Net.Sockets;
	using System.Threading;
	using System.Text;
	using System.Text.RegularExpressions;

	/// <summary>
	/// DLM: Stores the From:, To:, Subject:, body and attachments
	/// within an email. Binary attachments are Base64-decoded
	/// </summary>

	internal class Pop3Message
	{
		private readonly Socket _client;
		private Pop3MessageComponents _messageComponents;
		private string _from;
		private string _to;
		private string _subject;
		private string _contentType;
		private readonly string _body;
        private bool _isMultipart;
        private string _multipartBoundary;
		
		private const int FromState=0;
		private const int ToState=1;
		private const int SubjectState = 2;
		private const int ContentTypeState = 3;
		private const int NotKnownState = -99;
		private const int EndOfHeader = -98;

		// this array corresponds with above
		// enumerator ...

		private readonly string[] _lineTypeString =
		{
			"From",
			"To",
			"Subject",
			"Content-Type"
		};
		
		private readonly long _inboxPosition;
        Pop3StateObject _pop3State;
        readonly ManualResetEvent _manualEvent = new ManualResetEvent(false);

		internal IEnumerator MultipartEnumerator
		{
			get { return _messageComponents.ComponentEnumerator; }
		}

		internal bool IsMultipart
		{
			get { return _isMultipart; }
		}

		internal string From
		{
			get { return _from; }
		}

		internal string To
		{
			get { return _to; }
		}

		internal string Subject
		{
			get { return _subject; }
		}

		internal string Body
		{
			get { return _body; }
		}

		internal long InboxPosition
		{
			get { return _inboxPosition; }
		}

		//send the data to server
		private void Send(String data) 
		{
			try
			{
				// Convert the string data to byte data 
				// using ASCII encoding.
				
				byte[] byteData = Encoding.ASCII.GetBytes(data+"\r\n");
				
				// Begin sending the data to the remote device.
				_client.Send(byteData);
			}
			catch(Exception e)
			{
				throw new Pop3SendException(e.ToString());
			}
		}

		private void StartReceiveAgain(string data)
		{
			// receive more data if we expect more.
			// note: a literal "." (or more) followed by
			// "\r\n" in an email is prefixed with "." ...

			if( !data.EndsWith("\r\n.\r\n") )
			{
				_client.BeginReceive(_pop3State.Buffer,0,
					Pop3StateObject.BufferSize,0,
					new AsyncCallback(ReceiveCallback),
					_pop3State);
			}
			else
			{
				// stop receiving data ...
				_manualEvent.Set();
			}
		}

		private void ReceiveCallback( IAsyncResult ar ) 
		{
			try 
			{
				// Retrieve the state object and the client socket 
				// from the asynchronous state object.
				
				var stateObj = (Pop3StateObject) ar.AsyncState;
				var client = stateObj.WorkSocket;
				
				// Read data from the remote device.
				int bytesRead = client.EndReceive(ar);

				if (bytesRead > 0) 
				{
					// There might be more data, 
					// so store the data received so far.
					
					stateObj.Sb.Append(
						Encoding.ASCII.GetString(stateObj.Buffer
						,0,bytesRead));

					// read more data from pop3 server ...
					StartReceiveAgain(stateObj.Sb.ToString());
				}
			} 
			catch (Exception e) 
			{
				_manualEvent.Set();

				throw new 
					Pop3ReceiveException("RecieveCallback error" + 
					e);
			}
		}

		private void StartReceive()
		{
			// start receiving data ...
			_client.BeginReceive(_pop3State.Buffer,0,
				Pop3StateObject.BufferSize,0,
				new AsyncCallback(ReceiveCallback),
				_pop3State);

			// wait until no more data to be read ...
			_manualEvent.WaitOne();
		}

		private int GetHeaderLineType(string line)
		{
			int lineType = NotKnownState;

			for(int i=0; i<_lineTypeString.Length; i++)
			{
				string match = _lineTypeString[i];

				if( Regex.Match(line,"^"+match+":"+".*$").Success )
				{
					lineType = i;
					break;
				}

                if( line.Length == 0 )
				{
					lineType = EndOfHeader;
					break;
				}
			}

			return lineType;
		}

		private long ParseHeader(string[] lines)
		{
			int numberOfLines = lines.Length;
			long bodyStart = 0;

			for(int i=0; i<numberOfLines; i++)
			{
				string currentLine = lines[i].Replace("\n","");

				int lineType = GetHeaderLineType(currentLine);

				switch(lineType)
				{
					// From:
					case FromState:
							_from = Pop3Parse.From(currentLine);	
					break;

					// Subject:
					case SubjectState:
							_subject =	Pop3Parse.Subject(currentLine);
					break;

					// To:
					case ToState:
							_to = Pop3Parse.To(currentLine);
					break;

					// Content-Type
					case ContentTypeState:
							
						_contentType = 
							Pop3Parse.ContentType(currentLine);
						
						_isMultipart = 
							Pop3Parse.IsMultipart(_contentType);

						if(_isMultipart)
						{
							// if boundary definition is on next
							// line ...

							if(_contentType
								.Substring(_contentType.Length-1,1).
								Equals(";"))
							{
								++i;

								_multipartBoundary
									= Pop3Parse.
									MultipartBoundary(lines[i].
									Replace("\n",""));
							}
							else
							{
								// boundary definition is on same
								// line as "Content-Type" ...

								_multipartBoundary =
									Pop3Parse
									.MultipartBoundary(_contentType);
							}
						}

					break;

					case EndOfHeader:
							bodyStart = i+1;
					break;
				}

				if(bodyStart>0)
				{
					break;
				}
			}

			return(bodyStart);
		}
		
		private void ParseEmail(string[] lines)
		{
			long startOfBody = ParseHeader(lines);

			_messageComponents = 
				new Pop3MessageComponents(lines,startOfBody
				,_multipartBoundary,_contentType);
		}

		private void LoadEmail()
		{
			// tell pop3 server we want to start reading
			// email (m_inboxPosition) from inbox ...

			Send("retr "+_inboxPosition);

			// start receiving email ...
			StartReceive();

			// parse email ...
			ParseEmail(
				_pop3State.Sb.ToString().Split(new[] { '\r'}));

			// remove reading pop3State ...
			_pop3State = null;
		}

		internal Pop3Message(long position, long size, Socket client)
		{
			_inboxPosition = position;
			_client = client;

			_pop3State = new Pop3StateObject {WorkSocket = _client, Sb = new StringBuilder()};

		    // load email ...
			LoadEmail();

			// get body (if it exists) ...
			IEnumerator multipartEnumerator =
				MultipartEnumerator;

			while( multipartEnumerator.MoveNext() )
			{
				var multipart = (Pop3Component)
					multipartEnumerator.Current;

				if( multipart.IsBody )
				{
					_body = multipart.Data;
					break;
				}
			}
		}

		public override string ToString()
		{
			IEnumerator enumerator = MultipartEnumerator;

			string ret = 
				"From    : "+_from+ "\r\n"+
				"To      : "+_to+ "\r\n"+
				"Subject : "+_subject+"\r\n";

			while( enumerator.MoveNext() )
			{
				ret += enumerator.Current+"\r\n";
			}
	
			return ret;
		}
	}
}
