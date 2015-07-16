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

using System;
using System.IO;
using BizUnit.Xaml;

namespace BizUnit.TestSteps.File
{
    /// <summary>
    /// The FileCreateStep creates a new FILE in the specified directory.
    /// </summary>
    public class CreateStep : TestStepBase
    {
        private const string FileCreationPathContextKey = "FileCreateStep-CreationPath";
        private const int BuffSize = 4096;
        
        ///<summary>
        /// The dataloader to be used as the source for the FILE to be created
        ///</summary>
        public DataLoaderBase DataSource { get; set; }

        ///<summary>
        /// The file path of the file to be created
        ///</summary>
        public string CreationPath { get; set; }

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
                context.LogInfo("FileCreateStep about to copy the data from datasource to: {0}", CreationPath);

                srcFs = DataSource.Load(context);
                dstFs = System.IO.File.Create(CreationPath);
                var buff = new byte[BuffSize];

                int read = srcFs.Read(buff, 0, BuffSize);

                while (read > 0)
                {
                    dstFs.Write(buff, 0, read);
                    read = srcFs.Read(buff, 0, BuffSize);
                }

                context.Add(FileCreationPathContextKey, CreationPath, true);
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
            if (string.IsNullOrEmpty(CreationPath))
            {
                throw new ArgumentNullException("CreationPath is either null or of zero length");
            }
            CreationPath = context.SubstituteWildCards(CreationPath);

            DataSource.Validate(context);
        }
    }
}
