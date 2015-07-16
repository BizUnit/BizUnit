//---------------------------------------------------------------------
// File: TestStepBuilderBase.cs
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

using BizUnit.Common;
using System;
using System.Xml;
using System.Web.UI;
using System.Reflection;
using System.Collections.Generic;

namespace BizUnit.BizUnitOM
{
    /// <summary>
    /// The TestStepBuilderBase is the base class for TestStepBuilder, 
    /// ContextLoaderStepBuilder and ValidationStepBuilder. It should 
    /// not be used directly. TestStepBuilderBase handles the generic 
    /// setting of step properties.
    /// </summary>
    [Obsolete("TestStepBuilderBase has been deprecated. Please investigate the use of BizUnit.Xaml.TestCase.")]
    public abstract class TestStepBuilderBase
    {
        protected XmlNode stepXmlConfig;
        protected IList<Pair> propsToSet = new List<Pair>();
        protected IList<Pair> propsToTakeFromCtx = new List<Pair>();
        private DefaultTestStepParameterFormatter formatter = new DefaultTestStepParameterFormatter();

        internal TestStepBuilderBase() {}

        /// <summary>
        /// TestStepBuilderBase constructor.
        /// </summary>
        /// 
        /// <param name='config'>The Xml configuration for a test step that 
        /// implements the ITestStep interface.</param>
        public TestStepBuilderBase(XmlNode config)
        {
            ArgumentValidation.CheckForNullReference(config, "config");

            XmlNode assemblyPathNode = config.SelectSingleNode("@assemblyPath");
            XmlNode typeNameNode = config.SelectSingleNode("@typeName");

            RawTestStep = ObjectCreator.CreateStep(typeNameNode.Value, assemblyPathNode.Value);
            stepXmlConfig = config;
        }

        /// <summary>
        /// TestStepBuilderBase constructor.
        /// </summary>
        /// 
        /// <param name='typeName'>The type name of the test step to be created by the builder.</param>
        public TestStepBuilderBase(string typeName)
        {
            ArgumentValidation.CheckForEmptyString(typeName, "typeName");

            RawTestStep = ObjectCreator.CreateStep(typeName, null);
            if (null == RawTestStep)
            {
                throw new ArgumentException(string.Format("The test step could not be created, check the test step type and assembly path are correct, type: {0}", typeName));
            }
        }

        /// <summary>
        /// TestStepBuilderBase constructor.
        /// </summary>
        /// 
        /// <param name='typeName'>The type name of the test step to be created by the builder.</param>
        /// <param name='assemblyPath'>The assembly path name of the test step to be created by the builder.</param>
        public TestStepBuilderBase(string typeName, string assemblyPath)
        {
            ArgumentValidation.CheckForEmptyString(typeName, "typeName");
            // assemblyPath - optional

            RawTestStep = ObjectCreator.CreateStep(typeName, assemblyPath);
            if (null == RawTestStep)
            {
                throw new ArgumentException(string.Format("The test step could not be created, check the test step type and assembly path are correct, type: {0}, assembly path: {1}", typeName, assemblyPath));
            }
        }

        /// <summary>
        /// RawTestStep returns the test step, which could be either a config driven step or an OM driven step.
        /// </summary>
        public object RawTestStep { get; protected set; }

        /// <summary>
        /// SetProperty is used to set a property on a test step.
        /// </summary>
        /// 
        /// <param name='name'>The name of the property to set on the test step or test sub-step.</param>
        /// <param name='args'>An object array that will be formatted to the correct type of the property which is being set.</param>
        public void SetProperty(string name, object[] args)
        {
            if (null == RawTestStep)
            {
                throw new InvalidOperationException("The test step does not implement ITestStepOM");
            }

            object[] clonedArgs = new object[args.Length];
            for (int c = 0; c < args.Length; c++ )
            {
                clonedArgs[c] = args[c];
            }

            if (!ProcessTakefromContext(name, args))
            {
                propsToSet.Add(new Pair(name, clonedArgs));
            }
        }

