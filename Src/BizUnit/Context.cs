//---------------------------------------------------------------------
// File: Context.cs
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
using System.Collections;
using BizUnit.BizUnitOM;
using BizUnit.Common;

namespace BizUnit
{
    /// <summary>
    /// Represents a state object that is passed between BizUnit test steps.
    /// </summary>
    /// 
    /// <remarks>
    /// The context is passed by BizUnit to each individual test step, state maybe written to or read from the context, note the following
    /// wild cards are supported: %DateTime%, %DateTimeISO8601%, %ServerName% and %Guid%.
    /// The context also provides helper methods for test steps to read their configuration, and to log information, warnings, errors 
    /// and data in a consistent manner.
    /// The context contains some standard information on the current test executing, such as BizUnitTestCaseName which contains the 
    /// name of the current test case being executed and BizUnitTestCaseStartTime which contains the time the current test case started.
    ///	</remarks>
    public class Context
    {
        private readonly Hashtable _context;
        private BizUnit _bizUnit;
        private readonly ILogger _logger;
        private readonly DateTime _startTime;
        private const string DateTime = "%DateTime%";
        private const string DateTimeIso8601 = "%DateTimeISO8601%";
        private const string ServerName = "%ServerName%";
        private const string Guid = "%Guid%";
        private const string TestStartDateTime = "%Test_Start_DateTime%";
        private bool _disposeMembersOnTestCaseCompletion = false;

        public const string TakeFromContext = "takeFromCtx:";

        /// <summary>
        /// Default Context constructor.
        /// </summary>
        /// 
        /// <remarks>
        /// This may be used for scenarios where by the context object needs 
        /// to be created and passed to BizUnit.
        ///	</remarks>
        public Context()
        {
            _context = new Hashtable();
            _startTime = System.DateTime.Now;
            _logger = new Logger();
        }

        internal Context(BizUnit bizUnit)
        {
            _bizUnit = bizUnit;
            _context = new Hashtable();
            _logger = new Logger();
            _startTime = System.DateTime.Now;
        }

        public Context(ILogger logger)
        {
            _logger = logger;
            _context = new Hashtable();
            _startTime = System.DateTime.Now;
        }

        internal void SetTestStage(TestStage currentStage)
        {
            CurrentTestStage = currentStage;
        }

        public TestStage CurrentTestStage { get; private set; }
        public string TestName { get; private set; }

        internal void SetTestName(string currentTestName)
        {
            TestName = currentTestName;
        }

        internal Context(BizUnit bizUnit, ILogger logger)
        {
            _bizUnit = bizUnit;
            _logger = logger;
            _context = new Hashtable();
            _startTime = System.DateTime.Now;
        }

        private Context(BizUnit bizUnit, Hashtable ctx, ILogger logger, DateTime t)
        {
            _bizUnit = bizUnit;
            _logger = logger;
            _context = ctx;
            _startTime = t;
        }

        internal ILogger Logger
        {
            get
            {
                return _logger;
            }
        }

        internal BizUnit BizUnitObject
        {
            get
            {
                return _bizUnit;
            }
        }

        internal void Initialize(BizUnit bizunit)
        {
            ArgumentValidation.CheckForNullReference(bizunit, "bizunit");
            _bizUnit = bizunit;
        }

        internal Context CloneForConcurrentUse(ILogger logger)
        {
            return new Context(_bizUnit, _context, logger, _startTime);
        }

        internal void Teardown()
        {
            if(_disposeMembersOnTestCaseCompletion)
            {
                DisposeDisposableMembers();   
            }
        }

        /// <summary>
        /// Adds a new object to the context.
        /// </summary>
        /// <param name="key">The name of the key for the object added.</param>
        /// <param name="newValue">The object to be added to the context.</param>
        /// 
        /// <remarks>
        /// The following example demonstrates how to add a new item to the context:
        /// 
        /// <code escaped="true">
        /// context.Add("FILE_NAME", fileName);
        ///	</code>
        ///	
        ///	</remarks>
        public void Add(string key, object newValue)
        {
            ArgumentValidation.CheckForNullReference(key, "key");
            Add(key, newValue, false);
        }

        /// <summary>
        /// Adds a new object to the context.
        /// </summary>
        /// <param name="key">The name of the key for the object added.</param>
        /// <param name="newValue">The object to be added to the context.</param>
        /// <param name="updateIfExists">If the object already exists and this flag 
        /// is set to true, its value will be updated, otherwise the method will 
        /// throw an exception.</param>
        /// 
        /// <remarks>
        /// The following example demonstrates how to add a new item to the context:
        /// 
        /// <code escaped="true">
        /// context.Add("FILE_NAME", fileName);
        ///	</code>
        ///	
        ///	</remarks>
        public void Add(string key, object newValue, bool updateIfExists)
        {
            ArgumentValidation.CheckForNullReference(key, "key");

            LogInfo(string.Format("Adding context property: {0}, value: {1}", key, newValue));

            lock (_context.SyncRoot)
            {
                if (updateIfExists && _context.ContainsKey(key))
                {
                    LogInfo(string.Format("Adding context property: {0} already existed, property will be updated", key));
                    _context.Remove(key);
                }
                _context.Add(key, newValue);
            }
        }

        /// <summary>
        /// Removes an object from the context.
        /// </summary>
        /// <param name="key">The name of the key for the object to remove.</param>
        /// 
        /// <remarks>
        /// The following example demonstrates how to remove an item from the context:
        /// 
        /// <code escaped="true">
        /// context.Remove("FILE_NAME");
        ///	</code>
        ///	
        ///	</remarks>
        public void Remove(string key)
        {
            ArgumentValidation.CheckForNullReference(key, "key");

            LogInfo(string.Format("Removing context property: {0}", key));

            lock (_context.SyncRoot)
            {
                    _context.Remove(key);
            }
        }

        /// <summary>
        /// Gets a string previously saved on the context.
        /// </summary>
        /// <param name="key">The name of the key for the object to get.</param>
        /// <returns>string value</returns>
        /// 
        /// <remarks>
        /// The following example demonstrates how to get a string value from the context:
        /// 
        /// <code escaped="true">
        /// string fileName = context.GetValue("FILE_NAME");
        ///	</code>
        ///	
        ///	</remarks>
        public string GetValue(string key)
        {
            ArgumentValidation.CheckForNullReference(key, "key");

            lock (_context.SyncRoot)
            {
                return (string)_context[key];
            }
        }

