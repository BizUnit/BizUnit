//---------------------------------------------------------------------
// File: FactBasedRuleEngineStep.cs
// 
// Summary: 
//
//---------------------------------------------------------------------
// Copyright (c) 2004-2011, Kevin B. Smith. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Xml;
using Microsoft.RuleEngine;
using System.Data.SqlClient;
using System.Data;
using System.Collections.ObjectModel;
using BizUnit.TestSteps.Common;
using BizUnit.Xaml;

namespace BizUnit.TestSteps.BizTalk.Bre
{
    /// <summary>
    /// Summary description for FactBasedRuleEngineStep.
    /// </summary>
    /// 
    /// <remarks>
    /// The following shows an example of the Xml representation of this test step.
    ///
    /// <code escaped="true">
    ///	<TestStep assemblyPath="" typeName="BizUnit.BizTalkSteps.FactBasedRuleEngineStep, BizUnit.BizTalkSteps, Version=3.1.0.0, Culture=neutral, PublicKeyToken=7eb7d82981ae5162">
    ///		<RuleStoreName>C:\Program Files\Microsoft BizTalk Server 2004\SDK\Utilities\TestFramework\SDK\SampleSolution\Test\BiztalkFunctionalTests\RulesTestCases\LoanProcessing.xml</RuleStoreName>
    ///     <RuleSetInfoCollectionName>LoanProcessing</RuleSetInfoCollectionName>
    ///     <DebugTracking>C:\Program Files\Microsoft BizTalk Server 2004\SDK\Utilities\TestFramework\SDK\SampleSolution\Test\BiztalkFunctionalTests\RulesTestCases\outputtraceforLoanPocessing.txt</DebugTracking>
    ///     <ResultFilePath>C:\Program Files\Microsoft BizTalk Server 2004\SDK\Utilities\TestFramework\SDK\SampleSolution\Test\BiztalkFunctionalTests\RulesTestCases</ResultFilePath>
    ///     <Facts>
    ///        <Fact type="document" schemaType="LoanProcessing" instanceDocument="C:\Program Files\Microsoft BizTalk Server 2004\SDK\Utilities\TestFramework\SDK\SampleSolution\Test\BiztalkFunctionalTests\RulesTestCases\SampleLoan.xml"/>
    ///        <Fact type="object" assemblyPath="C:\Program Files\Microsoft Biztalk Server\SDK\Samples\Business Rules\Business Rules Hello World1\MySampleLibrary\bin\Debug\MySampleLibrary.dll" typeName="Microsoft.Samples.BizTalk.BusinessRulesHelloWorld1.MySampleLibrary.MySampleBusinessObject"/>
    ///        <Fact type="dataConnection" connectionString="" dataset="" tableName=""/>
    ///        <Fact type="dataTable" connectionString="" command="" dataset="" tableName=""/>
    ///        <Fact type="dataRow" connectionString="" command="" dataset="" tableName=""/>
    ///     </Facts>
    /// 
    ///		<!-- Note: Validation step could be any generic validation step -->	
    ///		<ValidationStep assemblyPath="" typeName="BizUnit.XmlValidationStep">
    ///			<XmlSchemaPath>.\TestData\PurchaseOrder.xsd</XmlSchemaPath>
    ///			<XmlSchemaNameSpace>http://SendMail.PurchaseOrder</XmlSchemaNameSpace>
    ///			<XPathList>
    ///				<XPathValidation query="/*[local-name()='PurchaseOrder' and namespace-uri()='http://SendMail.PurchaseOrder']/*[local-name()='PONumber' and namespace-uri()='']">PONumber_0</XPathValidation>
    ///			</XPathList>
    ///		</ValidationStep>			
    /// 
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
    ///			<term>ResultFilePath</term>
    ///			<description>The path used to write updated fact documents to</description>
    ///		</item>
    ///		<item>
    ///			<term>Facts</term>
    ///			<description>Facts to pass to rules engine prior to ruleset execution</description>
    ///		</item>	
    ///	</list>
    ///	</remarks>

    public class FactBasedRuleEngineStep : TestStepBase
    {
        private Collection<Fact> _factsList = new Collection<Fact>(); 
        
        public string RuleStoreName { get; set; }
        public string RuleSetInfoCollectionName { get; set; }
        public string DebugTracking { get; set; }

        public FactBasedRuleEngineStep()
        {
            SubSteps = new Collection<SubStepBase>();            
        }

        public Collection<Fact> FactsList 
        { 
            get
            {
                return _factsList;
            } 
            set
            {
                _factsList = value;
            }
        }

