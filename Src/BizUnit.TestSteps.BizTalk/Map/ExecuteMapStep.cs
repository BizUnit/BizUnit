//---------------------------------------------------------------------
// File: ExecuteMapStep.cs
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
using System.Collections.ObjectModel;
using BizUnit.Common;
using BizUnit.TestSteps.Common;
using BizUnit.Xaml;

namespace BizUnit.TestSteps.BizTalk.Map
{
    /// <summary>
    /// The ExecuteMapStep can be used to execute a map and test the output from it
    /// </summary>
    /// 
    /// <remarks>
    /// The following shows an example of the Xml representation of this test step.
    /// 
    /// <code escaped="true">
    ///	<TestStep assemblyPath="" typeName="BizUnit.BizTalkSteps.ExecuteMapStep, BizUnit.BizTalkSteps, Version=3.1.0.0, Culture=neutral, PublicKeyToken=7eb7d82981ae5162">
    ///		<Map assemblyPath="BizUnit.BizTalkTestArtifacts" 
    ///         typeName="MapSchema1ToSchema2"/>
    ///		<Source>.\TestData\Schema1.xml</Source>
    ///		<Destination>.\TestData\Schema2.actual.xml</Destination>
    ///
    ///		<!-- Note: ContextLoader Step could be any generic validation step -->	
    /// 	<ContextLoaderStep assemblyPath="" typeName="BizUnit.RegExContextLoader">
    ///			<RegEx contextKey="HTTP_Url">/def:html/def:body/def:p[2]/def:form</RegEx>
    ///			<RegEx contextKey="ActionID">/def:html/def:body/def:p[2]/def:form/def:input[3]</RegEx>
    ///			<RegEx contextKey="ActionType">/def:html/def:body/def:p[2]/def:form/def:input[4]</RegEx>
    ///			<RegEx contextKey="HoldEvent">/def:html/def:body/def:p[2]/def:form/def:input[2]</RegEx>
    ///		</ContextLoaderStep>
    /// 
    ///		<!-- Note: Validation step could be any generic validation step -->	
    ///		<ValidationStep assemblyPath="" typeName="BizUnit.XmlValidationStep">
    ///			<XmlSchemaPath>.\TestData\PurchaseOrder.xsd</XmlSchemaPath>
    ///			<XmlSchemaNameSpace>http://SendMail.PurchaseOrder</XmlSchemaNameSpace>
    ///			<XPathList>
    ///				<XPathValidation query="/*[local-name()='PurchaseOrder' and namespace-uri()='http://SendMail.PurchaseOrder']/*[local-name()='PONumber' and namespace-uri()='']">PONumber_0</XPathValidation>
    ///			</XPathList>
    ///		</ValidationStep>			
    ///	</TestStep>
    ///	</code>
    ///	
    ///	<list type="table">
    ///		<listheader>
    ///			<term>Map:assemblyPath</term>
    ///			<description>The assembly containing the BizTalk map to execute.</description>
    ///		</listheader>
    ///		<item>
    ///			<term>Map:typeName</term>
    ///			<description>The typename of the BizTalk map to execute</description>
    ///		</item>
    ///		<item>
    ///			<term>Source</term>
    ///			<description>The relative file location of the input document to be mapped</description>
    ///		</item>
    ///		<item>
    ///			<term>Destination</term>
    ///			<description>The relative file location of the ouput document that has been mapped</description>
    ///		</item>
    ///		<item>
    ///			<term>ContextLoaderStep</term>
    ///			<description>The configuration for the context loader step used to load data into the BizUnit context which may be used by subsequent test steps<para>(optional)</para></description>
    ///		</item>
    ///		<item>
    ///			<term>ValidationStep</term>
    ///			<description>The configuration for the validation step used to validate the contents of the file, the validation step should implement IValidationTestStep<para>(optional)</para></description>
    ///		</item>
    ///	</list>
    ///	</remarks>

    public class ExecuteMapStep : TestStepBase
    {
        private string _mapAssemblyPath;
        private string _mapTypeName;
        private string _source;
        private string _destination;

        public ExecuteMapStep()
        {
            SubSteps = new Collection<SubStepBase>();
        }

        ///<summary>
        /// Gets and sets the assembly path for the .NET type of the map to be executed
        ///</summary>
        public string MapAssemblyPath
        {
            get { return _mapAssemblyPath; }
            set { _mapAssemblyPath = value; }
        }

        ///<summary>
        /// Gets and sets the type name for the .NET type of the map to be executed
        ///</summary>
        public string MapTypeName
        {
            get { return _mapTypeName; }
            set { _mapTypeName = value; }
        }

        ///<summary>
        /// Gets and sets the relative file location of the input document to be mapped
        ///</summary>
        public string Source
        {
            get { return _source; }
            set { _source = value; }
        }

        ///<summary>
        /// Gets and sets the relative file location of the ouput document that has been mapped
        ///</summary>
        public string Destination
        {
            get { return _destination; }
            set { _destination = value; }
        }

        /// <summary>
        /// ITestStep.Execute() implementation
        /// </summary>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public override void Execute(Context context)
        {
            var mapType = ObjectCreator.GetType(_mapTypeName, _mapAssemblyPath); 

            var destDir = Path.GetDirectoryName(_destination);
            if ((destDir.Length > 0) && !Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }

            var bmt = new BizTalkMapTester(mapType);
            bmt.Execute(_source, _destination);

            Stream data;
            using (var fs = new FileStream(_destination, FileMode.Open, FileAccess.Read))
            {
                data = StreamHelper.LoadMemoryStream(fs);
            }

            data.Seek(0, SeekOrigin.Begin);
            foreach (var subStep in SubSteps)
            {
                data = subStep.Execute(data, context);
            }
        }

        /// <summary>
        /// ITestStepOM.Validate() implementation
        /// </summary>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public override void Validate(Context context)
        {
            _source = context.SubstituteWildCards(_source);
            if (!System.IO.File.Exists(_source))
            {
                throw new ArgumentException("Source file does not exist.", _source);
            }

            ArgumentValidation.CheckForEmptyString(_mapTypeName, "MapTypeName");

            _destination = context.SubstituteWildCards(_destination);

            foreach(var step in SubSteps)
            {
                step.Validate(context);
            }
        }
    }
}