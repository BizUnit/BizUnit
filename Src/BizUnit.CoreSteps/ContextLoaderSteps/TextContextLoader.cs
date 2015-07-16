//---------------------------------------------------------------------
// File: TextContextLoader.cs
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

namespace BizUnit.CoreSteps.ContextLoaderSteps
{
    using System;
    using System.IO;
    using System.Xml;

	/// <summary>
	/// The TextContextLoader searches the source data and adds the value into the context.
	/// </summary>
	/// 
	/// <remarks>
	/// <para>Here's the SearchString, but then skip this bit and return: TargetFindString</para>
	/// 
	/// <para>searchString="SearchString" skipNumber="37" stringLength="16"</para>
	/// 
	/// <para>The following shows an example of the Xml representation of this test step.</para>
	/// 
	/// <code escaped="true">
	///	<ContextLoaderStep assemblyPath="" typeName="BizUnit.TextContextLoader">
	///		<Item contextKey="KeyString" searchString="SearchString" skipNumber="37" stringLength="16"/>
	///	</ContextLoaderStep>
	///	</code>
	///	
	///	<list type="table">
	///		<listheader>
	///			<term>Tag</term>
	///			<description>Description</description>
	///		</listheader>
	///		<item>
	///			<term>Item/contextKey</term>
	///			<description>The name of context key which will be used when addin the new context item</description>
	///		</item>
	///		<item>
	///			<term>Item/searchString</term>
	///			<description>The string used to search for</description>
	///		</item>
	///		<item>
	///			<term>Item/skipNumber</term>
	///			<description>The number of characters to skip after the search string</description>
	///		</item>
	///		<item>
	///			<term>Item/stringLength</term>
	///			<description>The number of characters after skipNumber</description>
	///		</item>
	///	</list>
	///	</remarks>	
    [Obsolete("TextContextLoader has been deprecated. Investigate the BizUnit.TestSteps namespace.")]
	public class TextContextLoader : IContextLoaderStep
	{
		/// <summary>
		/// IContextLoaderStep.ExecuteContextLoader() implementation
		/// </summary>
		/// <param name="data">The data which the values are read from.</param>
		/// <param name="contextConfig">The configuration for the context loader test step.</param>
		/// <param name="context">The context object into which the values will be written.</param>
		public void ExecuteContextLoader(Stream data, XmlNode contextConfig, Context context)
		{
			XmlNodeList contextNodes = contextConfig.SelectNodes("Item");

			var sr = new StreamReader( data );
			string strData = sr.ReadToEnd();

			foreach (XmlNode contextNode in contextNodes)
			{
				string contextKey = contextNode.SelectSingleNode("@contextKey").Value;
				string searchString = contextNode.SelectSingleNode("@searchString").Value;
				int skipNumber = Convert.ToInt32( contextNode.SelectSingleNode("@skipNumber").Value );
				int stringLength = Convert.ToInt32( contextNode.SelectSingleNode("@stringLength").Value );

				int startIndex = strData.IndexOf( searchString );
				string targetData = strData.Substring( startIndex + skipNumber + searchString.Length, stringLength );

                context.LogInfo("TextContextLoader loading key: {0} with value: \"{1}\"", contextKey, targetData);

				context.Add( contextKey, targetData );
			}
		}
	}
}
