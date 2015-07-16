//---------------------------------------------------------------------
// File: DotNetObjectInvokerStep.cs
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
using System.Reflection;
using System.Xml.Serialization;
using System.Collections;
using BizUnit.CoreSteps.Utilities;

namespace BizUnit.CoreSteps.TestSteps
{
	/// <summary>
	/// The DotNetObjectInvokerStep is used to invoke a method on a .Net component with the ability to handle 
    /// input and output parameters.
	/// </summary>
	/// 
	/// <remarks>
	/// The following shows an example of the Xml representation of this test step.
	/// 
	/// <code escaped="true">
	///	<TestStep assemblyPath="" typeName="BizUnit.DotNetObjectInvokerStep">
	///		<TypeName>Microsoft.Samples.BizTalk.WoodgroveBank.ServiceLevelTracking.ServiceLevelTracking, Microsoft.Samples.BizTalk.WoodgroveBank.ServiceLevelTracking, Version=1.0.0.0, Culture=neutral, PublicKeyToken=a1054514fc67bded</TypeName>	
	///		<AssemblyPath></AssemblyPath>
	///		<MethodToInvoke>TestEndRequest3</MethodToInvoke>
    ///		<Parameter><string>fooBar</string></Parameter>
    ///		<Parameter><int>123</int></Parameter>
	///		<ReturnParameter><int>barfoo</int></ReturnParameter>
	///	</TestStep>
	///	</code>
	///	
	///	<list type="table">
	///		<listheader>
	///			<term>Tag</term>
	///			<description>Description</description>
	///		</listheader>
	///		<item>
	///			<term>TypeName</term>
	///			<description>The name of the type of the .Net object to invoke the method on. Note, if the type is GAC'd then the assembly name, version, public key etc need to be specified</description>
	///		</item>
	///		<item>
	///			<term>AssemblyPath</term>
	///			<description>The path to the assembly <para>(optional)</para></description>
	///		</item>
	///		<item>
	///			<term>MethodToInvoke</term>
	///			<description>The name of the method to invoke</description>
	///		</item>
	///		<item>
    ///			<term>Parameter</term>
	///			<description>The value for the parameter to pass into the method. Note: the format should be the serialised .Net type, 
	///         if the value is taken form the context, the value in the context should also be a serialized .Net type.<para>(optional | multiple)</para></description>
	///		</item>
	///		<item>
	///			<term>ReturnParameter</term>
	///			<description>The value returned from the method <para>(optional)</para></description>
	///		</item>
	///	</list>
	///	</remarks>
    [Obsolete("DotNetObjectInvokerStep has been deprecated. Investigate the BizUnit.TestSteps namespace.")]
	public class DotNetObjectInvokerStep : ITestStep
	{
		/// <summary>
		/// ITestStep.Execute() implementation
		/// </summary>
		/// <param name='testConfig'>The Xml fragment containing the configuration for this test step</param>
		/// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
		public void Execute(XmlNode testConfig, Context context)
		{
			// Rread test config...
			var typeName = context.ReadConfigAsString( testConfig, "TypeName" );
			var assemblyPath = context.ReadConfigAsString( testConfig, "AssemblyPath" );
			var methodToInvoke = context.ReadConfigAsString( testConfig, "MethodToInvoke" );
            var parameters = testConfig.SelectNodes("Parameter");
			var returnParameter = context.ReadConfigAsXml( testConfig, "ReturnParameter", true );
			
			var obj = CreateObject(typeName, assemblyPath, context);

			var mi = obj.GetType().GetMethod(methodToInvoke);
			var pi = mi.GetParameters();
			
			var parameterArray = new ArrayList();

			for( int c = 0; c < pi.Length; c++)
			{
				var t = pi[c].ParameterType;
				var xs = new XmlSerializer(t);
                parameterArray.Add(xs.Deserialize(new XmlTextReader(StreamHelper.LoadMemoryStream(context.GetInnerXml(parameters[c])))));
			}

			var paramsForCall = new object[parameterArray.Count];
			for( int c = 0; c < parameterArray.Count; c++ )
			{
				paramsForCall[c] = parameterArray[c];
			}

			context.LogInfo("About to call the method: {0}() on the type {1}", methodToInvoke, typeName );
			// Call the .Net Object...
			var returnValue = mi.Invoke(obj, paramsForCall);
			context.LogInfo("Return value: {0}", returnValue);

            if (!string.IsNullOrEmpty(returnParameter))
			{
				var xsRet = new XmlSerializer(returnValue.GetType());

				var rs = new MemoryStream();
				xsRet.Serialize( new StreamWriter(rs), returnValue );
				var es = StreamHelper.LoadMemoryStream(returnParameter);

				rs.Seek(0, SeekOrigin.Begin);
				es.Seek(0, SeekOrigin.Begin);
				StreamHelper.CompareXmlDocs(rs, es, context);
			}
		}

		static private object CreateObject(string typeName, string assemblyPath, Context context)
		{
			object comp = null;
			Type ty;

			context.LogInfo("About to create the folowing .Net type: {0}", typeName);

			if (!string.IsNullOrEmpty(assemblyPath)) 
			{
				context.LogInfo("Loading assembly form path: {0}", assemblyPath);

				var assembly = Assembly.LoadFrom(assemblyPath);
				ty = assembly.GetType(typeName, true, false);
			}
			else 
			{
				ty = Type.GetType(typeName);
			}

			if (ty != null) 
			{
				comp = Activator.CreateInstance(ty);
			}
			
			return comp;
		}
	}
}
