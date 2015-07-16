//---------------------------------------------------------------------
// File: HttpHelper.cs
// 
// Summary: 
//
// Author: Kevin B. Smith
//
//---------------------------------------------------------------------
// Copyright (c) 2004-2015, Kevin B. Smith. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------

namespace BizUnit.TestSteps.Http
{
	using System;
	using System.IO;
	using System.Net;

	/// <summary>
	/// Helper class for HTTP
	/// </summary>
	public class HttpHelper
	{
		/// <summary>
		/// Helper method to execute an HTTP request-response
		/// </summary>
		/// 
		/// <param name="url">The HTTP Url</param>
		/// <param name="payload">Byte array conatining the request data</param>
		/// <param name="requestTimeout">The request timeout value</param>
		/// <param name="context">The BizUnit context object which holds state and is passed between test steps</param>
		/// <returns>response MemoryStream</returns>
		public static MemoryStream SendRequestData(String url, byte[] payload, int requestTimeout, Context context)
		{
			WebResponse result = null;
			var response = new MemoryStream();
			Stream responseStream = null;
			Stream requestStream = null;
			
            try 
			{
                var req = (HttpWebRequest)WebRequest.Create(url);

				req.Method = "POST";
				req.Timeout = requestTimeout;
				req.ContentType = "text/xml; charset=\"utf-8\"";

				req.ContentLength = payload.Length;
				requestStream = req.GetRequestStream();
				requestStream.Write( payload, 0, payload.Length );

				result = req.GetResponse();
				responseStream = result.GetResponseStream();

				const int bufferSize = 4096;
				var buffer = new byte[bufferSize];

				int count = responseStream.Read( buffer, 0, bufferSize );
				while (count > 0) 
				{
					response.Write(buffer, 0, count);
					count = responseStream.Read(buffer, 0, bufferSize );
				}
				response.Seek(0, SeekOrigin.Begin);
			} 
			catch(Exception e) 
			{
				context.LogException( e );
				throw;
			} 
			finally 
			{
				if ( null != result ) 
				{
					result.Close();
				}

				if ( null != responseStream )
				{
					responseStream.Close();
				}

				if ( null != requestStream )
				{
					requestStream.Close();
				}
			}

			return response;
		}
	}
}
