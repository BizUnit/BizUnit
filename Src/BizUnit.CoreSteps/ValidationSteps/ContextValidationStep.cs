//---------------------------------------------------------------------
// File: ContextValidationStep.cs
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

namespace BizUnit.CoreSteps.ValidationSteps
{
	using System;
	using System.IO;
	using System.Xml;

	/// <summary>
	/// The ContextValidationStep validates the value of one or more context values.
	/// </summary>
	/// 
	/// <remarks>
	/// The following shows an example of the Xml representation of this test step.
	/// 
	/// <code escaped="true">
	///	<ValidationStep assemblyPath="" typeName="BizUnit.ContextValidationStep">
	///		<Context keyName="SchemeID">GF81300000</Context>
	///	</ValidationStep>
	///	</code>
	///	
	///	<list type="table">
	///		<listheader>
	///			<term>Tag</term>
	///			<description>Description</description>
	///		</listheader>
	///		<item>
	///			<term>Context</term>
	///			<description>The attribute keyName identifies the context key to validate, while the element represents the expected result <para>(optional)(repeating)</para></description>
	///		</item>
	///	</list>
	///	</remarks>	
    [Obsolete("ContextValidationStep has been deprecated. Investigate the BizUnit.TestSteps namespace.")]
    public class ContextValidationStep : IValidationStep
	{
		/// <summary>
		/// IValidationStep.ExecuteValidation() implementation
		/// </summary>
		/// <param name='data'>The stream cintaining the data to be validated.</param>
		/// <param name='validatorConfig'>The Xml fragment containing the configuration for the test step</param>
		/// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
		public void ExecuteValidation(Stream data, XmlNode validatorConfig, Context context)
		{
			XmlNodeList validationNodes = validatorConfig.SelectNodes("Context");

			foreach (XmlNode validationNode in validationNodes)
			{
				string keyName = validationNode.SelectSingleNode("@keyName").Value;
				string expectedValue = validationNode.InnerText;

				string actualValue = context.GetValue(keyName);

				if ( (!string.IsNullOrEmpty(actualValue)) && (0 == expectedValue.CompareTo(actualValue)) )
				{
					context.LogInfo("Context validation succeeded, key: \"{0}\" equals: \"{1}\".", keyName, expectedValue);
				}
				else
				{
					throw new Exception(String.Format("Context validation failed for key: {0}. Expected value: {1}, actual value: {2}", keyName, expectedValue, actualValue));
				}
			}
		}
	}
}
