//---------------------------------------------------------------------
// File: Delete.cs
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
using System.Collections.ObjectModel;
using BizUnit.Xaml;

namespace BizUnit.TestSteps.File
{
    ///<summary>
    /// Given a file path, the step deletes the files
    ///</summary>
    public class DeleteStep : TestStepBase
    {
        public DeleteStep()
        {
            FilePathsToDelete = new Collection<string>();
        }

        ///<summary>
        /// Collection of file paths to delete. May be the full file path, or a directory path with a wild card to search for. e.g. C:\Temp\Foo.xml or C:\Temp\*.xml
        ///</summary>
        public Collection<string> FilePathsToDelete { get; set; }

        /// <summary>
        /// TestStepBase.Execute() implementation
        /// </summary>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public override void Execute(Context context)
        {
            foreach (var path in FilePathsToDelete)
            {
                DeleteFile(path, context);
            }
        }

        private static void DeleteFile(string filePathToDelete, Context context)
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePathToDelete);
            var fileName = Path.GetFileName(filePathToDelete);
            var fileExtension = Path.GetExtension(filePathToDelete).Remove(0, 1); // Remove '.'
            var directory = Path.GetDirectoryName(filePathToDelete);

            if (0 != fileNameWithoutExtension.CompareTo("*"))
            {
                System.IO.File.Delete(filePathToDelete);
            }
            else if (!string.IsNullOrEmpty(fileExtension) && !string.IsNullOrEmpty(directory))
            {
                var di = new DirectoryInfo(directory);
                var files = di.GetFiles(fileName);

                context.LogInfo("{0} files were found matching the File Mask: \"{1}\" in the directory: \"{2}\"", files.Length, fileName, directory);

                foreach (var file in files)
                {
                    System.IO.File.Delete(file.FullName);
                    context.LogInfo("File: \"{0}\" was successfully deleted.", file.FullName);
                }
            }
            else
            {
                throw new ApplicationException(string.Format("The file path: {0} is not valid", filePathToDelete));
            }

            context.LogInfo("File.Delete has deleted file: {0}", filePathToDelete);
        }

        ///<summary>
        /// TestStepBase.Validate() implementation
        ///</summary>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        ///<exception cref="ArgumentNullException"></exception>
        public override void Validate(Context context)
        {
            if (null == FilePathsToDelete || 0 == FilePathsToDelete.Count)
            {
                throw new ArgumentNullException("FilePathsToDelete is either null or of zero length");
            }

            for (int c = 0; c < FilePathsToDelete.Count; c++)
            {
                FilePathsToDelete[c] = context.SubstituteWildCards(FilePathsToDelete[c]);
            }
        }
    }
}
