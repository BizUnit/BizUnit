//---------------------------------------------------------------------
// File: FileCreateStep.cs
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

	/// <summary>
	/// The WaitOnFileStep is used to wait for a FILE to be written to a given location. The name
    /// of the file waited on is stored in the context under the key: waitedForFileName
	/// </summary>
	/// 
	/// <remarks>
	/// The following shows an example of the Xml representation of this test step.
	/// 
	/// <code escaped="true">
	///	<TestStep assemblyPath="" typeName="BizUnit.WaitOnFileStep">
	///		<Path>n:\\</Path>
	///		<FileFilter>*.xml</FileFilter>
	///		<TimeOut>10000</TimeOut>
	///	</TestStep>
	///	</code>
	///	
	///	<list type="table">
	///		<listheader>
	///			<term>Tag</term>
	///			<description>Description</description>
	///		</listheader>
	///		<item>
	///			<term>Path</term>
	///			<description>The directory to look for the FILE</description>
	///		</item>
	///		<item>
	///			<term>FileFilter</term>
	///			<description>The FILE mask to be used to search for a FILE, e.g. *.xml</description>
	///		</item>
	///		<item>
	///			<term>TimeOut</term>
	///			<description>The time to wait for the FILE to become present in miliseconds</description>
	///		</item>
	///	</list>
	///	</remarks>
    [Obsolete("WaitOnFileStep has been deprecated. Investigate the BizUnit.TestSteps namespace.")]
	public class WaitOnFileStep: ITestStep
	{
		ManualResetEvent _mre;
		string _newFilePath;

		/// <summary>
		/// ITestStep.Execute() implementation
		/// </summary>
		/// <param name='testConfig'>The Xml fragment containing the configuration for this test step</param>
		/// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
		public void Execute(System.Xml.XmlNode testConfig, Context context)
		{
			// read test config...
			string path = context.ReadConfigAsString( testConfig, "Path" );
			string fileFilter = context.ReadConfigAsString( testConfig, "FileFilter" );
			int timeOut = context.ReadConfigAsInt32( testConfig, "TimeOut" );
			
			var watcher = new FileSystemWatcher
			                  {
			                      Path = path,
			                      Filter = fileFilter,
			                      NotifyFilter = NotifyFilters.LastWrite,
			                      EnableRaisingEvents = true,
			                      IncludeSubdirectories = false
			                  };
		    watcher.Changed += OnCreated;
			_mre = new ManualResetEvent(false);

			if(!_mre.WaitOne(timeOut, false))
			{
				throw new Exception(string.Format("WaitOnFileStep timed out after {0} milisecs watching path:{1}, filter{2}", timeOut, path, fileFilter));
			}

            context.LogInfo(string.Format("WaitOnFileStep found the file: {0}", _newFilePath));
            context.Add("waitedForFileName", _newFilePath);
		}

		private void OnCreated(object sender, FileSystemEventArgs e) 
		{ 
			_newFilePath = e.FullPath;
			_mre.Set();
		}
	}
}
