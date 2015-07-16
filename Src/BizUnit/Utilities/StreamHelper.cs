//---------------------------------------------------------------------
// File: StreamHelper.cs
// 
// Summary: 
//
// Author: Kevin B. Smith (http://www.kevinsmith.co.uk)
//
//---------------------------------------------------------------------
// Copyright (c) 2004-2010, Kevin B. Smith. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------

namespace BizUnit
{
	using System;
	using System.Xml;
	using System.Text;
	using System.IO;

	/// <summary>
	/// Helper class for stream opperations
	/// </summary>
	public class StreamHelper
	{
		/// <summary>
		/// Performs a binary comparison between two streams
		/// </summary>
		/// <param name="s1">The 1st stream to compare aginst the 2nd</param>
		/// <param name="s2">The 2nd stream to compare aginst the 1st</param>
		static public void CompareStreams(Stream s1, Stream s2)
		{
			byte[] buff1 = new byte[4096];
			byte[] buff2 = new byte[4096];
			int read1;

			do
			{
				read1 = s1.Read(buff1, 0, 4096);
				int read2 = s2.Read(buff2, 0, 4096);

				if ( read1 != read2 )
				{
					throw new ApplicationException( String.Format( "Streams do not contain identical data!" ) );
				}

				if ( 0 == read1 )
				{
					break;
				}

				for ( int c = 0; c < read1; c++ )
				{
					if ( buff1[c] != buff2[c] )
					{
						throw new ApplicationException( String.Format( "Streams do not contain identical data!" ) );
					}
				}

			} while( read1 > 0 );
		}

		/// <summary>
		/// Helper method to load a disc FILE into a MemoryStream
		/// </summary>
		/// <param name="filePath">The path to the FILE containing the data</param>
		/// <param name="timeout">The timeout afterwhich if the FILE is not found the method will fail</param>
		/// <returns>MemoryStream containing the data in the FILE</returns>

		public static MemoryStream LoadFileToStream(string filePath, double timeout)
		{
			MemoryStream ms = null;
			bool loaded = false;

			DateTime now = DateTime.Now;

			do
			{
				try
				{
					ms = LoadFileToStream(filePath);
					loaded = true;
					break;
				}
				catch(Exception)
				{
					if ( DateTime.Now < now.AddMilliseconds(timeout) )
					{
						System.Threading.Thread.Sleep(500);
					}
				}
			} while ( DateTime.Now < now.AddMilliseconds(timeout) );


			if ( !loaded )
			{
				throw new ApplicationException( string.Format( "The file: {0} was not found within the timeout period!", filePath ) );
			}

			return ms;
		}

		/// <summary>
		/// Helper method to load a disc FILE into a MemoryStream
		/// </summary>
		/// <param name="filePath">The path to the FILE containing the data</param>
		/// <returns>MemoryStream containing the data in the FILE</returns>
		public static MemoryStream LoadFileToStream(string filePath)
		{
			FileStream fs = null;
			MemoryStream s;

			try
			{
				// Get the match data...
				fs = File.OpenRead(filePath);
				s = new MemoryStream();

				byte[] buff = new byte[1024];
				int read = fs.Read(buff, 0, 1024);

				while ( 0 < read )
				{
					s.Write(buff, 0, read);
					read = fs.Read(buff, 0, 1024);
				}

				s.Flush();
				s.Seek(0, SeekOrigin.Begin);
			}
			finally
			{
				if ( null != fs )
				{
					fs.Close();
				}
			}

			return s;
		}

		/// <summary>
		/// Helper method to write the data in a stream to the console
		/// </summary>
		/// <param name="description">The description text that will be written before the stream data</param>
		/// <param name="ms">Stream containing the data to write</param>
		/// <param name="context">The BizUnit context object which holds state and is passed between test steps</param>
		public static void WriteStreamToConsole(string description, MemoryStream ms, Context context)
		{
			ms.Seek(0, SeekOrigin.Begin);
			StreamReader sr = new StreamReader(ms);
			context.LogData( description, sr.ReadToEnd() );
			ms.Seek(0, SeekOrigin.Begin);
		}

		/// <summary>
		/// Helper method to load a forward only stream into a seekable MemoryStream
		/// </summary>
		/// <param name="s">The forward only stream to read the data from</param>
		/// <returns>MemoryStream containg the data as read from s</returns>
		public static MemoryStream LoadMemoryStream(Stream s)
		{
			MemoryStream ms = new MemoryStream();
			byte[] buff = new byte[1024];
			int read = s.Read(buff, 0, 1024);

			while ( 0 < read )
			{
				ms.Write(buff, 0, read);
				read = s.Read(buff, 0, 1024);
			}
			ms.Flush();
			ms.Seek(0, SeekOrigin.Begin);

			return ms;
		}

		/// <summary>
		/// Helper method to load a string into a MemoryStream
		/// </summary>
		/// <param name="s">The string containing the data that will be loaded into the stream</param>
		/// <returns>MemoryStream containg the data read from the string</returns>
		public static MemoryStream LoadMemoryStream(string s)
		{
			Encoding utf8 = Encoding.UTF8;
			byte[] bytes = utf8.GetBytes(s);
			MemoryStream ms = new MemoryStream(bytes);

			ms.Flush();
			ms.Seek(0, SeekOrigin.Begin);

			return ms;
		}

		/// <summary>
		/// Helper method to compare two Xml documents from streams
		/// </summary>
		/// <param name="s1">Stream containing the 1st Xml document</param>
		/// <param name="s2">Stream containing the 2nd Xml document</param>
		/// <param name="context">The BizUnit context object which holds state and is passed between test steps</param>
		public static void CompareXmlDocs(Stream s1, Stream s2, Context context)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(new XmlTextReader(s1));
			XmlElement root = doc.DocumentElement;
			string data1 = root.OuterXml;

			doc = new XmlDocument();
			doc.Load(new XmlTextReader(s2));
			root = doc.DocumentElement;
			string data2 = root.OuterXml;

			context.LogInfo("About to compare the following Xml documents:\r\nDocument1: {0},\r\nDocument2: {1}", data1, data2);

			CompareStreams( LoadMemoryStream(data1), LoadMemoryStream(data2) );
		}

		/// <summary>
		/// Helper method to encode a stream
		/// </summary>
		/// <param name="rawData">Stream containing data to be encoded</param>
		/// <param name="encoding">The encoding to be used for the data</param>
		/// <returns>Encoded MemoryStream</returns>
		public static Stream EncodeStream(Stream rawData, Encoding encoding)
		{
			rawData.Seek(0, SeekOrigin.Begin);
			StreamReader sr = new StreamReader(rawData);
			string data = sr.ReadToEnd();
			Encoding e = encoding;
			byte[] bytes = e.GetBytes(data);

			return new MemoryStream(bytes);
		}
	}
}
