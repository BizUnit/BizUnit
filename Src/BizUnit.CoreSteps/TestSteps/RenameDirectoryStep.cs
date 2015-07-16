//---------------------------------------------------------------------
// File: RenameDirectoryStep.cs
// 
// Summary: 
//
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
	/// The RenameDirectoryStep renames a directory, this test step is often used for negative test scenarios
	/// </summary>
	/// 
	/// <remarks>
	/// The following shows an example of the Xml representation of this test step.
	/// 
	/// <code escaped="true">
	///	<TestStep assemblyPath="" typeName="BizUnit.RenameDirectoryStep">
	///		<SourceDirectory>.\Send_01</SourceDirectory>
	///		<DestinationDirectory>.\Send_01x</DestinationDirectory>
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
	///			<description>The current path of the directory to rename</description>
	///		</item>
	///		<item>
	///			<term>DestinationDirectory</term>
	///			<description>The new path of the directory being renamed</description>
	///		</item>
	///	</list>
	///	</remarks>
    [Obsolete("RenameDirectoryStep has been deprecated. Investigate the BizUnit.TestSteps namespace.")]
	public class RenameDirectoryStep : ITestStep
	{
		/// <summary>
		/// ITestStep.Execute() implementation
		/// </summary>
		/// <param name='testConfig'>The Xml fragment containing the configuration for this test step</param>
		/// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
		public void Execute(XmlNode testConfig, Context context)
		{
			string srcDirectory = context.ReadConfigAsString( testConfig, "SourceDirectory");
			string dstDirectory = context.ReadConfigAsString( testConfig, "DestinationDirectory");

			context.LogInfo("About to renme the directory \"{0}\" to \"{1}\"", srcDirectory, dstDirectory);

			var di = new DirectoryInfo(srcDirectory);
			di.MoveTo(dstDirectory);
		}
	}
}