        /// <summary>
        /// Gets an object previously saved to the context .
        /// </summary>
        /// <param name="key">The name of the key for the object to get.</param>
        /// <returns>object</returns>
        /// 
        /// <remarks>
        /// The following example demonstrates how to get an object from the context:
        /// 
        /// <code escaped="true">
        /// object fileName = context.GetObject("FILE_NAME");
        ///	</code>
        ///	
        ///	</remarks>
        public object GetObject(string key)
        {
            ArgumentValidation.CheckForNullReference(key, "key");

            lock (_context.SyncRoot)
            {
                return _context[key];
            }
        }

        /// <summary>
        /// Executes a validation test step.
        /// </summary>
        /// <param name="data">The stream containing the data to validate.</param>
        /// <param name="validationStep">The IValidationStepOM to be used for the validation test step.</param>
        [Obsolete("ExecuteValidator has been deprecated.")]
        public void ExecuteValidator(Stream data, IValidationStepOM validationStep)
        {
            _bizUnit.ExecuteValidator(data, validationStep, this);
        }

        /// <summary>
        /// Executes a validation test step.
        /// </summary>
        /// <param name="data">The stream containing the data to validate.</param>
        /// <param name="validatorConfig">The Xml configuration for the validation test step, this configuration is passed onto the validation step by BizUnit.</param>
        /// 
        /// <remarks>
        /// The following example demonstrates how to execute a validation test step:
        /// 
        /// <code escaped="true">
        /// XmlNode validationConfig = testConfig.SelectSingleNode("ValidationStep");
        /// 
        /// ...
        /// 
        /// MemoryStream response = HttpHelper.SendRequestData(destinationUrl, data, requestTimeout, context);
        /// context.ExecuteValidator( response, validationConfig );
        ///	</code>
        ///	
        ///	</remarks>
        [Obsolete("ExecuteValidator has been deprecated.")]
        public void ExecuteValidator(Stream data, XmlNode validatorConfig)
        {
            _bizUnit.ExecuteValidator(data, validatorConfig, this);
        }

        /// <summary>
        /// Executes a validation test step, with the option to seek the data stream to the beginning
        /// </summary>
        /// <param name="data">The stream containing the data to validate.</param>
        /// <param name="validationStep">The IValidationStepOM to be used for the validation test step (optional).</param>
        /// <param name="seekStream">True if the stream should be seeked to the begining.</param>
        [Obsolete("ExecuteValidator has been deprecated.")]
        public void ExecuteValidator(Stream data, IValidationStepOM validationStep, bool seekStream)
        {
            if (seekStream && null != data)
            {
                data.Seek(0, SeekOrigin.Begin);
            }

            _bizUnit.ExecuteValidator(data, validationStep, this);
        }
        
        /// <summary>
        /// Executes a validation test step, with the option to seek the data stream to the beginning
        /// </summary>
        /// <param name="data">The stream containing the data to validate.</param>
        /// <param name="validatorConfig">The Xml configuration for the validation test step, this configuration is passed onto the validation step by BizUnit.</param>
        /// <param name="seekStream">True if the stream should be seeked to the begining.</param>
        /// 
        /// <remarks>
        /// The following example demonstrates how to execute a validation test step:
        /// 
        /// <code escaped="true">
        /// XmlNode validationConfig = testConfig.SelectSingleNode("ValidationStep");
        /// 
        /// ...
        /// 
        /// MemoryStream response = HttpHelper.SendRequestData(destinationUrl, data, requestTimeout, context);
        /// context.ExecuteValidator( response, validationConfig, true );
        ///	</code>
        ///	
        ///	</remarks>
        [Obsolete("ExecuteValidator has been deprecated.")]
        public void ExecuteValidator(Stream data, XmlNode validatorConfig, bool seekStream)
        {
            if (seekStream && null != data)
            {
                data.Seek(0, SeekOrigin.Begin);
            }

            _bizUnit.ExecuteValidator(data, validatorConfig, this);
        }

        /// <summary>
        /// Executes a validation test step, either the Xml configuration for the step or the BizUnit OM maybe supplied, but not both.
        /// </summary>
        /// <param name="data">The stream containing the data to validate.</param>
        /// <param name="validatorConfig">The Xml configuration for the validation test step, this configuration is passed onto the validation step by BizUnit (optional).</param>
        /// <param name="validationStep">The validation step implementing IValidationStepOM to execute (optional).</param>
        [Obsolete("ExecuteValidator has been deprecated.")]
        public void ExecuteValidator(Stream data, XmlNode validatorConfig, IValidationStepOM validationStep)
        {
            ExecuteValidator(data, validatorConfig, validationStep, false);
        }

        /// <summary>
        /// Executes a validation test step, either the Xml configuration for the step or the BizUnit OM maybe supplied, but not both.
        /// </summary>
        /// <param name="data">The stream containing the data to validate.</param>
        /// <param name="validatorConfig">The Xml configuration for the validation test step, this configuration is passed onto the validation step by BizUnit (optional).</param>
        /// <param name="validationStep">The validation step implementing IValidationStepOM to execute (optional).</param>
        /// <param name="seekStream">True if the stream should be seeked to the begining.</param>
        [Obsolete("ExecuteValidator has been deprecated.")]
        public void ExecuteValidator(Stream data, XmlNode validatorConfig, IValidationStepOM validationStep, bool seekStream)
        {
            if (null != validationStep && null != validatorConfig)
            {
                throw new ApplicationException(
                    "Cannot execute a validation step using both Xml configuration and the BizUnit OM");
            }

            ExecuteValidator(data, validationStep, seekStream);
            ExecuteValidator(data, validatorConfig, seekStream);
        }
        
