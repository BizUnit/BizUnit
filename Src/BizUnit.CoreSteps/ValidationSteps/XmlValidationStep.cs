//---------------------------------------------------------------------
// File: XmlValidationStep.cs
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


using BizUnit.CoreSteps.Utilities;

namespace BizUnit.CoreSteps.ValidationSteps
{
	using System;
	using System.IO;
	using System.Xml;
	using System.Xml.Schema;
    using System.Collections.Generic;
    using System.Web.UI;
    using BizUnitOM;

	/// <summary>
	/// The XmlValidationStep validates an Xml document, it may validate against a given schema, and also evaluate XPath queries.
	/// </summary>
	/// 
	/// <remarks>
	/// The following shows an example of the Xml representation of this test step.
	/// 
	/// <code escaped="true">
	///	<ValidationStep assemblyPath="" typeName="BizUnit.XmlValidationStep">
	///		<XmlSchemaPath>.\TestData\PurchaseOrder.xsd</XmlSchemaPath>
	///		<XmlSchemaNameSpace>urn:bookstore-schema</XmlSchemaNameSpace>
	///		<XPathList>
	///			<XPathValidation query="/*[local-name()='PurchaseOrder' and namespace-uri()='http://SendMail.PurchaseOrder']/*[local-name()='PONumber' and namespace-uri()='']">PONumber_0</XPathValidation>
	///		</XPathList>
	///	</ValidationStep>
	///	</code>
	///	
	///	<list type="table">
	///		<listheader>
	///			<term>Tag</term>
	///			<description>Description</description>
	///		</listheader>
	///		<item>
	///			<term>XmlSchemaPath</term>
	///			<description>The XSD schema to use to validate the XML data <para>(optional)</para></description>
	///		</item>
	///		<item>
	///			<term>XmlSchemaNameSpace</term>
	///			<description>The XSD schema namespace to validate the XML data against <para>(optional)</para></description>
	///		</item>
	///		<item>
	///			<term>XPathList/XPathValidation</term>
	///			<description>XPath expression to evaluate against the XML document is defined in the attribute query, the expected result is defined in the element <para>(optional)(repeating)</para></description>
	///		</item>
	///	</list>
	///	</remarks>	
    [Obsolete("XmlValidationStep has been deprecated. Investigate the BizUnit.TestSteps namespace.")]
	public class XmlValidationStep : IValidationStepOM
	{
        private IList<Pair> _xPathValidations = new List<Pair>();
        private string _xmlSchemaPath;
        private string _xmlSchemaNameSpace;

        public string XmlSchemaPath
        {
            set
            {
                _xmlSchemaPath = value;
            }
        }

        public string XmlSchemaNameSpace
        {
            set
            {
                _xmlSchemaNameSpace = value;
            }
        }

        public IList<Pair> XPathValidations
        {
            get
            {
                return _xPathValidations;
            }

            set
            {
                _xPathValidations = value;
            }
        }

        /// <summary>
		/// IValidationStep.ExecuteValidation() implementation
		/// </summary>
		/// <param name='data'>The stream cintaining the data to be validated.</param>
		/// <param name='validatorConfig'>The Xml fragment containing the configuration for the test step</param>
		/// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
		public void ExecuteValidation(Stream data, XmlNode validatorConfig, Context context)
		{
			_xmlSchemaPath = context.ReadConfigAsString( validatorConfig, "XmlSchemaPath", true );
			_xmlSchemaNameSpace = context.ReadConfigAsString( validatorConfig, "XmlSchemaNameSpace", true );
			var xpaths = validatorConfig.SelectNodes( "XPathList/*" );
            _xPathValidations = new List<Pair>();

			foreach( XmlNode xpath in xpaths)
			{
                string xpathExp = xpath.SelectSingleNode("@query").Value;
                string expectedValue = xpath.SelectSingleNode(".").InnerText;

                _xPathValidations.Add(new Pair(xpathExp, expectedValue));
			}

            ExecuteValidation(data, context);
		}

        /// <summary>
		/// IValidationStep.ExecuteValidation() implementation
		/// </summary>
		/// <param name='data'>The stream cintaining the data to be validated.</param>
		/// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public void ExecuteValidation(Stream data, Context context)
        {
            var doc = new XmlDocument();
            var trData = new XmlTextReader(data);
            var vr = new XmlValidatingReader(trData);

            // If schema was specified use it to vaidate against...
            if (null != _xmlSchemaPath)
            {
                MemoryStream xsdSchema = StreamHelper.LoadFileToStream(_xmlSchemaPath);
                var trSchema = new XmlTextReader(xsdSchema);
                var xsc = new XmlSchemaCollection();

                if (null != _xmlSchemaNameSpace)
                {
                    xsc.Add(_xmlSchemaNameSpace, trSchema);
                    vr.Schemas.Add(xsc);
                }

                doc.Load(vr);
            }

            data.Seek(0, SeekOrigin.Begin);
            doc.Load(data);

            foreach (Pair validation in _xPathValidations)
            {
                var xpathExp = (string)validation.First;
                var expectedValue = (string)validation.Second;

                context.LogInfo("XmlValidationStep evaluting XPath {0} equals \"{1}\"", xpathExp, expectedValue);

                XmlNode checkNode = doc.SelectSingleNode(xpathExp);

                if (0 != expectedValue.CompareTo(checkNode.InnerText))
                {
                    throw new ApplicationException(string.Format("XmlValidationStep failed, compare {0} != {1}, xpath query used: {2}", expectedValue, checkNode.InnerText, xpathExp));
                }
            }
        }

        public void Validate(Context context)
        {
            // xPathValidations - optional

            if (string.IsNullOrEmpty(_xmlSchemaPath))
            {
                throw new ArgumentNullException("XmlSchemaPath is either null or of zero length");
            }
            _xmlSchemaPath = context.SubstituteWildCards(_xmlSchemaPath);

            if (string.IsNullOrEmpty(_xmlSchemaNameSpace))
            {
                throw new ArgumentNullException("XmlSchemaNameSpace is either null or of zero length");
            }
            _xmlSchemaNameSpace = context.SubstituteWildCards(_xmlSchemaNameSpace);
        }
    }
}
