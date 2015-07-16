//---------------------------------------------------------------------
// File: SOAPHTTPRequestResponseStep.cs
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

namespace BizUnit.CoreSteps.TestSteps
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Net;
    using System.Reflection;
    using System.Xml.Serialization;
    using Microsoft.CSharp;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.Web.Services.Description;

    /// <summary>
    /// The SOAPOverHTTPRequestResponseStep test step may be used to call a Web Service and optionally validate it's response.
    /// </summary>
    /// 
    /// <remarks>
    /// The following shows an example of the Xml representation of this test step.
    /// 
    /// <code escaped="true">
    ///	<TestStep assemblyPath="" typeName="BizUnit.SOAPOverHTTPRequestResponseStep">
    ///		<WebServiceWSDLURL>http://machine/virdir/StockQuoteService.aspx?wsdl</WebServiceWSDLURL>
    ///		<ServiceName>Samples_StockQuoteService</ServiceName>
    ///		<WebMethod>GetQuote</WebMethod>
    ///		<InputMessageTypeName>QuoteRequest</InputMessageTypeName>
    ///		<MessagePayload>.\TestData\RequestMSFTQuote.xml</MessagePayload>
    ///		
    ///		<!-- Note: ContextLoader Step could be any generic validation step -->	
    ///	    <ContextLoaderStep assemblyPath="" typeName="BizUnit.XmlContextLoader">
    ///		    <XPath contextKey="PoNumber">/Po/Header/Id</XPath>
    ///		    <XPath contextKey="Sender">/Po/Header/Id</XPath>
    ///		    <XPath contextKey="Amount">/Po/Header/Id</XPath>
    ///		    <XPath contextKey="Description">/Po/Header/Detail/Description</XPath>
    ///	    </ContextLoaderStep>
    ///	
    ///		<!-- Note: Validation step could be any generic validation step -->	
    ///		<ValidationStep assemblyPath="" typeName="BizUnit.XmlValidationStep">
    ///			<XPathList>
    ///				<XPathValidation query="/StockQuote/Symbol">MSFT</XPathValidation>
    ///				<XPathValidation query="/StockQuote/LastPrice">35.36</XPathValidation>
    ///			</XPathList>
    ///		</ValidationStep>
    ///	</TestStep>
    ///	</code>
    ///	
    ///	<list type="table">
    ///		<listheader>
    ///			<term>Tag</term>
    ///			<description>Description</description>
    ///		</listheader>
    ///		<item>
    ///			<term>WebServiceWSDLURL</term>
    ///			<description>The Url where the WSDL maybe obtained</description>
    ///		</item>
    ///		<item>
    ///			<term>ServiceName</term>
    ///			<description>The name of the Web Service to invoke</description>
    ///		</item>
    ///		<item>
    ///			<term>WebMethod</term>
    ///			<description>The Web Method (opperation) to invoke.</description>
    ///		</item>
    ///		<item>
    ///			<term>InputMessageTypeName/ContextProperty</term>
    ///			<description>The type of the input message.</description>
    ///		</item>
    ///		<item>
    ///			<term>MessagePayload</term>
    ///			<description>The path to the input data, note, this is the serialized object.</description>
    ///		</item>
    ///		<item>
    ///			<term>ContextLoaderStep</term>
    ///			<description>The configuration for the context loader step used to load data into the BizUnit context which may be used by subsequent test steps<para>(optional)</para></description>
    ///		</item>
    ///		<item>
    ///			<term>ValidationStep</term>
    ///			<description>Optional validation step.</description>
    ///		</item>
    ///	</list>
    /// <para>
    /// The example below illustrates the calling of the this step for a web service which takes 
    /// and returns no paramters:
    /// <code escaped="true">
    /// <TestStep assemblyPath="" typeName="BizUnit.SOAPHTTPRequestResponseStep">
    /// 	<WebServiceWSDLURL>http://localhost/StockQuoteService/StockQuoteService.asmx?wsdl</WebServiceWSDLURL>
    /// 	<ServiceName>StockQuoteService</ServiceName>
    /// 	<WebMethod>VoidMethod</WebMethod>
    /// </TestStep>
    /// </code>
    /// </para>
    /// <para>
    /// The following example illustrates the use of this step in order to invoke a stock quote web service
    /// exposed via BizTalk.
    /// An example HTTP request packet is shown below:
    /// <code escaped="true">
    /// POST /StockQuoteService_Proxy/StockQuoteService_QuoteService_Port_1.asmx HTTP/1.1
    /// Host: localhost
    /// Content-Type: application/soap+xml; charset=utf-8
    /// Content-Length: length
    /// 
    /// <?xml version="1.0" encoding="utf-8"?>
    /// <soap12:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap12="http://www.w3.org/2003/05/soap-envelope">
    ///   <soap12:Body>
    ///     <Operation_1 xmlns="http://StockQuoteService.StockQuote">
    ///       <StockQuote>
    ///         <Symbol xmlns="">MSFT</Symbol>
    ///         <LastPrice xmlns=""></LastPrice>
    ///       </StockQuote>
    ///     </Operation_1>
    ///   </soap12:Body>
    /// </soap12:Envelope>
    /// </code>
    /// </para>
    /// <para>
    /// An example HTTP response packet is shown below:
    /// <code escaped="true">
    /// HTTP/1.1 200 OK
    /// Content-Type: application/soap+xml; charset=utf-8
    /// Content-Length: length
    /// 
    /// <?xml version="1.0" encoding="utf-8"?>
    /// <soap12:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap12="http://www.w3.org/2003/05/soap-envelope">
    ///   <soap12:Body>
    ///     <Operation_1Response xmlns="http://StockQuoteService.StockQuote">
    ///       <StockQuote>
    ///         <Symbol xmlns="">MSFT</Symbol>
    ///         <LastPrice xmlns="">29.29</LastPrice>
    ///       </StockQuote>
    ///     </Operation_1Response>
    ///   </soap12:Body>
    /// </soap12:Envelope>
    /// </code>
    /// </para>
    /// <para>
    /// An example test step for this web service is shown below:
    /// <code escaped="true">
    /// <TestStep assemblyPath="" typeName="BizUnit.SOAPHTTPRequestResponseStep">
    /// 
    /// 	<WebServiceWSDLURL>http://localhost/StockQuoteService_Proxy/StockQuoteService_QuoteService_Port_1.asmx?wsdl</WebServiceWSDLURL>
    /// 	<ServiceName>StockQuoteService_QuoteService_Port_1</ServiceName>
    /// 	<WebMethod>Operation_1</WebMethod>
    /// 	<InputMessageTypeName>Operation_1StockQuote</InputMessageTypeName>
    /// 	<MessagePayload>..\..\..\Test\BizUnit.Tests\Data\SOAPHTTPRequestResponse-RequestInput001.xml</MessagePayload>
    /// 
    /// 	<ValidationStep assemblyPath="" typeName="BizUnit.XmlValidationStep">
    /// 		<XPathList>
    /// 			<XPathValidation query="/Operation_1ResponseStockQuote/Symbol">MSFT</XPathValidation>
    /// 			<XPathValidation query="/Operation_1ResponseStockQuote/LastPrice">29.29</XPathValidation>
    /// 		</XPathList>
    /// 	</ValidationStep>
    /// 
    /// </TestStep>
    /// </code>
    /// </para>
    /// <para>
    /// The contents of the input file SOAPHTTPRequestResponse-RequestInput001.xml is show below:
    /// <code escaped="true">
    /// <Operation_1StockQuote xmlns="http://StockQuoteService.StockQuote">
    /// 	<Symbol xmlns="">MSFT</Symbol>
    /// 	<LastPrice xmlns=""></LastPrice>
    /// </Operation_1StockQuote>    
    /// </code>
    /// </para>
    ///	</remarks>	
    [Obsolete("SOAPHTTPRequestResponseStep has been deprecated. Investigate the BizUnit.TestSteps namespace.")]
    public class SOAPHTTPRequestResponseStep : ITestStep
    {
        /// <summary>
        /// ITestStep.Execute() implementation
        /// </summary>
        /// <param name='testConfig'>The Xml fragment containing the configuration for this test step</param>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public void Execute(XmlNode testConfig, Context context)
        {
            const string soapproxynamespace = "BizUnit.Proxy";
            Stream request = null;
            Stream response = null;

            // Turn on shadow copying of asseblies for the current appdomain. 
            AppDomain.CurrentDomain.SetShadowCopyFiles();

            try
            {
                string wsdlFile = context.ReadConfigAsString(testConfig, "WebServiceWSDLURL");
                string soapMessagePath = context.ReadConfigAsString(testConfig, "MessagePayload", true);
                string inputMessageTypeName = context.ReadConfigAsString(testConfig, "InputMessageTypeName", true);
                string webMethod = context.ReadConfigAsString(testConfig, "WebMethod");
                string serviceName = context.ReadConfigAsString(testConfig, "ServiceName");

                Assembly proxyAssembly = GetProxyAssembly(wsdlFile, soapproxynamespace);

                object objInputMessage = null;
                if(null != inputMessageTypeName && null != soapMessagePath)
                {
                    objInputMessage =
                        LoadMessage(proxyAssembly, soapproxynamespace + "." + inputMessageTypeName, soapMessagePath);

                    if (null != objInputMessage)
                    {
                        request = GetOutputStream(objInputMessage);
                        context.LogData("SOAPHTTPRequestResponseStep request data", request);
                    }
                }

                object proxy = Activator.CreateInstance(proxyAssembly.GetType(soapproxynamespace + "." + serviceName));

                MethodInfo mi = proxy.GetType().GetMethod(webMethod);

                context.LogInfo("SOAPHTTPRequestResponseStep about to post data from File: {0} to the Service: {1} defined in WSDL: {2}", soapMessagePath, serviceName, wsdlFile);

                object outputMessage;
                if (null != inputMessageTypeName && null != soapMessagePath)
                {
                    outputMessage = mi.Invoke(proxy, new[] { objInputMessage });
                }
                else
                {
                    outputMessage = mi.Invoke(proxy, null);
                }

                if (null != outputMessage)
                {
                    response = GetOutputStream(outputMessage);
                    context.LogData("SOAPHTTPRequestResponseStep response data", response);
                }

                // Execute ctx loader step if present...
                if (null != response)
                {
                    context.ExecuteContextLoader(response, testConfig.SelectSingleNode("ContextLoaderStep"), true);
                }

                // Validate the response...
                try
                {
                    context.ExecuteValidator(response, testConfig.SelectSingleNode("ValidationStep"), true);
                }
                catch (Exception e)
                {
                    throw new ApplicationException("SOAPHTTPRequestResponseStep response stream was not correct!", e);
                }
            }
            catch(Exception ex)
            {
                context.LogError("SOAPHTTPRequestResponseStep Failed");
                context.LogException(ex);
                throw;
            }
            finally
            {
                if (null != response)
                {
                    response.Close();
                }

                if (null != request)
                {
                    request.Close();
                }
            }
        }

        internal static Assembly GetProxyAssembly(string wsdlUri, string codeNamespace)
        {
            var provider = new CSharpCodeProvider();
            var client = new WebClient();
            var referenceAssemblies = new[] { "system.dll", "System.Xml.dll", "System.Web.Services.dll" };
            var wsdlStream = client.OpenRead(wsdlUri);

            var wsdl = ServiceDescription.Read(wsdlStream);
            var wsdlImport = new ServiceDescriptionImporter();
            wsdlImport.AddServiceDescription(wsdl, null, null);

            var proxyClassNamespace = new CodeNamespace(codeNamespace);
            var codeCompileUnit = new CodeCompileUnit();
            codeCompileUnit.Namespaces.Add(proxyClassNamespace);

            var warnings = wsdlImport.Import(proxyClassNamespace, codeCompileUnit);
            if (warnings != 0)
            {
                throw new ApplicationException("SOAPHTTPRequestResponseStep experienced problems while importing the WSDL!");
            }

            var compileParam = new CompilerParameters(referenceAssemblies)
                                   {
                                       GenerateInMemory = false,
                                       OutputAssembly = GetProxyFileName()
                                   };

            CompilerResults compilerResults = provider.CompileAssemblyFromDom(compileParam, codeCompileUnit);

            if (compilerResults.Errors.HasErrors)
            {
                throw new ApplicationException("SOAPHTTPRequestResponseStep experienced problems while executing CompileAssemblyFromDom");
            }

            provider.Dispose();

            return compilerResults.CompiledAssembly;
        }

        /// <summary>
        /// Returns a name that can be used for the proxy assembly.
        /// </summary>
        /// <returns></returns>
        internal static string GetProxyFileName()
        {
            const int max = 20;
            const string baseName = "TestFrameWorkProxy";
            string fname = "TestFrameWorkProxy.dll";

            for (int i = 0; i <= max; i++)
            {
                fname = baseName + i + ".dll";

                // try to delete the file if it exists
                try
                {
                    File.Delete(fname);
                }
                catch
                {
                }

                if (!File.Exists(fname))
                {
                    return fname;
                }
            }

            return fname;
        }

        internal static MemoryStream GetOutputStream(object outputMessage)
        {
            var ms = new MemoryStream();
            var outputSerializer = new XmlSerializer(outputMessage.GetType());
            outputSerializer.Serialize(ms, outputMessage);
            ms.Seek(0, SeekOrigin.Begin);

            return ms;
        }

        internal static string GetDefaultNamespace(Assembly assembly, string msgTypeName)
        {
            var t = assembly.GetType(msgTypeName);

            var ret = "";
            var attributes = t.GetCustomAttributes(false);

            for (var i = 0; i < attributes.Length; i++)
            {
                var att = (Attribute)attributes[i];

                if (att.ToString().Equals("System.Xml.Serialization.XmlTypeAttribute"))
                {
                    var typeAtt = (XmlTypeAttribute)att;
                    ret = typeAtt.Namespace;
                    break;
                }
            }

            return ret;
        }

        internal static object LoadMessage(Assembly assembly, string msgTypeName, string messagePath)
        {
            XmlReader messageReader = null;
            object objMessage;

            try
            {
                // This object requires CreateInstanceFrom...however the proxy object instantiation 
                // does not require CreateInstancefrom
                Activator.CreateInstanceFrom(assembly.Location, msgTypeName);
                messageReader = new XmlTextReader(messagePath);

                string defNamespace = GetDefaultNamespace(assembly, msgTypeName);

                var serializer = !string.IsNullOrEmpty(defNamespace) ? new XmlSerializer(assembly.GetType(msgTypeName), defNamespace) : new XmlSerializer(assembly.GetType(msgTypeName));

                objMessage = serializer.Deserialize(messageReader);
            }
            finally
            {
                if (messageReader != null)
                {
                    messageReader.Close();
                }
            }

            return objMessage;
        }
    }
}
