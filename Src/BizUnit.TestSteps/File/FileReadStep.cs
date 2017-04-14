
using BizUnit.Common;
using BizUnit.TestBuilder;
using BizUnit.TestBuilderteps.Common;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;

namespace BizUnit.TestSteps.File
{
    public class FileReadStep : TestStepBase
    {
        public FileReadStep() 
        {
            SubSteps = new Collection<SubStepBase>();
        }

        ///<summary>
        /// The time to wait in milisecs for the file, after which the step will fail if the file is not found
        ///</summary>
        public int Timeout { get; set; }

        ///<summary>
        /// The directory path to search
        ///</summary>
        public string DirectoryPath { get; set; }

        ///<summary>
        /// Filter to apply to directory path, e.g. "*.xml" or "MyFile*.txt"
        ///</summary>
        public string SearchPattern { get; set; }

        ///<summary>
        /// Flag to specify whether the files should be deleted upon completion of the test step
        ///</summary>
        public bool DeleteFile { get; set; }        

        /// <summary>
        /// TestStepBase.Execute() implementation
		/// </summary>
		/// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
		public override void Execute(Context context)
        {
            var endTime = DateTime.Now.AddMilliseconds(Timeout);
            bool found = false;
            string[] filelist;
            int fileCount;
            string filePath = String.Empty;

            context.LogInfo("Searching directory: {0}, search pattern: {1}", DirectoryPath, SearchPattern);

            do
            {
                Thread.Sleep(50);

                // Get the list of files in the directory
                filelist = Directory.GetFiles(DirectoryPath, SearchPattern);
                fileCount = filelist.Length;

                if (1 == fileCount)
                {
                    filePath = filelist[0];
                    found = true;
                    break;
                }
            } while (endTime > DateTime.Now);

            if (!found)
            {
                // Expecting more than one file 
                throw new ApplicationException(String.Format("Directory should have contained one file, it contained: {0} file(s)", fileCount));
            }

            context.LogInfo("Found file: {0}", filePath);

            Stream fileData = StreamHelper.LoadFileToStream(filePath, Timeout);
            context.LogData("file contents", fileData);
            fileData.Seek(0, SeekOrigin.Begin);

            // Check it against the validate steps to see if it matches one of them
            foreach (var subStep in SubSteps)
            {
                try
                {
                    fileData.Seek(0, SeekOrigin.Begin);
                    fileData = subStep.Execute(fileData, context);
                }
                catch (Exception ex)
                {
                    context.LogException(ex);
                    throw;
                }
            }

            if (DeleteFile)
                System.IO.File.Delete(filePath);
        }

        public override void Validate(Context context)
        {
            ArgumentValidation.CheckForEmptyString(DirectoryPath, "DirectoryPath");
            ArgumentValidation.CheckForEmptyString(SearchPattern, "SearchPattern");
        }
    }
}
