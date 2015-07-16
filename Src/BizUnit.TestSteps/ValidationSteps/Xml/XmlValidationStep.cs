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

using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Collections.ObjectModel;
using BizUnit.Common;
using BizUnit.TestSteps.Common;
using BizUnit.Xaml;

namespace BizUnit.TestSteps.ValidationSteps.Xml
{
    /// <summary>
    /// The XmlValidationStep validates an Xml document, it may validate against a given schema, and also evaluate XPath queries.
    /// The Xpath query is extended from XmlValidationStep to allow Xpath functions to be used which may not return a node set.
    /// </summary>
    /// 
    /// <remarks>
    /// The following shows an example of the Xml representation of this test step.
    /// 
    /// <code escaped="true">
    ///	<SubSteps assemblyPath="" typeName="BizUnit.XmlValidationStep">
    ///		<XmlSchemaPath>.\TestData\PurchaseOrder.xsd</XmlSchemaPath>
    ///		<XmlSchemaNameSpace>urn:bookstore-schema</XmlSchemaNameSpace>
    ///		<XPathList>
    ///			<XPathValidation query="/*[local-name()='PurchaseOrder' and namespace-uri()='http://SendMail.PurchaseOrder']/*[local-name()='PONumber' and namespace-uri()='']">PONumber_0</XPathValidation>
    ///		</XPathList>
    ///	</SubSteps>
    ///	</code>
    ///	
    ///	<list type="table">
    ///		<listheader>
    ///			<term>Tag</term>
    ///			<description>Description</description>
    ///		</listheader>
    ///		<item>
    ///			<term>XmlSchemaPath</term>
    ///			<description>The XSD schema to use to validate the XML data (optional)</description>
    ///		</item>
    ///		<item>
    ///			<term>XmlSchemaNameSpace</term>
    ///			<description>The XSD schema namespace to validate the XML data against (optional)</description>
    ///		</item>
    ///		<item>
    ///			<term>XPathList/XPathValidation</term>
    ///			<description>XPath expression to evaluate against the XML document (optional)(repeating).</description>
    ///		</item>
    ///	</list>
    ///	</remarks>	
    public class XmlValidationStep : SubStepBase
    {
        private Collection<XPathDefinition> _xPathValidations = new Collection<XPathDefinition>();
        private Collection<SchemaDefinition> _xmlSchemas = new Collection<SchemaDefinition>();
        private Exception _validationException;
        private Context _context;

        public Collection<SchemaDefinition> XmlSchemas
        {
            set
            {
                _xmlSchemas = value;
            }
            get
            {
                return _xmlSchemas;
            }
        }

        public Collection<XPathDefinition> XPathValidations
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
        /// ITestStep.Execute() implementation
        /// </summary>
        /// <param name='data'>The stream cintaining the data to be validated.</param>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public override Stream Execute(Stream data, Context context)
        {
            _context = context;

            var document = ValidateXmlInstance(data, context);
            ValidateXPathExpressions(document, context);
            data.Seek(0, SeekOrigin.Begin);

            return data;
        }

        public override void Validate(Context context)
        {
            foreach(var schema in XmlSchemas)
            {
                ArgumentValidation.CheckForNullReference(schema.XmlSchemaPath, "schema.XmlSchemaPath");
                ArgumentValidation.CheckForNullReference(schema.XmlSchemaNameSpace, "schema.XmlSchemaNameSpace");
            }

            foreach(var xpath in XPathValidations)
            {
                ArgumentValidation.CheckForNullReference(xpath.XPath, "xpath.XPath");
                ArgumentValidation.CheckForNullReference(xpath.Value, "xpath.Value");
            }
        }

        private XmlDocument ValidateXmlInstance(Stream data, Context context)
        {
            try
            {
                var settings = new XmlReaderSettings();
                foreach (var xmlSchema in _xmlSchemas)
                {
                    settings.Schemas.Add(xmlSchema.XmlSchemaNameSpace, xmlSchema.XmlSchemaPath);
                }
                settings.ValidationType = ValidationType.Schema;

                XmlReader reader = XmlReader.Create(data, settings);
                var document = new XmlDocument();
                document.Load(reader);

                var eventHandler = new ValidationEventHandler(ValidationEventHandler);

                document.Validate(eventHandler);

                return document;
            }
            catch (Exception ex)
            {
                context.LogException(ex);
                throw new ValidationStepExecutionException("Failed to validate document instance", ex, context.TestName);
            }
        }

        private void ValidateXPathExpressions(XmlDocument doc, Context context)
        {
            foreach (XPathDefinition validation in _xPathValidations)
            {
                var xpathExp = validation.XPath;
                var expectedValue = validation.Value;

                if (null != validation.Description)
                {
                    context.LogInfo("XPath: {0}", validation.Description);
                }
                context.LogInfo("Evaluting XPath {0} equals \"{1}\"", xpathExp, expectedValue);

                XPathNavigator xpn = doc.CreateNavigator();
                object result = xpn.Evaluate(xpathExp);
                
                string actualValue = null;
                if (result.GetType().Name == "XPathSelectionIterator")
                {
                    var xpi = result as XPathNodeIterator;
                    xpi.MoveNext(); // BUGBUG!
                    actualValue = xpi.Current.ToString();
                }
                else
                {
                    actualValue = result.ToString();
                }

                if (!string.IsNullOrEmpty(validation.ContextKey))
                {
                    context.Add(validation.ContextKey, actualValue);
                }

                if (!string.IsNullOrEmpty(expectedValue))
                {

                    if (0 != expectedValue.CompareTo(actualValue))
                    {
                        context.LogError("XPath evaluation failed. Expected:<{0}>. Actual:<{1}>.", expectedValue, actualValue);

                        throw new ApplicationException(
                            string.Format("XmlValidationStep failed, compare {0} != {1}, xpath query used: {2}",
                                          expectedValue, actualValue, xpathExp));
                    }

                    context.LogInfo("XPath evaluation succeeded. Expected:<{0}>. Actual:<{1}>.", expectedValue, actualValue);
                }
            }
        }

        void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            switch (e.Severity)
            {
                case XmlSeverityType.Error:
                    _context.LogError(e.Message);
                    _validationException = e.Exception;
                    break;
                case XmlSeverityType.Warning:
                    _context.LogWarning(e.Message);
                    _validationException = e.Exception;
                    break;
            }
        }
    }
}
