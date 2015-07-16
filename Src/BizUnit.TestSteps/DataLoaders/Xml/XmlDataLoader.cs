//---------------------------------------------------------------------
// File: XmlDataLoader.cs
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
using System.Xml;
using System.IO;
using System.Xml.XPath;
using System.Collections.ObjectModel;
using BizUnit.Common;
using BizUnit.TestSteps.Common;
using BizUnit.Xaml;

namespace BizUnit.TestSteps.DataLoaders.Xml
{
    /// <summary>
    /// The XmlDataLoader maybe used to load a file from disc and passed to a test 
    /// step or sub-step which accepts a dataloader. The XmlDataLoader enables the 
    /// contents of the data to be modified, for example a node containing a reference 
    /// number could be XPath'd and set to a value that was previously set in the 
    /// context.  This gives greater flexibility around the data that is loaded into 
    /// test steps.
    /// <para>
    /// An example usage of this might be as follows:
    /// A test needs to be created to test a web service that fetches the status of a 
    /// trade, however before this can be done a new trade needs to be created since 
    /// a unique bookingReference is required. The test therefore is created in two 
    /// parts, first it books a new trade using the bookTrade service, the 
    /// bookingReference is set in the test context using the XmlValidationStep. 
    /// Next the test calls the getTradeStatus web method, it loads the request 
    /// body from disc using an XmlDataLoader which in turn sets the value in the body
    /// of the request message to the value of bookingReference previously set in 
    /// the test context.
    /// </para>
    /// </summary>
    /// 
    /// <remarks>
    /// The following example demonstrates how to create and use a dataloader:
    /// 
    /// <code escaped="true">
    /// // The WebServiceStep allows a DataLoader to be used to set the RequestBody,
    /// // this allows greater flexibility around how data is loaded by a test step.
    /// 
    /// var ws = new WebServiceStep();
    ///	ws.Action = "http://schemas.affinus.com/finservices/tradeflow";
    /// 
    /// // Create the dataloader and configure...
    /// var xdl = new XmlDataLoader();
    /// xdl.FilePath = @"..\..\..\Tests\Affinus.TradeServices.BVTs\TradeFlow\BookTrade_RQ.xml";
    /// var xpd = new XPathDefinition();
    /// xpd.Description = "Booking Reference";
    /// 
    /// // Set the BookingReference node to the value in the context with the key "BookingReference"...
    /// xpd.XPath = "/*[local-name()='ConfirmBooking_RQ' and namespace-uri()='http://schemas.affinus.com/finservices/tradeflow']/*[local-name()='Message' and namespace-uri()='http://schemas.affinus.com/finservices/tradeflow']/*[local-name()='Book' and namespace-uri()='http://schemas.affinus.com/finservices/tradeflow']/*[local-name()='AncillaryBookingReference' and namespace-uri()='http://schemas.virgin-atlantic.com/AncillarySales/Book/Services/ConfirmBooking/2009']/@*[local-name()='bookingReference' and namespace-uri()='']";
    /// xpd.ContextKey = "BookingReference";
    /// xdl.UpdateXml.Add(xpd);
    ///
    /// // Assign the dataloader to the RequestBody
    /// ws.RequestBody = xdl;
    /// ws.ServiceUrl = "http://localhost/TradeServices/TradeFlow.svc";
    /// ws.Username = @"domain\user";
    ///
    ///	</code>
    /// </remarks>
    public class XmlDataLoader : DataLoaderBase
    {
        private Collection<XPathDefinition> _updateXml = new Collection<XPathDefinition>();

        ///<summary>
        /// The file path of the data to be loaded
        ///</summary>
        public string FilePath { get; set; }

        ///<summary>
        /// A collection of XPathDefinition's to be applied to the data fetched from FilePath
        ///</summary>
        public Collection<XPathDefinition> UpdateXml
        {
            get
            {
                return _updateXml;
            }

            set
            {
                _updateXml = value;
            }
        }

        public override Stream Load(Context context)
        {
            var doc = new XmlDocument();
            context.LogInfo("Loading file: {0}", FilePath);
            doc.Load(FilePath);

            if (null != UpdateXml)
            {
                foreach (var xpath in UpdateXml)
                {
                    context.LogInfo("Selecting node in document, description: {0}, XPath: {1}", xpath.Description, xpath.XPath);
                    XPathNavigator xpn = doc.CreateNavigator();
                    XPathNavigator node = xpn.SelectSingleNode(xpath.XPath);

                    if (null == node)
                    {
                        context.LogError("XPath expression failed to find node");
                        throw new ApplicationException(String.Format("Node not found: {0}", xpath.Description));
                    }

                    if (!string.IsNullOrEmpty(xpath.ContextKey))
                    {
                        context.LogInfo("Updating XmlNode with value from context key: {0}", xpath.ContextKey);
                        node.SetValue(context.GetValue(xpath.ContextKey));
                    }
                    else
                    {
                        context.LogInfo("Updating XmlNode with value: {0}", xpath.Value);
                        node.SetValue(xpath.Value);
                    }
                }
            }

            var ms = new MemoryStream();
            doc.Save(ms);
            ms.Seek(0, SeekOrigin.Begin);

            return ms;
        }

        public override void Validate(Context context)
        {
            ArgumentValidation.CheckForEmptyString(FilePath, "FilePath");
        }
    }
}
