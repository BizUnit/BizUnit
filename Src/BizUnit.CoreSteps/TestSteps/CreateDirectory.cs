//---------------------------------------------------------------------
// File: CreateDirectory.cs
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

namespace BizUnit.CoreSteps.TestSteps
{
    using System.Xml;

    /// <summary>
    /// The CreateDirectory creates a new Directory.
    /// </summary>
    /// 
    /// <remarks>
    /// The following shows an example of the Xml representation of this test step.
    /// 
    /// <code escaped="true">
    ///	<TestStep assemblyPath="" typeName="BizUnit.CreateDirectory">
    ///		<DirectoryName>.\TestData\InDoc1.xml</DirectoryName>
    ///	</TestStep>
    ///	</code>
    ///	
    ///	<list type="table">
    ///		<listheader>
    ///			<term>Tag</term>
    ///			<description>Description</description>
    ///		</listheader>
    ///		<item>
    ///			<term>DirectoryName</term>
    ///			<description>The name of the directory to create</description>
    ///		</item>
    ///	</list>
    ///	</remarks>

    [Obsolete("CreateDirectory has been deprecated. Investigate the BizUnit.TestSteps namespace.")]
    public class CreateDirectory : ITestStep
    {
		/// <summary>
		/// ITestStep.Execute() implementation
		/// </summary>
		/// <param name='testConfig'>The Xml fragment containing the configuration for this test step</param>
		/// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        
        public void Execute(XmlNode testConfig, Context context)
        {
            string directoryName = context.ReadConfigAsString(testConfig, "DirectoryName");

            context.LogInfo("About to create the directory: {0}", directoryName);
		    
            System.IO.Directory.CreateDirectory(directoryName);
        }
    }
}
