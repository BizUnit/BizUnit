//---------------------------------------------------------------------
// File: CodeValidationStep.cs
// 
// Summary: 
//
// Author: Jon Fancey (http://www.jonfancey.com)
//
//---------------------------------------------------------------------
// Copyright (c) 2004-2015, Kevin B. Smith. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------

namespace BizUnit.CoreSteps.ValidationSteps
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Reflection;

    /// <summary>
	/// The CodeValidationStep validates expected results from the execution of a method, property or field.
	/// </summary>
	/// 
	/// <remarks>
	/// The following shows an example of the Xml representation of this test step.
	/// 
	/// <code escaped="true">
	///	<ValidationStep assemblyPath="" typeName="BizUnit.CodeValidationStep">
	///		<TypeName>SampleCodeValidator.CheckResults</TypeName>
	///		<AssemblyPath>SampleCodeValidator.dll</AssemblyPath>
	///		<Constructor>
	///			<Parameters>
	///				<Parameter value="test"/>
	///			</Parameters>
	///		</Constructor>
	///		<Member name="GetValue">
	///			<!-- input parms to method -->
	///			<Parameters>
	///				<Parameter name="A" value="1"/>
	///				<Parameter name="B" value="2"/>
	///				<Parameter name="C" value="test"/>
	///			</Parameters>
	///			<!-- output parms from method -->
	///			<Results>
	///				<Result value="foo"/> <!-- return value of member -->
	///				<Result name ="A" value="bar"/>
	///			</Results>
	///		</Member>
	///	</ValidationStep>
	///	</code>
	///	
	///	<list type="table">
	///		<listheader>
	///			<term>Tag</term>
	///			<description>Description</description>
	///		</listheader>
	///		<item>
	///			<term>TypeName</term>
	///			<description>Fully qualified name of the type to create.</description>
	///		</item>
	///		<item>
	///			<term>AssemblyPath</term>
	///			<description>Location of the assembly on disk containing the required type.</description>
	///		</item>
	///		<item>
	///			<term>Constructor</term>
	///			<description>Contains the parameters for the constructor to call for the desired type.</description>
	///		</item>
	///		<item>
	///			<term>Member</term>
	///			<description>The name of the member (field/property/method) to invoke.</description>
	///		</item>
	///		<item>
	///			<term>Parameter</term>
	///			<description>The required parameters for the member to invoke. Note that for overloaded members this step disambiguates by number of parameters only.</description>
	///			and resulting values to test.
	///		</item>
	///		<item>
	///			<term>Result</term>
	///			<description>The result - either return value or ref/out parameter to test. To indicate that the result is the return value the name attribute should not be specified.</description>
	///		</item>
	///	</list>
	///	</remarks>
    [Obsolete("CodeValidationStep has been deprecated. Investigate the BizUnit.TestSteps namespace.")]
    public class CodeValidationStep : IValidationStep
	{
		/// <summary>
		/// used to check that the execution of a class-based fact has been successful
		/// </summary>
		/// <param name="data"></param>
		/// <param name="validatorConfig"></param>
		/// <param name="context"></param>
        public void ExecuteValidation(Stream data, XmlNode validatorConfig, Context context)
        {
			// get the type name, create an instance of it
		    string assemblyPath = null;
			Type type;
			var assemblyNode = validatorConfig.SelectSingleNode("AssemblyPath");

			if (assemblyNode !=  null)
			{
				assemblyPath = assemblyNode.InnerText;
			}

			object fact = context.ReadConfigAsObject(validatorConfig.SelectSingleNode("TypeName"), ".",true);
			if (fact as string == null)
			{
				type = fact.GetType();
			}
			else
			{
				var typeName = (string)fact;
				if (typeName.Length == 0)
				{
					// fail
					throw (new ApplicationException("no type specified"));
				}
			
				// get constructor details
				XmlNodeList constructorParms = validatorConfig.SelectNodes("Constructor/Parameters/Parameter");

				if (!string.IsNullOrEmpty(assemblyPath))
				{
                    Assembly asm = Assembly.LoadFrom(assemblyPath);
					if (asm == null)
					{
						// fail
						throw (new Exception("failed to create type " + typeName));
					}
					type = asm.GetType(typeName, true, false);
				}
				else
				{
					// must be in path
					type = Type.GetType(typeName);
				}
			
				// set up constructor params
				var construct = new object[constructorParms.Count];

				var constructorDetails = type.GetConstructors();
				
				// match constructor on number of parms
				foreach (ConstructorInfo info in constructorDetails)
				{
					int i = 0;
					if (info.GetParameters().Length == constructorParms.Count)
					{
						foreach (ParameterInfo pi in info.GetParameters())
						{
							construct[i] = Convert.ChangeType(context.ReadConfigAsString(constructorParms[i], "."),pi.ParameterType);
							i++;
						}
						break;
					}
					if (i == constructorParms.Count)
					{
						// no match on constructor parms
						throw(new Exception(String.Format("No suitable constructor found for type {0}. Looking for constructor with {1} parameters. Cannot create instance",type.Name,constructorParms.Count)));
					}
				}
				
				// call the constructor 
				fact = Activator.CreateInstance(type, BindingFlags.Default, Type.DefaultBinder, construct, null);
			}
		
			// process the parameters
			XmlNode method = validatorConfig.SelectSingleNode("Member");
			string methodName  = method.Attributes.GetNamedItem("name").Value;
			var parms = validatorConfig.SelectSingleNode("Member/Parameters");
			var results = validatorConfig.SelectNodes("Member/Results/Result");

			// call the method & compare the return value with that expected
			var args = new object[parms.SelectNodes("Parameter").Count];

			// check if void return
			var mi = type.GetMember(methodName);
			if (mi.Length == 0) // member not found
			{
				throw (new ApplicationException(String.Format("Requested member {0} not found in type {1} in instance {2}", methodName, type.Name, fact)));
			}

			MethodInfo methodDetails = null;
			int infoCount = 0;

			if (mi.Length > 1)
			{
				// match correct member with our supplied parameters
				for(; infoCount < mi.Length; infoCount++)
				{
					methodDetails = mi[infoCount] as MethodInfo;
					if (methodDetails == null)
					{
						// can only support parameters for method type
						throw(new Exception(String.Format("Ambiguous member name. Found {0} matches", mi.Length)));
					}

                    if (methodDetails.GetParameters().Length == parms.SelectNodes("Parameter").Count)
					{
						// found a match on the correct number of parameters
						break;
					}
				}
				if (infoCount == mi.Length)
				{
					throw(new Exception(String.Format("Ambiguous member name. Found {0} matches", mi.Length)));
				}
			}

			switch(mi[infoCount].MemberType)
			{
				case MemberTypes.Method:
				{
					methodDetails = (MethodInfo)mi[infoCount];
					int x = 0;
					foreach (ParameterInfo pi in methodDetails.GetParameters())
					{
						string val = null;
						try
						{
							XmlNode parm = parms.SelectSingleNode("Parameter[@name='" + pi.Name + "']");
							if (parm == null)
							{
								// parameter name not defined in configuration
								throw(new Exception(String.Format("The parameter name {0} for method {1} was not defined in the step configuration",pi.Name,methodDetails.Name)));
							}
							else
							{
								val = context.ReadConfigAsString(parm, ".");
								args[x++] = Convert.ChangeType(val,pi.ParameterType);
							}
						}
						catch (FormatException e)
						{
							Console.WriteLine(String.Format("The method parameter {0} with value {1} could not be cast to {2}",pi.Name,val,pi.ParameterType.Name));
							throw;
						}
					}
					break;
				}
			}

			object result = type.InvokeMember(methodName,BindingFlags.InvokeMethod | BindingFlags.GetProperty,Type.DefaultBinder,fact,args);

			// evaluate expected results			
			for (int i = 0; i < results.Count; i++)
			{
				string resultName = String.Empty;
				string resultValue;

				if (results[i].Attributes.GetNamedItem("name") != null)
				{
					resultName = results[i].Attributes.GetNamedItem("name").Value;
					// find index of parameter in the arguments to retrieve by name
					if (methodDetails != null)
					{
						int p = 0;
						ParameterInfo[] pi = methodDetails.GetParameters();
						for(;p < methodDetails.GetParameters().Length;p++)
						{
							if (pi[p].Name == resultName)
							{
								// compare
								resultValue = results[i].Attributes.GetNamedItem("value").Value;
								context.LogInfo("CodeValidationStep evaluating result {0} equals \"{1}\"", resultName, resultValue, args[i].ToString());					
								if (resultValue != args[p].ToString())
								{
									throw new ApplicationException(string.Format( "CodeValidationStep failed, compare {0} != {1}, parameter name used: {2}", resultValue, args[i], resultName));
								}
								break;
							}
						}
						if (p == methodDetails.GetParameters().Length)
						{
							// not found
							throw new ApplicationException(string.Format("Parameter {0} not found in member {1}",resultName, methodDetails.Name));
						}
					}
					else 
					{
						throw new ApplicationException("Configuration not supported");
					}
				}
				else
				{
					// don't have a named parm value for the return value
					if (result == null)
					{
						throw new ApplicationException("CodeValidationStep failed, expected return value from member but found null");
					}

                    resultValue = results[i].Attributes.GetNamedItem("value").Value;
					context.LogInfo("CodeValidationStep evaluating return value {0} equals \"{1}\"", resultValue, result);
					if (resultValue.CompareTo(result) != 0)
					{
						throw new ApplicationException(string.Format( "CodeValidationStep failed, compare {0} != {1}, parameter name used: {2}", resultValue, result, resultName));
					}
				}
			}
        }
    }
}