        /// <summary>
        /// Executes a context loader test step.
        /// </summary>
        /// <param name="data">The stream containing the data to validate.</param>
        /// <param name="contextConfig">The Xml configuration for the context loader 
        /// test step, this configuration is passed onto the validation step by BizUnit (optional).</param>
        /// 
        /// <remarks>
        /// Context loader steps load data onto the context. The following example demonstrates how to execute a validation test step:
        /// 
        /// <code escaped="true">
        /// XmlNode contextConfig = testConfig.SelectSingleNode("ContextConfig");
        /// 
        /// ...
        /// 
        /// MemoryStream response = HttpHelper.SendRequestData(destinationUrl, data, requestTimeout, context);
        /// context.ExecuteContextLoader( response, contextConfig );
        ///	</code>
        ///	
        ///	</remarks>
        [Obsolete("ExecuteContextLoader has been deprecated.")]
        public void ExecuteContextLoader(Stream data, XmlNode contextConfig)
        {
            _bizUnit.ExecuteContextLoader(data, contextConfig, this);
        }

        /// <summary>
        /// Executes a validation test step, either the Xml configuration for the step or the BizUnit OM maybe supplied, but not both.
        /// </summary>
        /// <param name="data">The stream containing the data to validate.</param>
        /// <param name="contextConfig">The Xml configuration for the context loader step, this configuration is passed onto the context loader step by BizUnit (optional).</param>
        /// <param name="contextLoaderStep">The context loader step implementing IContextLoaderStepOM to execute (optional).</param>
        [Obsolete("ExecuteContextLoader has been deprecated.")]
        public void ExecuteContextLoader(Stream data, XmlNode contextConfig, IContextLoaderStepOM contextLoaderStep)
        {
            ExecuteContextLoader(data, contextConfig, contextLoaderStep, false);
        }

        /// <summary>
        /// Executes a validation test step, either the Xml configuration for the step or the BizUnit OM maybe supplied, but not both.
        /// </summary>
        /// <param name="data">The stream containing the data to validate.</param>
        /// <param name="contextConfig">The Xml configuration for the context loader step, this configuration is passed onto the context loader step by BizUnit (optional).</param>
        /// <param name="contextLoaderStep">The context loader step implementing IContextLoaderStepOM to execute (optional).</param>
        /// <param name="seekStream">True if the stream should be seeked to the begining.</param>
        [Obsolete("ExecuteContextLoader has been deprecated.")]
        public void ExecuteContextLoader(Stream data, XmlNode contextConfig, IContextLoaderStepOM contextLoaderStep, bool seekStream)
        {
            if (null != contextLoaderStep && null != contextConfig)
            {
                throw new ApplicationException(
                    "Cannot execute a context loader step using both Xml configuration and the BizUnit OM");
            }

            ExecuteContextLoader(data, contextLoaderStep, seekStream);
            ExecuteContextLoader(data, contextConfig, seekStream);
        }

        /// <summary>
        /// Executes a context loader test step, , with the option to seek the data stream to the beginning.
        /// </summary>
        /// <param name="data">The stream containing the data to validate.</param>
        /// <param name="contextConfig">The Xml configuration for the context loader 
        /// test step, this configuration is passed onto the validation step by BizUnit.</param>
        /// <param name="seekStream">True if the stream should be seeked to the begining.</param>
        /// 
        /// <remarks>
        /// Context loader steps load data onto the context. The following example demonstrates how to execute a validation test step:
        /// 
        /// <code escaped="true">
        /// XmlNode contextConfig = testConfig.SelectSingleNode("ContextConfig");
        /// 
        /// ...
        /// 
        /// MemoryStream response = HttpHelper.SendRequestData(destinationUrl, data, requestTimeout, context);
        /// context.ExecuteContextLoader( response, contextConfig );
        ///	</code>
        ///	
        ///	</remarks>
        [Obsolete("ExecuteContextLoader has been deprecated.")]
        public void ExecuteContextLoader(Stream data, XmlNode contextConfig, bool seekStream)
        {
            if (seekStream && null != data)
            {
                data.Seek(0, SeekOrigin.Begin);
            }

            _bizUnit.ExecuteContextLoader(data, contextConfig, this);
        }

        /// <summary>
        /// Executes a context loader step
        /// </summary>
        /// <param name="data">The stream containing the data to validate.</param>
        /// <param name="contextLoaderStep">The context loader step implmenting IContextLoaderStepOM which will be executed (optional).</param>
        [Obsolete("ExecuteContextLoader has been deprecated.")]
        public void ExecuteContextLoader(Stream data, IContextLoaderStepOM contextLoaderStep)
        {
            ExecuteContextLoader(data, contextLoaderStep, false);
        }

        /// <summary>
        /// Executes a context loader step, with the option to seek the data stream to the beginning
        /// </summary>
        /// <param name="data">The stream containing the data to validate.</param>
        /// <param name="contextLoaderStep">The context loader step implmenting IContextLoaderStepOM which will be executed (optional).</param>
        /// <param name="seekStream">True if the stream should be seeked to the begining.</param>
        [Obsolete("ExecuteContextLoader has been deprecated.")]
        public void ExecuteContextLoader(Stream data, IContextLoaderStepOM contextLoaderStep, bool seekStream)
        {
            if (seekStream && null != data)
            {
                data.Seek(0, SeekOrigin.Begin);
            }

            _bizUnit.ExecuteContextLoader(data, contextLoaderStep, this);
        }

        private string ReadConfigAsString(XmlNode config, string xPath, bool optional, bool asXml)
        {
            return (string)ReadConfigAsObject(config, xPath, optional, asXml);
        }

        /// <summary>
        /// Used by a test step to read an object, if the object is in the context 
        /// the object will be returned, otherwise the string value from the Xml configuration 
        /// will be returned.
        /// </summary>
        /// <param name="config">The Xml configuration for the test step.</param>
        /// <param name="xPath">The XPath expression used to query for the configuration value.</param>
        /// <param name="optional">True if this is an option field. If false and the XPath expression does not find a value and exception will be thrown.</param>
        /// <returns>string value</returns>
        /// 
        /// <remarks>
        /// The following example demonstrates how to use the method:
        /// 
        /// <code escaped="true">
        ///	public void Execute(XmlNode testConfig, Context context)
        ///	{
        ///		object fooObj = context.ReadConfigAsObject( testConfig, "Foo", true );
        ///	</code>
        ///	
        ///	</remarks>
        [Obsolete("ReadConfigAsXXX has been deprecated. From 4.0 test steps are configured using property setters and getters.")]
        public object ReadConfigAsObject(XmlNode config, string xPath, bool optional)
        {
            ArgumentValidation.CheckForNullReference(config, "config");
            ArgumentValidation.CheckForNullReference(xPath, "xPath");
            ArgumentValidation.CheckForNullReference(optional, "optional");

