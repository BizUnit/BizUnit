//---------------------------------------------------------------------
// File: XmlContextLoader.cs
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
    using System.Collections.Generic;
    using System.Web.UI;
    using BizUnitOM;

	/// <summary>
	/// The XmlContextLoader evaluates an XPath expression to the source data and adds the value into the context.
	/// </summary>
	/// 
	/// <remarks>
	/// The following shows an example of the Xml representation of this test step.
	/// 
	/// <code escaped="true">
	///	<ContextLoaderStep assemblyPath="" typeName="BizUnit.XmlContextLoader">
	///		<XPath contextKey="HTTP_Url">/def:html/def:body/def:p[2]/def:form</XPath>
	///		<XPath contextKey="ActionID">/def:html/def:body/def:p[2]/def:form/def:input[3]</XPath>
	///		<XPath contextKey="ActionType">/def:html/def:body/def:p[2]/def:form/def:input[4]</XPath>
	///		<XPath contextKey="HoldEvent">/def:html/def:body/def:p[2]/def:form/def:input[2]</XPath>
	///	</ContextLoaderStep>
	///	</code>
	///	
	///	<list type="table">
	///		<listheader>
	///			<term>Tag</term>
	///			<description>Description</description>
	///		</listheader>
	///		<item>
	///			<term>XPath</term>
	///			<description>The XPAth expression to evaluate against the input data <para>(repeating)</para></description>
	///		</item>
	///		<item>
	///			<term>XPath/contextKey</term>
	///			<description>The name of context key which will be used when addin the new context item</description>
	///		</item>
	///	</list>
	///	</remarks>	
    [Obsolete("XmlContextLoader has been deprecated. Investigate the BizUnit.TestSteps namespace.")]
	public class XmlContextLoader : IContextLoaderStepOM
	{
	    private IList<Pair> _xPathExpressions = new List<Pair>();

	    public IList<Pair> XPathExpressions
	    {
	        set
	        {
	            _xPathExpressions = value;
	        }
	    }

		/// <summary>
		/// IContextLoaderStep.ExecuteContextLoader() implementation
		/// </summary>
		/// <param name="data">The data which the values are read from.</param>
		/// <param name="contextConfig">The configuration for the context loader test step.</param>
		/// <param name="context">The context object into which the values will be written.</param>
		public void ExecuteContextLoader(Stream data, XmlNode contextConfig, Context context)
		{
			var contextNodes = contextConfig.SelectNodes("XPath");

			foreach (XmlNode contextNode in contextNodes)
			{
                var xPathPair = new Pair(contextNode.SelectSingleNode("@contextKey").Value, contextNode.SelectSingleNode(".").InnerText);
			    _xPathExpressions.Add(xPathPair);
			}

            ExecuteContextLoader(data, context);
		}

	    public void ExecuteContextLoader(Stream data, Context context)
	    {
            var doc = new XmlDocument();
            doc.Load(data);

            foreach (var xPathExpression in _xPathExpressions)
            {
                var contextKey = (string)xPathExpression.First;
                var xpathExp = (string)xPathExpression.Second;
                string val;

                context.LogInfo("XmlContextLoader loading key:{0} with value:\"{1}\"", contextKey, xpathExp);

                try
                {
                    val = doc.SelectSingleNode(xpathExp).InnerText;
                }
                catch (Exception ex)
                {
                    context.LogError("The XPath expression: {0}, could not be evaluated", xpathExp);
                    context.LogException(ex);
                    throw;
                }

                context.Add(contextKey, val);
            }
        }

        public void Validate(Context context)
	    {
            // No validation for _xPathExpressions
	    }
	}
}
