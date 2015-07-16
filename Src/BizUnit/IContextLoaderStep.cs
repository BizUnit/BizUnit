//---------------------------------------------------------------------
// File: IContextLoaderStep.cs
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

using System.IO;
using System.Xml;

namespace BizUnit
{
	/// <summary>
	/// The IContextLoaderStep interface is implemented by test steps which need to load data into the context.
	/// </summary>
	/// 
	/// <remarks>
	/// The following example demonstrates how to create and call BizUnit:
	/// 
	/// <code escaped="true">
	///	public class XmlContextLoader : IContextLoaderStep
	///	{
	///		public void ExecuteContextLoader(Stream data, XmlNode contextConfig, Context context)
	///		{
	///			XmlNodeList contextNodes = contextConfig.SelectNodes("XPath");
	///
	///			StreamReader sr = new StreamReader( data );
	///			string strData = sr.ReadToEnd();
	///
	///			XmlDocument doc = new XmlDocument();
	///			doc.Load( data );
	///
	///			foreach (XmlNode contextNode in contextNodes)
	///			{
	///				string contextKey = contextNode.SelectSingleNode("@contextKey").Value;
	///				string xpathExp = contextNode.SelectSingleNode(".").InnerText;
	///
	///				context.LogInfo("XmlContextLoader loading key:{0} with value:\"{1}\"", contextKey, xpathExp );
	///
	///				XmlNode node = doc.SelectSingleNode( xpathExp );
	///
	///				context.Add( contextKey, node.InnerText );
	///			}
	///		}
	///	}
	///	</code>
	///	
	///	</remarks>
	public interface IContextLoaderStep
	{
        /// <summary>
        /// ExecuteContextLoader is called by the BizUnit framework to execute the context loader test step.
        /// </summary>
        /// <param name="data">The data which the values are read from.</param>
        /// <param name="contextConfig">The configuration for the context loader test step.</param>
        /// <param name="context">The context object into which the values will be written.</param>
        void ExecuteContextLoader(Stream data, XmlNode contextConfig, Context context);
	}
}
