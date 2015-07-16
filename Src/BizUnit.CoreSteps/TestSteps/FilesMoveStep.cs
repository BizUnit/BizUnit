//---------------------------------------------------------------------
// File: FilesMoveStep.cs
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
    /// The FilesMoveStep step moves the contents of one directory to another based on the file mask supplied.
    /// </summary>
    /// 
    /// <remarks>
    /// The following shows an example of the Xml representation of this test step.
    /// 
    /// <code escaped="true">
    ///	<TestStep assemblyPath="" typeName="BizUnit.FilesMoveStep">
    ///		<SourceDirectory>.\Rec_03</SourceDirectory>
    ///		<SearchPattern>*.xml</SearchPattern>
    ///		<DestinationDirectory>.\Rec_04</DestinationDirectory>
    ///	</TestStep>
    ///	</code>
    ///	
    ///	<list type="table">
    ///		<listheader>
    ///			<term>Tag</term>
    ///			<description>Description</description>
    ///		</listheader>
    ///		<item>
    ///			<term>SourceDirectory</term>
    ///			<description>Source directory for the files</description>
    ///		</item>
    ///		<item>
    ///			<term>SearchPattern</term>
    ///			<description>Matching pattern for files</description>
    ///		</item>
    ///		<item>
    ///			<term>DestinationDirectory</term>
    ///			<description>Destination directory for the files</description>
    ///		</item>
    ///	</list>
    ///	</remarks>
    [Obsolete("FilesMoveStep has been deprecated. Investigate the BizUnit.TestSteps namespace.")]
	public class FilesMoveStep : ITestStep
	{
		/// <summary>
		/// ITestStep.Execute() implementation
		/// </summary>
		/// <param name='testConfig'>The Xml fragment containing the configuration for this test step</param>
		/// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
		public void Execute(XmlNode testConfig, Context context)
        {
			string sourcePath = context.ReadConfigAsString(testConfig, "SourceDirectory");
			string pattern = context.ReadConfigAsString(testConfig, "SearchPattern");
			string destinationPath = context.ReadConfigAsString(testConfig, "DestinationDirectory");
			string [] filelist = Directory.GetFiles( sourcePath, pattern ) ;

			foreach( string file in filelist)
			{
				File.Move( file, destinationPath + @"\" + Path.GetFileName( file ) ) ;

				context.LogInfo( "FilesMoveStep has moved file: \"{0}\" to \"{1}\"", file, destinationPath ) ;
			}
		}
	}
}