        /// <summary>
        /// TestStepBase.Execute() implementation
        /// </summary>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public override void Execute(Context context)
        {
            var fi = new System.IO.FileInfo(RuleStoreName);

            if (!fi.Exists)
            {
                throw new FileNotFoundException("RuleStoreName", RuleStoreName);
            }

            var ruleStore = new FileRuleStore(fi.FullName);
            var rsInfo = ruleStore.GetRuleSets(RuleSetInfoCollectionName, RuleStore.Filter.Latest);
            if (rsInfo.Count != 1)
            {
                // oops ... error
                throw new ApplicationException(String.Format("RuleStore {0} did not contain RuleSet {1}", RuleStoreName, RuleSetInfoCollectionName));
            }

            var ruleset = ruleStore.GetRuleSet(rsInfo[0]);

            // Create an instance of the DebugTrackingInterceptor
            var dti = new DebugTrackingInterceptor(DebugTracking);

            // Create an instance of the Policy Tester class
            var policyTester = new PolicyTester(ruleset);

            // load the facts into array
            var facts = new object[_factsList.Count]; 
            var i = 0;

            foreach (var currentFact in _factsList)
            {
                switch (currentFact.GetFactType)
                {
                    case "ObjectFact":
                        {
                            var fact = currentFact as ObjectFact;

                            object[] objectArgs = null;
                            if (null != fact.Args)
                            {
                                objectArgs = fact.Args.Split(new char[] { ',' });
                            }

                            System.Reflection.Assembly asm;
                            Type type;
                            if (fact.AssemblyPath.Length > 0)
                            {
                                asm = System.Reflection.Assembly.LoadWithPartialName(fact.AssemblyPath);
                                if (asm == null)
                                {
                                    // fail
                                    throw (new Exception("failed to create type " + fact.Type));
                                }
                                type = asm.GetType(fact.Type, true, false);
                            }
                            else
                            {
                                // must be in path
                                type = Type.GetType(fact.Type);
                            }

                            facts[i] = Activator.CreateInstance(type, objectArgs);
                            break;
                        }
                    case "DocumentFact":
                        {
                            var fact = currentFact as DocumentFact;
                            var xd1 = new XmlDocument();
                            xd1.Load(fact.InstanceDocument);
                            var txd = new TypedXmlDocument(fact.SchemaType, xd1);
                            facts[i] = txd; 
                            break;
                        }
                    case "DataConnectionFact":
                        {
                            var fact = currentFact as DataConnectionFact;
                            var conn = new SqlConnection(fact.ConnectionString);
                            conn.Open();
                            var dc = new DataConnection(fact.Dataset, fact.TableName, conn);
                            facts[i] = dc;
                            break;
                        }
                    case "dataTable":
                    case "dataRow":
                        {
                            var fact = currentFact as DataTableFact;

                            var dAdapt = new SqlDataAdapter();
                            dAdapt.TableMappings.Add("Table", fact.TableName);
                            var conn = new SqlConnection(fact.ConnectionString);
                            conn.Open();
                            var myCommand = new SqlCommand(fact.Command, conn) {CommandType = CommandType.Text};
                            dAdapt.SelectCommand = myCommand;
                            var ds = new DataSet(fact.Dataset);
                            dAdapt.Fill(ds);
                            var tdt = new TypedDataTable(ds.Tables[fact.TableName]);
                            if (fact.Type == "dataRow")
                            {
                                var tdr = new TypedDataRow(ds.Tables[fact.TableName].Rows[0], tdt);
                                facts[i] = tdr;
                            }
                            else
                            {
                                facts[i] = tdt;
                            }
                            break;
                        }
                }
                i++;
            }

            // Execute Policy Tester
            try
            {
                policyTester.Execute(facts, dti);
            }
            catch (Exception e)
            {
                context.LogException(e);
                throw;
            }
            finally
            {
                dti.CloseTraceFile();
            }

            // write out all document instances passed in
            foreach (object fact in facts)
            {
                switch (fact.GetType().Name)
                {
                    case "TypedXmlDocument":
                        {
                            var txd = (TypedXmlDocument)fact;

                            context.LogData("TypedXmlDocument result: ", txd.Document.OuterXml);
                            Stream data = StreamHelper.LoadMemoryStream(txd.Document.OuterXml);

                            // Validate if configured...
                            // HACK: We need to prevent ExecuteValidator for /each/ TypedXmlDocument
                            if (txd.DocumentType == "UBS.CLAS.PoC.Schemas.INSERTS")
                            {
                                foreach (var subStep in SubSteps)
                                {
                                    data = subStep.Execute(data, context);
                                }

                            }

                            break;
                        }
                    case "DataConnection":
                        {
                            var dc = (DataConnection)fact;
                            dc.Update(); // persist any changes
                            break;
                        }
                    case "TypedDataTable":
                        {
                            var tdt = (TypedDataTable)fact;
                            tdt.DataTable.AcceptChanges();
                            break;
                        }
                    case "TypedDataRow":
                        {
                            var tdr = (TypedDataRow)fact;
                            tdr.DataRow.AcceptChanges();
                            break;
                        }
                }
            }
        }

        public override void Validate(Context context)
        {
            ;
        }
    }
}