            return ReadConfigAsObject(config, xPath, optional, false);
        }

        /// <summary>
        /// Used by a test step to read an object, if the object is in the context 
        /// the object will be returned, otherwise the string value from the Xml configuration 
        /// will be returned.
        /// </summary>
        /// <param name="config">The Xml configuration for the test step.</param>
        /// <param name="xPath">The XPath expression used to query for the configuration value.</param>
        /// <returns>string value</returns>
        /// 
        /// <remarks>
        /// The following example demonstrates how to use the method:
        /// 
        /// <code escaped="true">
        ///	public void Execute(XmlNode testConfig, Context context)
        ///	{
        ///		object fooObj = context.ReadConfigAsObject( testConfig, "Foo" );
        ///	</code>
        ///	
        ///	</remarks>
        [Obsolete("ReadConfigAsXXX has been deprecated. From 4.0 test steps are configured using property setters and getters.")]
        public object ReadConfigAsObject(XmlNode config, string xPath)
        {
            ArgumentValidation.CheckForNullReference(config, "config");
            ArgumentValidation.CheckForNullReference(xPath, "xPath");

            return ReadConfigAsObject(config, xPath, false);
        }

        private object ReadConfigAsObject(XmlNode config, string xPath, bool optional, bool asXml)
        {
            XmlNode node = config.SelectSingleNode(xPath);

            if (null == node && optional)
            {
                return null;
            }

            if (null != node)
            {
                // Take the value from the ctx?
                XmlNode fromCtx = node.SelectSingleNode("@takeFromCtx");
                if (null != fromCtx)
                {
                    string s = fromCtx.Value;
                    if (0 < s.Length)
                    {
                        lock (_context.SyncRoot)
                        {
                            return _context[s];
                        }
                    }
                }

                if (asXml)
                {
                    return SubstituteWildCards(node.InnerXml);
                }

                    return SubstituteWildCards(node.InnerText);
            }

            throw new NullReferenceException(string.Format("The XPath query: {0} did not find a node.", xPath));
        }

        public object ReadArgument(object arg)
        {
            ArgumentValidation.CheckForNullReference(arg, "arg");

            if (arg is System.String)
            {
                string strArg = (string) arg;
                if(strArg.Contains(TakeFromContext))
                {
                    string key = strArg.Substring(TakeFromContext.Length);
                    return _context[key];
                }

                return arg;
            }

            return arg;                
        }

        /// <summary>
        /// Used by a test step to read a configuration string value as Xml from it's Xml configuration.
        /// </summary>
        /// <param name="config">The Xml configuration for the test step.</param>
        /// <param name="xPath">The XPath expression used to query for the configuration value.</param>
        /// <returns>string value</returns>
        /// 
        /// <remarks>
        /// The following example demonstrates how to use the method:
        /// 
        /// <code escaped="true">
        ///	public void Execute(XmlNode testConfig, Context context)
        ///	{
        ///		string directory = context.ReadConfigAsXml( testConfig, "Directory" );
        ///	</code>
        ///	
        ///	</remarks>
        [Obsolete("ReadConfigAsXXX has been deprecated. From 4.0 test steps are configured using property setters and getters.")]
        public string ReadConfigAsXml(XmlNode config, string xPath)
        {
            ArgumentValidation.CheckForNullReference(config, "config");
            ArgumentValidation.CheckForNullReference(xPath, "xPath");

            return ReadConfigAsString(config, xPath, false, true);
        }

        /// <summary>
        /// Used by a test step to read a configuration string value as Xml from it's Xml configuration.
        /// </summary>
        /// <param name="config">The Xml configuration for the test step.</param>
        /// <param name="xPath">The XPath expression used to query for the configuration value.</param>
        /// <param name="optional">True if this is an option field. If false and the XPath expression does not find a value and exception will be thrown.</param>
        /// <returns>string value</returns>
        /// 
        /// <remarks>
        /// The following example demonstrates how to use the method:
        /// 
        /// <code escaped="true">
        ///	public void Execute(XmlNode testConfig, Context context)
        ///	{
        ///		string directory = context.ReadConfigAsXml( testConfig, "Directory", true );
        ///	</code>
        ///	
        ///	</remarks>
        [Obsolete("ReadConfigAsXXX has been deprecated. From 4.0 test steps are configured using property setters and getters.")]
        public string ReadConfigAsXml(XmlNode config, string xPath, bool optional)
        {
            ArgumentValidation.CheckForNullReference(config, "config");
            ArgumentValidation.CheckForNullReference(xPath, "xPath");
            ArgumentValidation.CheckForNullReference(optional, "optional");

            return ReadConfigAsString(config, xPath, optional, true);
        }

        /// <summary>
        /// Used by a test step to read a configuration string value from it's Xml configuration.
        /// </summary>
        /// <param name="config">The Xml configuration for the test step.</param>
        /// <param name="xPath">The XPath expression used to query for the configuration value.</param>
        /// <returns>string value</returns>
        /// 
        /// <remarks>
        /// The following example demonstrates how to use the method:
        /// 
        /// <code escaped="true">
        ///	public void Execute(XmlNode testConfig, Context context)
        ///	{
        ///		string directory = context.ReadConfigAsString( testConfig, "Directory" );
        ///	</code>
        ///	
        ///	</remarks>
        [Obsolete("ReadConfigAsXXX has been deprecated. From 4.0 test steps are configured using property setters and getters.")]
        public string ReadConfigAsString(XmlNode config, string xPath)
        {
            ArgumentValidation.CheckForNullReference(config, "config");
            ArgumentValidation.CheckForNullReference(xPath, "xPath");

            return ReadConfigAsString(config, xPath, false, false);
        }

