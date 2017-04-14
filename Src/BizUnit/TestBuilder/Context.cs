//---------------------------------------------------------------------
// File: Context.cs
// 
// Summary: 
//
//---------------------------------------------------------------------
// Copyright (c) 2004-2017, Kevin B. Smith. All rights reserved.
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
using BizUnit.Common;
using BizUnit.Core;

namespace BizUnit.TestBuilder
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
        private TestRunner _bizUnit;
        private readonly ILogger _logger;
        private readonly DateTime _startTime;

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

        internal Context(TestRunner bizUnit)
        {
            _bizUnit = bizUnit;
            _context = new Hashtable();
            _logger = new Logger();
            _startTime = System.DateTime.Now;
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Context.Context(ILogger)'
        public Context(ILogger logger)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Context.Context(ILogger)'
        {
            _logger = logger;
            _context = new Hashtable();
            _startTime = System.DateTime.Now;
        }

        internal void SetTestStage(TestStage currentStage)
        {
            CurrentTestStage = currentStage;
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Context.CurrentTestStage'
        public TestStage CurrentTestStage { get; private set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Context.CurrentTestStage'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Context.TestName'
        public string TestName { get; private set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Context.TestName'

        internal void SetTestName(string currentTestName)
        {
            TestName = currentTestName;
        }

        internal Context(TestRunner bizUnit, ILogger logger)
        {
            _bizUnit = bizUnit;
            _logger = logger;
            _context = new Hashtable();
            _startTime = System.DateTime.Now;
        }

        private Context(TestRunner bizUnit, Hashtable ctx, ILogger logger, DateTime t)
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

        internal TestRunner BizUnitObject
        {
            get
            {
                return _bizUnit;
            }
        }

        internal void Initialize(TestRunner bizunit)
        {
            ArgumentValidation.CheckForNullReference(bizunit, "bizunit");
            _bizUnit = bizunit;
        }

        internal Context CloneForConcurrentUse(ILogger logger)
        {
            return new Context(_bizUnit, _context, logger, _startTime);
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
        

        private string ReadConfigAsString(XmlNode config, string xPath, bool optional, bool asXml)
        {
            return (string)ReadConfigAsObject(config, xPath, optional, asXml);
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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Context.ReadArgument(object)'
        public object ReadArgument(object arg)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Context.ReadArgument(object)'
        {
            ArgumentValidation.CheckForNullReference(arg, "arg");

            if (arg is System.String)
            {
                string strArg = (string) arg;
                if(strArg.Contains(ContextConstaints.TakeFromContext))
                {
                    string key = strArg.Substring(ContextConstaints.TakeFromContext.Length);
                    return _context[key];
                }

                return arg;
            }

            return arg;                
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

            if (result.Contains(ContextConstaints.DateTime))
                result = result.Replace(ContextConstaints.DateTime, System.DateTime.Now.ToString("HHmmss-ddMMyyyy"));

            if (result.Contains(ContextConstaints.DateTimeIso8601))
                result = result.Replace(ContextConstaints.DateTimeIso8601, System.DateTime.Now.ToString("s"));

            if (result.Contains(ContextConstaints.ServerName))
                result = result.Replace(ContextConstaints.ServerName, Environment.MachineName);

            if (result.Contains(ContextConstaints.Guid))
                result = result.Replace(ContextConstaints.Guid, System.Guid.NewGuid().ToString());

            if (result.Contains(ContextConstaints.TestStartDateTime))
            {
                var testStartTime = (DateTime)_context[TestRunner.BizUnitTestCaseStartTime];
                result = result.Replace(ContextConstaints.TestStartDateTime, testStartTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            }

            return result;
        }
    
        internal void DisposeDisposableMembers()
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
