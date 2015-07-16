//---------------------------------------------------------------------
// File: FilesExistStep.cs
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

namespace BizUnit.CoreSteps.TestSteps
{
	using System;
	using System.IO;
	using System.Threading;
	using System.Xml;

	/// <summary>
    /// The FilesExistStep step determines the number of file in the specified directory which match the search pattern
    /// and compares this to the expected result.
    /// </summary>
    /// 
    /// <remarks>
    /// The following shows an example of the Xml representation of this test step.
    /// 
    /// <code escaped="true">
    /// <TestStep assemblyPath="" typeName="BizUnit.FilesExistStep">
    ///		<Timeout>3000</Timeout>
    ///		<DirectoryPath>.\Rec_03</DirectoryPath>
    ///		<SearchPattern>*.xml</SearchPattern>
    ///		<ExpectedNoOfFiles>1</ExpectedNoOfFiles>
    /// </TestStep>
    ///	</code>
    ///	
    ///	<list type="table">
    ///		<listheader>
    ///			<term>Tag</term>
    ///			<description>Description</description>
    ///		</listheader>
    ///		<item>
    ///			<term>Timeout</term>
    ///			<description>Time to wait before checking</description>
    ///		</item>
    ///		<item>
    ///			<term>DirectoryPath</term>
    ///			<description>Directory path to check</description>
    ///		</item>
    ///		<item>
    ///			<term>SearchPattern</term>
    ///			<description>Matching pattern for files</description>
    ///		</item>
    ///		<item>
    ///			<term>ExpectedNoOfFiles</term>
    ///			<description>Expected number of files</description>
    ///		</item>
    ///	</list>
    ///	</remarks>
    [Obsolete("FilesExistStep has been deprecated. Investigate the BizUnit.TestSteps namespace.")]
	public class FilesExistStep : ITestStep
	{
		/// <summary>
		/// ITestStep.Execute() implementation
		/// </summary>
		/// <param name='testConfig'>The Xml fragment containing the configuration for this test step</param>
		/// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
		public void Execute(XmlNode testConfig, Context context)
        {
			int timeout = context.ReadConfigAsInt32(testConfig, "Timeout");
			Thread.Sleep(timeout);
			
			// Get the list of files in the directory
			string directoryPath = context.ReadConfigAsString(testConfig, "DirectoryPath");
			string pattern = context.ReadConfigAsString(testConfig, "SearchPattern");
			string [] filelist = Directory.GetFiles( directoryPath, pattern ) ;
			int expectedNoOfFiles = context.ReadConfigAsInt32(testConfig, "ExpectedNoOfFiles");

			if ( filelist.Length != expectedNoOfFiles )
			{
				// Expecting more than one file 
				throw new ApplicationException( String.Format( "Directory does not contain the correct number of files!\n Found: {0} files matching the pattern {1}.", filelist.Length, pattern ) ) ;
			}

            context.LogInfo( "FilesExistStep found: \"{0}\" files", filelist.Length ) ;
		}
	}
}
