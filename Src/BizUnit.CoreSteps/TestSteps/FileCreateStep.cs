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
    using System.Xml;
    using BizUnitOM;

    /// <summary>
    /// The FileCreateStep creates a new FILE in the specified directory.
    /// </summary>
    /// 
    /// <remarks>
    /// The following shows an example of the Xml representation of this test step.
    /// 
    /// <code escaped="true">
    ///	<TestStep assemblyPath="" typeName="BizUnit.FileCreateStep">
    ///		<SourcePath>.\TestData\InDoc1.xml</SourcePath>
    ///		<CreationPath>.\Rec_01\InDoc1.xml</CreationPath>
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
    ///			<description>The location of the input FILE to be copied to the CreationPath</description>
    ///		</item>
    ///		<item>
    ///			<term>CreationPath</term>
    ///			<description>The location of the destination FILE</description>
    ///		</item>
    ///	</list>
    ///	</remarks>
    [Obsolete("FileCreateStep has been deprecated. Investigate the BizUnit.TestSteps namespace.")]
    public class FileCreateStep : ITestStepOM
    {
        private string _creationPath;
        private string _sourcePath;
        private const string FileCreationPathContextKey = "FileCreateStep-CreationPath";

        public string SourcePath
        {
           set
           {
               _sourcePath = value;
           }
        }

        public string CreationPath
        {
            set
            {
                _creationPath = value;
            }
        }

        /// <summary>
        /// ITestStep.Execute() implementation
        /// </summary>
        /// <param name='testConfig'>The Xml fragment containing the configuration for this test step</param>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public void Execute(XmlNode testConfig, Context context)
        {
            _sourcePath = context.ReadConfigAsString(testConfig, "SourcePath");
            _creationPath = context.ReadConfigAsString(testConfig, "CreationPath");

            Execute(context);
        }

        public void Execute(Context context)
        {
            FileStream dstFs = null;
            FileStream srcFs = null;

            try
            {
                context.LogInfo("FileCreateStep about to copy the data from File: {0} to the File: {1}", _sourcePath, _creationPath);

                srcFs = File.Open(_sourcePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                dstFs = File.Create(_creationPath);
                var buff = new byte[4096];

                var read = srcFs.Read(buff, 0, 4096);

                while (read > 0)
                {
                    dstFs.Write(buff, 0, read);
                    read = srcFs.Read(buff, 0, 4096);
                }

                context.Add(FileCreationPathContextKey, _creationPath, true);
            }
            finally
            {
                if (null != srcFs)
                {
                    srcFs.Close();
                }

                if (null != dstFs)
                {
                    dstFs.Close();
                }
            }
        }

        public void Validate(Context context)
        {
            if (string.IsNullOrEmpty(_creationPath))
            {
                throw new ArgumentNullException("CreationPath is either null or of zero length");
            }
            _creationPath = context.SubstituteWildCards(_creationPath);

            if (string.IsNullOrEmpty(_sourcePath))
            {
                throw new ArgumentNullException("SourcePath is either null or of zero length");
            }
            _sourcePath = context.SubstituteWildCards(_sourcePath);
        }
    }
}