        /// <summary>
        /// Used by a test step to read a configuration string value from it's Xml configuration.
        /// </summary>
        /// <param name="config">The Xml configuration for the test step.</param>
        /// <param name="xPath">The XPath expression used to query for the configuration value.</param>
        /// <param name="optional">True if this is an option field. If false and the XPath expression does not find a value and exception will be thrown.</param>
        /// <returns>string value</returns>
        /// 
        /// <remarks>
        /// The following example demonstrates how to use the method:
        /// 
        /// <code escaped="true">
        ///	public void Execute(XmlNode testConfig, Context context)
        ///	{
        ///		string directory = context.ReadConfigAsString( testConfig, "Directory", true );
        ///	</code>
        ///	
        ///	</remarks>
        [Obsolete("ReadConfigAsXXX has been deprecated. From 4.0 test steps are configured using property setters and getters.")]
        public string ReadConfigAsString(XmlNode config, string xPath, bool optional)
        {
            ArgumentValidation.CheckForNullReference(config, "config");
            ArgumentValidation.CheckForNullReference(xPath, "xPath");
            ArgumentValidation.CheckForNullReference(optional, "optional");

            return ReadConfigAsString(config, xPath, optional, false);
        }

        /// <summary>
        /// Used by a test step to read a configuration Int32 value from it's Xml configuration.
        /// </summary>
        /// <param name="config">The Xml configuration for the test step.</param>
        /// <param name="xPath">The XPath expression used to query for the configuration value.</param>
        /// <param name="optional">True if this is an option field. If false and the XPath expression does not find a value and exception will be thrown.</param>
        /// <returns>int value</returns>
        /// 
        /// <remarks>
        /// The following example demonstrates how to use the method:
        /// 
        /// <code escaped="true">
        ///	public void Execute(XmlNode testConfig, Context context)
        ///	{
        ///		int retryCount = context.ReadConfigAsInt32( testConfig, "RetryCount" );
        ///	</code>
        ///	
        ///	</remarks>
        [Obsolete("ReadConfigAsXXX has been deprecated. From 4.0 test steps are configured using property setters and getters.")]
        public int ReadConfigAsInt32(XmlNode config, string xPath, bool optional)
        {
            ArgumentValidation.CheckForNullReference(config, "config");
            ArgumentValidation.CheckForNullReference(xPath, "xPath");
            ArgumentValidation.CheckForNullReference(optional, "optional");

            return Convert.ToInt32(ReadConfigAsString(config, xPath, optional, false));
        }

        /// <summary>
        /// Used by a test step to read a configuration Int32 value from it's Xml configuration.
        /// </summary>
        /// <param name="config">The Xml configuration for the test step.</param>
        /// <param name="xPath">The XPath expression used to query for the configuration value.</param>
        /// <returns>int value</returns>
        /// 
        /// <remarks>
        /// The following example demonstrates how to use the method:
        /// 
        /// <code escaped="true">
        ///	public void Execute(XmlNode testConfig, Context context)
        ///	{
        ///		int retryCount = context.ReadConfigAsInt32( testConfig, "RetryCount", true );
        ///	</code>
        ///	
        ///	</remarks>
        [Obsolete("ReadConfigAsXXX has been deprecated. From 4.0 test steps are configured using property setters and getters.")]
        public int ReadConfigAsInt32(XmlNode config, string xPath)
        {
            ArgumentValidation.CheckForNullReference(config, "config");
            ArgumentValidation.CheckForNullReference(xPath, "xPath");

            return Convert.ToInt32(ReadConfigAsObject(config, xPath, false, false));
        }

        /// <summary>
        /// Used by a test step to read a configuration Int32 value from it's Xml configuration.
        /// </summary>
        /// <param name="config">The Xml configuration for the test step.</param>
        /// <param name="xPath">The XPath expression used to query for the configuration value.</param>
        /// <returns>uint value</returns>
        /// 
        /// <remarks>
        /// The following example demonstrates how to use the method:
        /// 
        /// <code escaped="true">
        ///	public void Execute(XmlNode testConfig, Context context)
        ///	{
        ///		uint retryCount = context.ReadConfigAsUInt32( testConfig, "RetryCount", true );
        ///	</code>
        ///	
        ///	</remarks>
        [Obsolete("ReadConfigAsXXX has been deprecated. From 4.0 test steps are configured using property setters and getters.")]
        public uint ReadConfigAsUInt32(XmlNode config, string xPath)
        {
            ArgumentValidation.CheckForNullReference(config, "config");
            ArgumentValidation.CheckForNullReference(xPath, "xPath");

            return Convert.ToUInt32(ReadConfigAsObject(config, xPath, false, false));
        }

        /// <summary>
        /// Used by a test step to read a configuration double value from it's Xml configuration.
        /// </summary>
        /// <param name="config">The Xml configuration for the test step.</param>
        /// <param name="xPath">The XPath expression used to query for the configuration value.</param>
        /// <param name="optional">True if this is an option field. If false and the XPath expression does not find a value and exception will be thrown.</param>
        /// <returns>double value</returns>
        /// 
        /// <remarks>
        /// The following example demonstrates how to use the method:
        /// 
        /// <code escaped="true">
        ///	public void Execute(XmlNode testConfig, Context context)
        ///	{
        ///		double timeout = context.ReadConfigAsDouble( testConfig, "Timeout" );
        ///	</code>
        ///	
        ///	</remarks>
        [Obsolete("ReadConfigAsXXX has been deprecated. From 4.0 test steps are configured using property setters and getters.")]
        public double ReadConfigAsDouble(XmlNode config, string xPath, bool optional)
        {
            ArgumentValidation.CheckForNullReference(config, "config");
            ArgumentValidation.CheckForNullReference(xPath, "xPath");
            ArgumentValidation.CheckForNullReference(optional, "optional");

            return Convert.ToDouble(ReadConfigAsObject(config, xPath, optional, false));
        }

