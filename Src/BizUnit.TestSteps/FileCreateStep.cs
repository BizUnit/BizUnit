//---------------------------------------------------------------------
// File: FileCreateStep.cs
// 
// Summary: 
//
//---------------------------------------------------------------------
// Copyright (c) 2004-2010, Kevin B. Smith. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------

namespace BizUnitCoreTestSteps
{
    using System;
    using System.IO;
    using BizUnit;

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

    public class FileCreateStep : TestStepBase
    {
        private string _creationPath;
        private string _sourcePath;
        private const string FileCreationPathContextKey = "FileCreateStep-CreationPath";
        private const int BuffSize = 4096;

        public DataLoaderBase DataSource { get; set; }

        public string CreationPath
        {
            set
            {
                _creationPath = value;
            }
            get
            {
                return _creationPath;
            }
        }

        /// <summary>
        /// Execute() implementation
        /// </summary>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public override void Execute(Context context)
        {
            FileStream dstFs = null;
            Stream srcFs = null;

            try
            {
                context.LogInfo("FileCreateStep about to copy the data from File: {0} to the File: {1}", _sourcePath, _creationPath);

                srcFs = DataSource.Load(context);
                dstFs = File.Create(_creationPath);
                var buff = new byte[BuffSize];

                int read = srcFs.Read(buff, 0, BuffSize);

                while (read > 0)
                {
                    dstFs.Write(buff, 0, read);
                    read = srcFs.Read(buff, 0, BuffSize);
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

        public override void Validate(Context context)
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
