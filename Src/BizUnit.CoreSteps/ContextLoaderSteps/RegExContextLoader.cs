//---------------------------------------------------------------------
// File: RegExContextLoader.cs
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

namespace BizUnit.CoreSteps.ContextLoaderSteps
{
	using System.IO;
	using System.Xml;
	using System.Text.RegularExpressions;

	/// <summary>
	/// The RegExContextLoader applies a regular expression to the source data and add the value into the context.
	/// </summary>
	/// 
	/// <remarks>
	/// The following shows an example of the Xml representation of this test step.
	/// 
	/// <code escaped="true">
	///	<ContextLoaderStep assemblyPath="" typeName="BizUnit.RegExContextLoader">
	///		<RegEx contextKey="HTTP_Url">/def:html/def:body/def:p[2]/def:form</RegEx>
	///		<RegEx contextKey="ActionID">/def:html/def:body/def:p[2]/def:form/def:input[3]</RegEx>
	///		<RegEx contextKey="ActionType">/def:html/def:body/def:p[2]/def:form/def:input[4]</RegEx>
	///		<RegEx contextKey="HoldEvent">/def:html/def:body/def:p[2]/def:form/def:input[2]</RegEx>
	///	</ContextLoaderStep>
	///	</code>
	///	
	///	<list type="table">
	///		<listheader>
	///			<term>Tag</term>
	///			<description>Description</description>
	///		</listheader>
	///		<item>
	///			<term>RegEx</term>
	///			<description>The regular expression to be executed against the input data <para>(repeating)</para></description>
	///		</item>
	///		<item>
	///			<term>RegEx/contextKey</term>
	///			<description>The name of context key which will be used when addin the new context item</description>
	///		</item>
	///	</list>
	///	</remarks>	
    [Obsolete("RegExContextLoader has been deprecated. Investigate the BizUnit.TestSteps namespace.")]
	public class RegExContextLoader : IContextLoaderStep
	{
		/// <summary>
		/// IContextLoaderStep.ExecuteContextLoader() implementation
		/// </summary>
		/// <param name="data">The data which the values are read from.</param>
		/// <param name="contextConfig">The configuration for the context loader test step.</param>
		/// <param name="context">The context object into which the values will be written.</param>
		public void ExecuteContextLoader(Stream data, XmlNode contextConfig, Context context)
		{
			XmlNodeList xpressions = contextConfig.SelectNodes("RegEx");
			
			var sr = new StreamReader(data);
			string input = sr.ReadToEnd();

			foreach(XmlNode xpression in xpressions)
			{
				XmlNode key = xpression.SelectSingleNode("@contextKey");
				Match match = Regex.Match(input, xpression.InnerText);
				context.Add(key.InnerText, match.Value);
				context.LogInfo("Loading context key: \"{0}\", value: \"{1}\"", key.InnerText, match.Value);
			}
		}
	}
}
