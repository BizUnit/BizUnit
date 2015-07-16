//---------------------------------------------------------------------
// File: FileMoveStep.cs
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
	using System.IO;

	/// <summary>
    /// The FileMoveStep moves a file from one directory to another.
    /// </summary>
    /// 
    /// <remarks>
    /// The following shows an example of the Xml representation of this test step.
    /// 
    /// <code escaped="true">
    ///	<TestStep assemblyPath="" typeName="BizUnit.FileMoveStep">
    ///		<SourcePath>.\Rec_01\InDoc1.xml</SourcePath>
    ///		<DestinationPath>.\Output\InDoc1.xml</DestinationPath>
    ///	</TestStep>
    ///	</code>
    ///	
    ///	<list type="table">
    ///		<listheader>
    ///			<term>Tag</term>
    ///			<description>Description</description>
    ///		</listheader>
    ///		<item>
    ///			<term>SourcePath</term>
    ///			<description>Source of the file</description>
    ///		</item>
    ///		<item>
    ///			<term>DestinationPath</term>
    ///			<description>Destination for the file</description>
    ///		</item>
    ///	</list>
    ///	</remarks>
    [Obsolete("FileMoveStep has been deprecated. Investigate the BizUnit.TestSteps namespace.")]
    public class FileMoveStep : ITestStep
	{
		/// <summary>
		/// ITestStep.Execute() implementation
		/// </summary>
		/// <param name='testConfig'>The Xml fragment containing the configuration for this test step</param>
		/// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
		public void Execute(XmlNode testConfig, Context context)
        {
			string sourcePath = context.ReadConfigAsString(testConfig, "SourcePath");
			string destinationPath = context.ReadConfigAsString(testConfig, "DestinationPath");

			File.Move( sourcePath, destinationPath ) ;

			context.LogInfo( "FileMoveStep has moved file: \"{0}\" to \"{1}\"", sourcePath, destinationPath ) ;
		}
	}
}
