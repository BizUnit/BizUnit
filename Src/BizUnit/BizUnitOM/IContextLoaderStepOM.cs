//---------------------------------------------------------------------
// File: IContextLoaderStepOM.cs
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
using System.IO;

namespace BizUnit.BizUnitOM
{
    /// <summary>
    /// The IContextLoaderStepOM interface is implemented by test steps which 
    /// need to load data into the context and wish to be driven through the 
    /// BizUnit object model.
    /// </summary>
    /// 
    /// <remarks>
    /// The following example demonstrates how to create and call BizUnit:
    /// 
    /// <code escaped="true">
    ///	public class XmlContextLoader : IContextLoaderStepOM
    ///	{
    ///    private IList&gt;Pair&lt; xPathExpressions = new List&gt;Pair&lt;();
    ///
    ///    public IList&gt;Pair&lt; XPathExpressions
	///    {
	///        set { xPathExpressions = value; }
	///    }
    ///
    /// 	public void ExecuteContextLoader(Stream data, XmlNode contextConfig, Context context)
	///   	{
	///		   XmlNodeList contextNodes = contextConfig.SelectNodes("XPath");
    ///
	///		   foreach (XmlNode contextNode in contextNodes)
	///		   {
    ///           Pair xPathPair = new Pair(contextNode.SelectSingleNode("@contextKey").Value, contextNode.SelectSingleNode(".").InnerText);
	///		      xPathExpressions.Add(xPathPair);
	///		   }
    ///
    ///        ExecuteContextLoader(data, context);
	///	    }
    /// 
    ///		public void ExecuteContextLoader(Stream data, Context context)
    ///		{
    ///         XmlDocument doc = new XmlDocument();
    ///         doc.Load(data);
    ///
    ///        foreach (Pair xPathExpression in xPathExpressions)
    ///        {
    ///            string contextKey = (string)xPathExpression.First;
    ///            string xpathExp = (string)xPathExpression.Second;
    ///            string val;
    ///
    ///            context.LogInfo("XmlContextLoader loading key:{0} with value:\"{1}\"", contextKey, xpathExp);
    ///
    ///            try
    ///            {
    ///                val = doc.SelectSingleNode(xpathExp).InnerText;
    ///            }
    ///            catch (Exception ex)
    ///            {
    ///                context.LogError("The XPath expression: {0}, could not be evaluated", xpathExp);
    ///                context.LogException(ex);
    ///                throw;
    ///            }
    ///
    ///            context.Add(contextKey, val);
    ///        }
    ///		}
    /// 
    /// 	public void Validate()
	///     {
    ///        // No validation for xPathExpressions
	///     }
    ///	}
    ///	</code>
    ///	
    ///	</remarks>
    [Obsolete("IContextLoaderStepOM has been deprecated. Please investigate the use of BizUnit.Xaml.TestCase.")]
    public interface IContextLoaderStepOM : IContextLoaderStep
    {
        /// <summary>
        /// ExecuteContextLoader is called by the BizUnit framework to execute the context loader test step.
        /// </summary>
        /// <param name="data">The data which the values are read from.</param>
        /// <param name="context">The context object into which the values will be written.</param>
        void ExecuteContextLoader(Stream data, Context context);

        /// <summary>
        /// Called by the BizUnit framework to validate that the context loader step has been correctly configured
        /// </summary>
        void Validate(Context context);
    }
}
