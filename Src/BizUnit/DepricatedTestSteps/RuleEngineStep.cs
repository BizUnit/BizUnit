//---------------------------------------------------------------------
// File: RuleEngineStep.cs
// 
// Summary: 
//
//---------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------
namespace BizUnit
{
	using System;
	using System.IO;
	using System.Xml;
	using Microsoft.RuleEngine;
	using Microsoft.BizTalk.RuleEngineExtensions;

	/// <summary>
	/// Summary description for RuleEngineStep.
	/// </summary>
	/// 
	/// <remarks>
	/// The following shows an example of the Xml representation of this test step.
	///
	/// <code escaped="true">
	///	<TestStep assemblyPath="" typeName="BizUnit.RuleEngineStep">
	///		<RuleStoreName>C:\Program Files\Microsoft BizTalk Server 2004\SDK\Utilities\TestFramework\SDK\SampleSolution\Test\BiztalkFunctionalTests\RulesTestCases\LoanProcessing.xml</RuleStoreName>
	///		<RuleSetInfoCollectionName>LoanProcessing</RuleSetInfoCollectionName>
	///		<DebugTracking>C:\Program Files\Microsoft BizTalk Server 2004\SDK\Utilities\TestFramework\SDK\SampleSolution\Test\BiztalkFunctionalTests\RulesTestCases\outputtraceforLoanPocessing.txt</DebugTracking>
	///		<SampleXML>C:\Program Files\Microsoft BizTalk Server 2004\SDK\Utilities\TestFramework\SDK\SampleSolution\Test\BiztalkFunctionalTests\RulesTestCases\SampleLoan.xml</SampleXML>
	///		<XSD>C:\Program Files\Microsoft BizTalk Server 2004\SDK\Utilities\TestFramework\SDK\SampleSolution\Test\BiztalkFunctionalTests\RulesTestCases\Case</XSD>			
	///		<ResultFile>C:\Program Files\Microsoft BizTalk Server 2004\SDK\Utilities\TestFramework\SDK\SampleSolution\Test\BiztalkFunctionalTests\RulesTestCases\LoanProcessingResult.xml</ResultFile>
	///	</TestStep>
	///	</code>
	///	
	///	<list type="table">
	///		<listheader>
	///			<term>Tag</term>
	///			<description>Description</description>
	///		</listheader>
	///		<item>
	///			<term>RuleStoreName</term>
	///			<description>The location of the rule store</description>
	///		</item>
	///		<item>
	///			<term>RuleSetInfoCollectionName</term>
	///			<description>The name of the rule set collection</description>
	///		</item>
	///		<item>
	///			<term>DebugTracking</term>
	///			<description>Location of the debug tracking</description>
	///		</item>
	///		<item>
	///			<term>SampleXML</term>
	///			<description>Location of the FILE containing the input data</description>
	///		</item>
	///		<item>
	///			<term>XSD</term>
	///			<description>The Xsd scehama</description>
	///		</item>
	///		<item>
	///			<term>ResultFile</term>
	///			<description>The result file to compare against</description>
	///		</item>
	///	</list>
	///	</remarks>

	public class RuleEngineStep : ITestStep
	{	
		/// <summary>
		/// ITestStep.Execute() implementation
		/// </summary>
		/// <param name='testConfig'>The Xml fragment containing the configuration for this test step</param>
		/// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
		public void Execute(System.Xml.XmlNode testConfig , Context context)
		{
			// Using Policy Tester
			 
			// Retrieve Rule-Set from Policy file
			string RuleStoreName = context.ReadConfigAsString(testConfig, "RuleStoreName");
			string RuleSetInfoCollectionName =context.ReadConfigAsString(testConfig, "RuleSetInfoCollectionName");
			string DebugTracking = context.ReadConfigAsString(testConfig, "DebugTracking");
			string SampleXML = context.ReadConfigAsString(testConfig, "SampleXML");
			string XSD = context.ReadConfigAsString(testConfig, "XSD");
			string ResultFile = context.ReadConfigAsString(testConfig, "ResultFile");

			RuleStore ruleStore = new FileRuleStore(RuleStoreName);
			RuleSetInfoCollection rsInfo = ruleStore.GetRuleSets(RuleSetInfoCollectionName, RuleStore.Filter.Latest);
			if (rsInfo.Count != 1)
			{
				// oops ... error
				throw new ApplicationException();
			}
			
			RuleSet ruleset = ruleStore.GetRuleSet(rsInfo[0]);
			
			// Create an instance of the DebugTrackingInterceptor
			DebugTrackingInterceptor dti = new DebugTrackingInterceptor(DebugTracking);

			// Create an instance of the Policy Tester class
			PolicyTester policyTester = new PolicyTester(ruleset);

			XmlDocument xd1 = new XmlDocument();
			xd1.Load(SampleXML);
								
			TypedXmlDocument doc1 = new TypedXmlDocument(XSD, xd1);

			// Execute Policy Tester
			try
			{
				policyTester.Execute(doc1, dti);
			}
			catch (Exception e) 
			{
				context.LogException(e);
				throw;
			}
			
			FileInfo f = new FileInfo(ResultFile);
			StreamWriter w = f.CreateText();
			w.Write(doc1.Document.OuterXml);
			w.Close();
		}
	}
}