        /// <summary>
        /// Used by a test step to read a configuration double value from it's Xml configuration.
        /// </summary>
        /// <param name="config">The Xml configuration for the test step.</param>
        /// <param name="xPath">The XPath expression used to query for the configuration value.</param>
        /// <returns>double value</returns>
        /// 
        /// <remarks>
        /// The following example demonstrates how to use the method:
        /// 
        /// <code escaped="true">
        ///	public void Execute(XmlNode testConfig, Context context)
        ///	{
        ///		double timeout = context.ReadConfigAsDouble( testConfig, "Timeout", true );
        ///	</code>
        ///	
        ///	</remarks>
        [Obsolete("ReadConfigAsXXX has been deprecated. From 4.0 test steps are configured using property setters and getters.")]
        public double ReadConfigAsDouble(XmlNode config, string xPath)
        {
            ArgumentValidation.CheckForNullReference(config, "config");
            ArgumentValidation.CheckForNullReference(xPath, "xPath");

            return Convert.ToDouble(ReadConfigAsObject(config, xPath, false, false));
        }

        /// <summary>
        /// Used by a test step to read a configuration float value from it's Xml configuration.
        /// </summary>
        /// <param name="config">The Xml configuration for the test step.</param>
        /// <param name="xPath">The XPath expression used to query for the configuration value.</param>
        /// <returns>double value</returns>
        /// 
        /// <remarks>
        /// The following example demonstrates how to use the method:
        /// 
        /// <code escaped="true">
        ///	public void Execute(XmlNode testConfig, Context context)
        ///	{
        ///		float timeout = context.ReadConfigAsFloat( testConfig, "Timeout", true );
        ///	</code>
        ///	
        ///	</remarks>
        [Obsolete("ReadConfigAsXXX has been deprecated. From 4.0 test steps are configured using property setters and getters.")]
        public float ReadConfigAsFloat(XmlNode config, string xPath)
        {
            ArgumentValidation.CheckForNullReference(config, "config");
            ArgumentValidation.CheckForNullReference(xPath, "xPath");

            return (float)Convert.ToDouble(ReadConfigAsObject(config, xPath, false, false));
        }

        /// <summary>
        /// Used by a test step to read a configuration bool value from it's Xml configuration.
        /// </summary>
        /// <param name="config">The Xml configuration for the test step.</param>
        /// <param name="xPath">The XPath expression used to query for the configuration value.</param>
        /// <param name="optional">True if this is an option field. If false and the XPath expression does not find a value and exception will be thrown.</param>
        /// <returns>bool value</returns>
        /// 
        /// <remarks>
        /// The following example demonstrates how to use the method:
        /// 
        /// <code escaped="true">
        ///	public void Execute(XmlNode testConfig, Context context)
        ///	{
        ///		bool findFirst = context.ReadConfigAsBool( testConfig, "FindFirst", true );
        ///	</code>
        ///	
        ///	</remarks>
        [Obsolete("ReadConfigAsXXX has been deprecated. From 4.0 test steps are configured using property setters and getters.")]
        public bool ReadConfigAsBool(XmlNode config, string xPath, bool optional)
        {
            ArgumentValidation.CheckForNullReference(config, "config");
            ArgumentValidation.CheckForNullReference(xPath, "xPath");
            ArgumentValidation.CheckForNullReference(optional, "optional");

            return Convert.ToBoolean(ReadConfigAsObject(config, xPath, optional, false));
        }

        /// <summary>
        /// Used by a test step to read a configuration bool value from it's Xml configuration.
        /// </summary>
        /// <param name="config">The Xml configuration for the test step.</param>
        /// <param name="xPath">The XPath expression used to query for the configuration value.</param>
        /// <returns>bool value</returns>
        /// 
        /// <remarks>
        /// The following example demonstrates how to use the method:
        /// 
        /// <code escaped="true">
        ///	public void Execute(XmlNode testConfig, Context context)
        ///	{
        ///		bool findFirst = context.ReadConfigAsBool( testConfig, "FindFirst", true );
        ///	</code>
        ///	
        ///	</remarks>
        [Obsolete("ReadConfigAsXXX has been deprecated. From 4.0 test steps are configured using property setters and getters.")]public bool ReadConfigAsBool(XmlNode config, string xPath)
        {
            ArgumentValidation.CheckForNullReference(config, "config");
            ArgumentValidation.CheckForNullReference(xPath, "xPath");

            return Convert.ToBoolean(ReadConfigAsObject(config, xPath, false, false));
        }

        /// <summary>
        /// Used by a test step to read the inner xml of an XmlNode, if the node has the 
        /// BizUnit attribute takeFromCtx set, the value will be fetched form the BizUnit 
        /// context.
        /// </summary>
        /// <param name="node">The XmlNode to fetch the inner xml from.</param>
        /// <returns>string</returns>
        /// 
        /// <remarks>
        /// The following example demonstrates how to use the method:
        /// 
        /// <code escaped="true">
        ///	public void Execute(XmlNode testConfig, Context context)
        ///	{
        ///     XmlNodeList parameters = testConfig.SelectNodes("Parameter");
        ///     
        ///     ....
        ///     
        ///     foreach (XmlNode paramter in parameters)
        ///     {
        ///        string val = context.GetInnerXml( node );
        ///        ...
        ///     }
        ///	</code>
        ///	
        ///	</remarks>
        [Obsolete("ReadConfigAsXXX has been deprecated. From 4.0 test steps are configured using property setters and getters.")]
        public string GetInnerXml(XmlNode node)
        {
            ArgumentValidation.CheckForNullReference(node, "node");

            XmlNode fromCtx = node.SelectSingleNode("@takeFromCtx");
            if (null != fromCtx)
            {
                string s = fromCtx.Value;
                if (0 < s.Length)
                {
                    lock (_context.SyncRoot)
                    {
                        return (string)_context[s];
                    }
                }
            }

            return node.InnerXml;
        }

        /// <summary>
        /// Used by a test step to log an Exception caught by the test step, this will be logged in the test output.
        /// </summary>
        /// <param name="e">The Exception to be logged by BizUnit.</param>
        /// 
        /// <remarks>
        /// The following example demonstrates how to use the method:
        /// 
        /// <code escaped="true">
        ///	public void Execute(XmlNode testConfig, Context context)
        ///	{
        ///	
        ///		try
        ///		{
        ///			...
        ///		}
        ///		catch(Exception ex)
        ///		{
        ///			context.LogException( ex );
        ///		}
        ///	</code>
        ///	
        ///	</remarks>
        public void LogException(Exception e)
        {
            ArgumentValidation.CheckForNullReference(e, "e");

            _logger.LogException(e);
        }

