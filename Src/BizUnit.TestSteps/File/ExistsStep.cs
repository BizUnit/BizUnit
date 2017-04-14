//---------------------------------------------------------------------
// File: ExistsStep.cs
// 
// Summary: 
//
//---------------------------------------------------------------------
// Copyright (c) 2004-2017, Kevin B. Smith. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------

using System;
using System.IO;
using BizUnit.Common;
using BizUnit.TestBuilder;
using System.Threading;

namespace BizUnit.TestBuilderteps.File
{
    ///<summary>
    /// Test step to check the existance of a file
    ///</summary>
    public class ExistsStep : TestStepBase
    {
        ///<summary>
        /// The time to wait in milisecs for the file, after which the step will fail if the file is not found
        ///</summary>
        public int Timeout { get; set; }

        ///<summary>
        /// The number of files to find
        ///</summary>
        public int ExpectedNoOfFiles { get; set; }

        ///<summary>
        /// The directory path to search
        ///</summary>
        public string DirectoryPath { get; set; }

        ///<summary>
        /// Filter to apply to directory path, e.g. "*.xml" or "MyFile*.txt"
        ///</summary>
        public string SearchPattern { get; set; }

        public override void Execute(Context context)
        {
            var now = DateTime.Now;
            string[] filelist = null;
            bool passed = false;
            int numberOfFiles = 0;

            context.LogInfo("About to check directory: '{0}' for files of type: '{1}'", DirectoryPath, SearchPattern);

            do
            {
                // Get the list of files in the directory
                filelist = Directory.GetFiles(DirectoryPath, SearchPattern);
                numberOfFiles = filelist.Length;

                if (filelist.Length == ExpectedNoOfFiles)
                {
                    passed = true;
                    break;
                }

                Thread.Sleep(50);
            } while (now.AddMilliseconds(Timeout) >= DateTime.Now);

            if(!passed)
                // Expecting more than one file 
                throw new ApplicationException(String.Format("Directory does not contain the correct number of files!\n Found: {0} files matching the pattern {1}.", numberOfFiles, SearchPattern));

            context.LogInfo("FilesExistStep found: \"{0}\" files", numberOfFiles);
        }

        public override void Validate(Context context)
        {
            ArgumentValidation.CheckForEmptyString(DirectoryPath, "DirectoryPath");
            ArgumentValidation.CheckForEmptyString(SearchPattern, "SearchPattern");
        }
    }
}
