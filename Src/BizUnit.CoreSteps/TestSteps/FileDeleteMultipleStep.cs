//---------------------------------------------------------------------
// File: FileDeleteMultipleStep.cs
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
    using System.Xml;
	using System.IO;
    using BizUnitOM;

	/// <summary>
	/// The FileDeleteMultipleStep deletes a FILE specified at a given location.
	/// </summary>
	/// 
	/// <remarks>
	/// The following shows an example of the Xml representation of this test step.
	/// 
	/// <code escaped="true">
	///	<TestStep assemblyPath="" typeName="BizUnit.FileDeleteMultipleStep">
	///		<Directory>C:\Recv2\</Directory>
	///		<SearchPattern>{*}.xml</SearchPattern>
	///	</TestStep>
	///	</code>
	///	
	///	<list type="table">
	///		<listheader>
	///			<term>Tag</term>
	///			<description>Description</description>
	///		</listheader>
	///		<item>
	///			<term>Directory</term>
	///			<description>The directory to search for files to delete</description>
	///		</item>
	///		<item>
	///			<term>SearchPattern</term>
	///			<description>The FILE mask used to search for FILE's to delete, e.g. PO_{*}.xml will delete files such as PO_{87108BAF-6812-418D-A89D-3A2D68E98926}.xml</description>
	///		</item>
	///	</list>
	///	</remarks>
    [Obsolete("FileDeleteMultipleStep has been deprecated. Investigate the BizUnit.TestSteps namespace.")]
    public class FileDeleteMultipleStep : ITestStepOM
	{
	    private string _directory;
	    private string _searchPattern;


	    public string Directory
	    {
	        set
	        {
	            _directory = value;
	        }
	    }

	    public string SearchPattern
	    {
	        set
	        {
	            _searchPattern = value;
	        }
	    }
        
        /// <summary>
		/// ITestStep.Execute() implementation
		/// </summary>
		/// <param name='testConfig'>The Xml fragment containing the configuration for this test step</param>
		/// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
		public void Execute(XmlNode testConfig, Context context)
		{
			_directory = context.ReadConfigAsString( testConfig, "Directory" );
			_searchPattern = context.ReadConfigAsString( testConfig, "SearchPattern" );

            Execute(context);
		}

	    public void Execute(Context context)
	    {
            var di = new DirectoryInfo(_directory);
            var files = di.GetFiles(_searchPattern);

            context.LogInfo("{0} files were found matching the File Mask: \"{1}\" in the directory: \"{2}\"", files.Length, _searchPattern, _directory);

            // Count all the files in each subdirectory that contain the letter "e."
            foreach (var file in files)
            {
                File.Delete(file.FullName);
                context.LogInfo("File: \"{0}\" was successfully deleted.", file.FullName);
            }
        }

        public void Validate(Context context)
	    {
            if (String.IsNullOrEmpty(_directory))
            {
                throw new ArgumentNullException("CreationPath is either null or of zero length");
            }
            _directory = context.SubstituteWildCards(_directory);

            if (String.IsNullOrEmpty(_searchPattern))
            {
                throw new ArgumentNullException("SourcePath is either null or of zero length");
            }
            _searchPattern = context.SubstituteWildCards(_searchPattern);
        }
	}
}