        /// <summary>
        /// Used by a test step to log test Data, this will be logged in the test output.
        /// </summary>
        /// <param name="description">The description of what the data being logged is.</param>
        /// <param name="data">The data to log.</param>
        /// 
        /// <remarks>
        /// The following example demonstrates how to use the method:
        /// 
        /// <code escaped="true">
        ///	public void Execute(XmlNode testConfig, Context context)
        ///	{
        ///	
        ///		...
        ///		context.LogData( "HTTP Response:", data );
        ///	</code>
        ///	
        ///	</remarks>
        public void LogData(string description, string data)
        {
            ArgumentValidation.CheckForNullReference(description, "description");
            ArgumentValidation.CheckForNullReference(data, "data");

            _logger.LogData(description, data);
        }

        /// <summary>
        /// Used by a test step to log test Data, this will be logged in the test output.
        /// </summary>
        /// <param name="description">The description of what the data being logged is.</param>
        /// <param name="data">The stream containing the data to log.</param>
        /// 
        /// <remarks>
        /// The following example demonstrates how to use the method:
        /// 
        /// <code escaped="true">
        ///	public void Execute(XmlNode testConfig, Context context)
        ///	{
        ///	
        ///		...
        ///		context.LogData( "HTTP Response:", data );
        ///	</code>
        ///	
        ///	</remarks>
        public void LogData(string description, Stream data)
        {
            LogData(description, data, false);
        }

        /// <summary>
        /// Used by a test step to log test Data, this will be logged in the test output.
        /// </summary>
        /// <param name="description">The description of what the data being logged is.</param>
        /// <param name="data">The stream containing the data to log.</param>
        /// <param name="seekStream">Seek the stream back to the beginning.</param>
        /// 
        /// <remarks>
        /// The following example demonstrates how to use the method:
        /// 
        /// <code escaped="true">
        ///	public void Execute(XmlNode testConfig, Context context)
        ///	{
        ///	
        ///		...
        ///		context.LogData( "HTTP Response:", data );
        ///	</code>
        ///	
        ///	</remarks>
        public void LogData(string description, Stream data, bool seekStream)
        {
            ArgumentValidation.CheckForNullReference(description, "description");
            ArgumentValidation.CheckForNullReference(data, "data");

            var sr = new StreamReader(data);
            LogData(description, sr.ReadToEnd());

            if(seekStream)
            {
                data.Seek(0, SeekOrigin.Begin);
            }
        }

        /// <summary>
        /// Used by a test step to log Xml test Data, this will be logged in the test output.
        /// </summary>
        /// <param name="description">The description of what the data being logged is.</param>
        /// <param name="data">The data to log.</param>
        /// 
        /// <remarks>
        /// The following example demonstrates how to use the method:
        /// 
        /// <code escaped="true">
        ///	public void Execute(XmlNode testConfig, Context context)
        ///	{
        ///	
        ///		...
        ///		context.LogXmlData( "HTTP Response:", data );
        ///	</code>
        ///	
        ///	</remarks>
        public void LogXmlData(string description, string data)
        {
            ArgumentValidation.CheckForNullReference(description, "description");
            ArgumentValidation.CheckForNullReference(data, "data");

            _logger.LogXmlData(description, data);
        }

        /// <summary>
        /// Used by a test step to log Xml test Data, this will be logged in the test output.
        /// </summary>
        /// <param name="description">The description of what the data being logged is.</param>
        /// <param name="data">The stream containing the data to log.</param>
        /// 
        /// <remarks>
        /// The following example demonstrates how to use the method:
        /// 
        /// <code escaped="true">
        ///	public void Execute(XmlNode testConfig, Context context)
        ///	{
        ///	
        ///		...
        ///		context.LogXmlData( "HTTP Response:", data );
        ///	</code>
        ///	
        ///	</remarks>
        public void LogXmlData(string description, Stream data)
        {
            LogXmlData(description, data, false);
        }

        /// <summary>
        /// Used by a test step to log Xml test Data, this will be logged in the test output.
        /// </summary>
        /// <param name="description">The description of what the data being logged is.</param>
        /// <param name="data">The stream containing the data to log.</param>
        /// <param name="seekStream">Seek the stream back to the beginning.</param>
        /// 
        /// <remarks>
        /// The following example demonstrates how to use the method:
        /// 
        /// <code escaped="true">
        ///	public void Execute(XmlNode testConfig, Context context)
        ///	{
        ///	
        ///		...
        ///		context.LogData( "HTTP Response:", data );
        ///	</code>
        ///	
        ///	</remarks>
        public void LogXmlData(string description, Stream data, bool seekStream)
        {
            ArgumentValidation.CheckForNullReference(description, "description");
            ArgumentValidation.CheckForNullReference(data, "data");

            var sr = new StreamReader(data);
            LogXmlData(description, sr.ReadToEnd());

            if (seekStream)
            {
                data.Seek(0, SeekOrigin.Begin);
            }
        }

        /// <summary>
        /// Used by a test step to log test information, this will be logged in the test output.
        /// </summary>
        /// <param name="text">The text to be written to the output.</param>
        /// 
        /// <remarks>
        /// The following example demonstrates how to use the method:
        /// 
        /// <code escaped="true">
        ///	public void Execute(XmlNode testConfig, Context context)
        ///	{
        ///	
        ///		...
        ///		context.LogInfo( "HTTP Response was successfully received" );
        ///	</code>
        ///	
        ///	</remarks>
        public void LogInfo(string text)
        {
            ArgumentValidation.CheckForNullReference(text, "text");

            _logger.Log(LogLevel.INFO, text);
        }