        private void SetPropertyOnStep(string name, object[] args, Context ctx)
        {
            PropertyInfo[] propertiesInfo = RawTestStep.GetType().GetProperties();
            bool found = false;

            foreach (PropertyInfo propertyInfo in propertiesInfo)
            {
                if (name == propertyInfo.Name)
                {

                    BizUnitParameterFormatterAttribute[] formatterAttributes =
                                    (BizUnitParameterFormatterAttribute[])propertyInfo.GetCustomAttributes(
                                                                   typeof(BizUnitParameterFormatterAttribute),
                                                                   false);

                    TestStepParameterFormatter paramterFormatter;

                    if (formatterAttributes != null && formatterAttributes.Length > 0)
                    {
                        object obj =
                            ObjectCreator.CreateStep(formatterAttributes[0].TypeName,
                                                     formatterAttributes[0].AssemblyPath);

                        if (null == obj)
                        {
                            throw new ApplicationException(string.Format("The propery {0} has specified a custom BizUnit parameter formatter specified, but the formatter cannot be created, typeName: {1}, assemblyPath: {2}", propertyInfo.Name, formatterAttributes[0].TypeName, formatterAttributes[0].AssemblyPath));
                        }

                        ITestStepParameterFormatter customFormatter = obj as ITestStepParameterFormatter;

                        if (null == customFormatter)
                        {
                            throw new ApplicationException(string.Format("The propery {0} has specified a custom BizUnit parameter formatter, but the formatter does not implement ITestStepParameterFormatter", propertyInfo.Name));
                        }
                        paramterFormatter = customFormatter.FormatParameters;
                    }
                    else
                    {
                        paramterFormatter = formatter.FormatParameters;
                    }
                    
                    object[] propertyArgs = paramterFormatter(propertyInfo.PropertyType, args, ctx);
                    propertyInfo.GetSetMethod().Invoke(RawTestStep, propertyArgs);
                    found = true;
                    break;
                }
            }

            if(!found)
            {
                throw new ApplicationException(string.Format("BizUnitOM could not find the property {0} on the test step", name));
            }
        }

        private bool ProcessTakefromContext(string name, object[] args)
        {
            if(null != args && 1 == args.Length)
            {
                string str = args[0] as System.String;
                if(null != str)
                {
                    if(str.StartsWith(Context.TakeFromContext))
                    {
                        string ctxPropName = str.Substring(Context.TakeFromContext.Length);
                        propsToTakeFromCtx.Add(new Pair(name, ctxPropName));
                        return true;
                    }
                }
            }

            return false;
        }

        internal void PrepareStepForExecution(Context ctx)
        {
            foreach (Pair prop in propsToSet)
            {
                string name = (string)prop.First;
                object[] args = (object[])prop.Second;

                SetPropertyOnStep(name, args, ctx);
            }

            foreach (Pair prop in propsToTakeFromCtx)
            {
                string name = (string)prop.First;
                string ctxPropName = (string)prop.Second;

                object[] args = new object[1];
                args[0] = ctx.GetObject(ctxPropName);
                SetPropertyOnStep(name, args, ctx);
            }
        }

        /// <summary>
        /// SetConfigXml is used to set the Xml configuration for a test step which will 
        /// be used during the execution of a test step.
        /// </summary>
        /// 
        /// <param name='xmlConfig'>The Xml configuration for the test step.</param>
        public void SetConfigXml(string xmlConfig)
        {
            ArgumentValidation.CheckForEmptyString(xmlConfig, "xmlConfig");

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlConfig);
            stepXmlConfig = doc.DocumentElement;
        }

        /// <summary>
        /// GetPropertyInfo returns the <see cref="System.Reflection.PropertyInfo"/> of the property specified by name.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>PropertyInfo for the property with the name specified.</returns>
        public PropertyInfo GetPropertyInfo(string propertyName)
        {
            PropertyInfo[] propertiesInfo = RawTestStep.GetType().GetProperties();
            foreach (PropertyInfo propertyInfo in propertiesInfo)
            {
                if (propertyName == propertyInfo.Name)
                {
                    return propertyInfo;
                }
            }

            return null;
        }
    }
}
