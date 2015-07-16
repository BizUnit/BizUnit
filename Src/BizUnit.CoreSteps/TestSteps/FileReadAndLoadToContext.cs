//---------------------------------------------------------------------
// File: FileReadAndLoadToContext.cs
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
using BizUnit.CoreSteps.Utilities;

namespace BizUnit.CoreSteps.TestSteps
{
	/// <summary>
    /// The FileReadAndLoadToContext reads a FILE from a given locaton and loads the contents into the context.
	/// </summary>
	/// 
	/// <remarks>
	/// The following shows an example of the Xml representation of this test step.
	/// 
	/// <code escaped="true">
    ///	<TestStep assemblyPath="" typeName="BizUnit.FileReadAndLoadToContext">
    ///		<FilePath>C:\Recv2\data.xml</FilePath>
    ///		<ContextPropertyName>PerfmonCounterList</ContextPropertyName>
    ///		<TimeOut>2000</TimeOut>
    ///	</TestStep>
	///	</code>
	///	
	///	<list type="table">
	///		<listheader>
	///			<term>Tag</term>
	///			<description>Description</description>
	///		</listheader>
	///		<item>
    ///			<term>FilePath</term>
	///			<description>The path to the FILE to read</description>
	///		</item>
	///		<item>
    ///			<term>ContextPropertyName</term>
	///			<description>The name of the context property to write the FILE to</description>
	///		</item>
    ///		<item>
    ///			<term>TimeOut</term>
    ///			<description>The time to wait for the FILE (milli seconds)(optional)</description>
    ///		</item>
    ///	</list>
	///	</remarks>
    [Obsolete("FileReadAndLoadToContext has been deprecated. Investigate the BizUnit.TestSteps namespace.")]
    class FileReadAndLoadToContext : ITestStep
    {
		/// <summary>
		/// ITestStep.Execute() implementation
		/// </summary>
		/// <param name='testConfig'>The Xml fragment containing the configuration for this test step</param>
		/// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public void Execute(XmlNode testConfig, Context context)
        {
            string filePath = context.ReadConfigAsString(testConfig, "FilePath");
            string contextPropertyName = context.ReadConfigAsString(testConfig, "ContextPropertyName");
            double timeOut = context.ReadConfigAsDouble(testConfig, "TimeOut", true);

            context.LogInfo("About to load the context property: {0} with the contents of the file: (1)", contextPropertyName, filePath);
		    
            // Read the FILE from disc...
            MemoryStream ms = StreamHelper.LoadFileToStream(filePath, timeOut);
            ms.Seek(0, SeekOrigin.Begin);
            var sr = new StreamReader(ms);
            string fileData = sr.ReadToEnd();

            // Write FILE contents to the context...
            context.Add(contextPropertyName, fileData);
        }
    }
}
