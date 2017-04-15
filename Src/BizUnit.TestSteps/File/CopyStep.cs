
using BizUnit.Core.TestBuilder;
using System;

namespace BizUnit.TestSteps.File
{
    public class CopyStep : TestStepBase
    {

        private string _sourcePath;
        private string _destinationPath;

        /// <summary>
        /// Gets or sets the source path.
        /// </summary>
        /// <value>The source path.</value>
        public string SourcePath
        {
            get { return _sourcePath; }
            set { _sourcePath = value; }
        }

        /// <summary>
        /// Gets or sets the destination path.
        /// </summary>
        /// <value>The destination path.</value>
        public string DestinationPath
        {
            get { return _destinationPath; }
            set { _destinationPath = value; }
        }

        /// <summary>
        /// ITestStep.Execute() implementation
        /// </summary>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public override void Execute(Context context)
        {
            context.LogInfo("Copying '{0}' to '{1}'", _sourcePath, _destinationPath);
            System.IO.File.Copy(_sourcePath, _destinationPath);

            context.LogInfo("FileCopyStep has been copied file from: \"{0}\" to \"{1}\"", _sourcePath, _destinationPath);
        }

        public override void Validate(Context context)
        {
            if (string.IsNullOrEmpty(_destinationPath))
            {
                throw new ArgumentNullException("DestinationPath is either null or of zero length");
            }
            _destinationPath = context.SubstituteWildCards(_destinationPath);

            if (string.IsNullOrEmpty(_sourcePath))
            {
                throw new ArgumentNullException("SourcePath is either null or of zero length");
            }
            _sourcePath = context.SubstituteWildCards(_sourcePath);
        }
    }
}