        /// <summary>
        /// Used by a test step to log test information, this will be logged in the test output.
        /// </summary>
        /// <param name="text">The text to be written to the output.</param>
        /// <param name="args">Array of arguments to be formatted with the text.</param>
        /// 
        /// <remarks> 
        /// The following example demonstrates how to use the method:
        /// 
        /// <code escaped="true">
        ///	public void Execute(XmlNode testConfig, Context context)
        ///	{
        ///	
        ///		...
        ///		context.LogInfo( "HTTP Response was successfully received by: {0}, at: {1}", clientName, DateTime.Now );
        ///	</code>
        ///	
        ///	</remarks>
        public void LogInfo(string text, params object[] args)
        {
            ArgumentValidation.CheckForNullReference(text, "text");
            ArgumentValidation.CheckForNullReference(args, "args");

            _logger.Log(LogLevel.INFO, string.Format(text, args));
        }

        /// <summary>
        /// Used by a test step to log a test warnings, this will be logged in the test output.
        /// </summary>
        /// <param name="text">The text to be written to the output.</param>
        /// 
        /// <remarks> 
        /// The following example demonstrates how to use the method:
        /// 
        /// <code escaped="true">
        ///	public void Execute(XmlNode testConfig, Context context)
        ///	{
        ///	
        ///		...
        ///		context.LogWarning( "The FILE was found, retrying..." );
        ///	</code>
        ///	
        ///	</remarks>
        public void LogWarning(string text)
        {
            ArgumentValidation.CheckForNullReference(text, "text");

            _logger.Log(LogLevel.WARNING, text);
        }

        /// <summary>
        /// Used by a test step to log a test warnings, this will be logged in the test output.
        /// </summary>
        /// <param name="text">The text to be written to the output.</param>
        /// <param name="args">Array of arguments to be formatted with the text.</param>
        /// 
        /// <remarks> 
        /// The following example demonstrates how to use the method:
        /// 
        /// <code escaped="true">
        ///	public void Execute(XmlNode testConfig, Context context)
        ///	{
        ///	
        ///		...
        ///		context.LogWarning( "The FILE was found, poll nummber: {0}, number of retries remaining: {1}", count, retriesLeft );
        ///	</code>
        ///	
        ///	</remarks>
        public void LogWarning(string text, params object[] args)
        {
            ArgumentValidation.CheckForNullReference(text, "text");
            ArgumentValidation.CheckForNullReference(args, "args");

            _logger.Log(LogLevel.WARNING, string.Format(text, args));
        }

        /// <summary>
        /// Used by a test step to log a test error, this will be logged in the test output.
        /// </summary>
        /// <param name="text">The text to be written to the output.</param>
        /// 
        /// <remarks> 
        /// The following example demonstrates how to use the method:
        /// 
        /// <code escaped="true">
        ///	public void Execute(XmlNode testConfig, Context context)
        ///	{
        ///	
        ///		...
        ///		context.Log(LogLevel.ERROR,  "The response data was invalid." );
        ///	</code>
        ///	
        ///	</remarks>
        public void LogError(string text)
        {
            ArgumentValidation.CheckForNullReference(text, "text");

            _logger.Log(LogLevel.ERROR, text);
        }

        /// <summary>
        /// Used by a test step to log a test error, this will be logged in the test output.
        /// </summary>
        /// <param name="text">The text to be written to the output.</param>
        /// <param name="args">Array of arguments to be formatted with the text.</param>
        /// 
        /// <remarks> 
        /// The following example demonstrates how to use the method:
        /// 
        /// <code escaped="true">
        ///	public void Execute(XmlNode testConfig, Context context)
        ///	{
        ///	
        ///		...
        ///		context.Log(LogLevel.ERROR,  "The request failed with the folowing error: {0}", requestErrorText );
        ///	</code>
        ///	
        ///	</remarks>
        public void LogError(string text, params object[] args)
        {
            ArgumentValidation.CheckForNullReference(text, "text");
            ArgumentValidation.CheckForNullReference(args, "args");

            _logger.Log(LogLevel.ERROR, text, args);
        }

        /// <summary>
        /// Sets a value indicating whether members, which implement <see cref="IDispoable"/>, are disposed on test case completion.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if members should be disposed; otherwise, <c>false</c> (default).
        /// </value>
        public bool DisposeMembersOnTestCaseCompletion
        {
            set
            {
                _disposeMembersOnTestCaseCompletion = value;
            }
        }

        /// <summary>
        /// Used by a test step to get the time that the test case started.
        /// </summary>
        /// 
        /// <remarks> 
        /// The following example demonstrates how to use the method:
        /// 
        /// <code escaped="true">
        ///	public void Execute(XmlNode testConfig, Context context)
        ///	{
        ///	
        ///		...
        ///		DateTime testStart = context.TestCaseStart;
        ///	</code>
        ///	
        ///	</remarks>
        public DateTime TestCaseStart
        {
            get
            {
                return _startTime;
            }
        }

        /// <summary>
        /// Used to substitute wild cards into strings.
        /// </summary>
        /// 
        /// <remarks> 
        /// The following wild cards are suported:
        /// 
        /// <code escaped="true">
        /// %DateTime% - will replace the wild card with the current date time in the format HHmmss-ddMMyyyy
        /// %ServerName% - will replace the wild card with the name of the server BizUnit is being executed on
        /// %Guid% - will be replaced by a new Guid
        ///	</code>
        ///	
        ///	</remarks>
        public string SubstituteWildCards(string rawString)
        {
            ArgumentValidation.CheckForNullReference(rawString, "rawString");

            string result = rawString;

            if (result.Contains(DateTime))
                result = result.Replace(DateTime, System.DateTime.Now.ToString("HHmmss-ddMMyyyy"));

            if (result.Contains(DateTimeIso8601))
                result = result.Replace(DateTimeIso8601, System.DateTime.Now.ToString("s"));

            if (result.Contains(ServerName))
                result = result.Replace(ServerName, Environment.MachineName);

            if (result.Contains(Guid))
                result = result.Replace(Guid, System.Guid.NewGuid().ToString());

            if (result.Contains(TestStartDateTime))
            {
                var testStartTime = (DateTime)_context[BizUnit.BizUnitTestCaseStartTime];
                result = result.Replace(TestStartDateTime, testStartTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            }

            return result;
        }
    
        private void DisposeDisposableMembers()
        {
            foreach (DictionaryEntry dictionaryEntry in _context)
            {
                var disposableObject = dictionaryEntry.Value as IDisposable;

                if (disposableObject != null)
                    disposableObject.Dispose();
            }
        }
    }
}
