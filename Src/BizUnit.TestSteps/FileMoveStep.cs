//---------------------------------------------------------------------
// File: FileMoveStep.cs
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
    /// The FileMoveStep moves a file from one directory to another.
    /// </summary>
    /// 
    /// <remarks>
    /// The following shows an example of the Xml representation of this test step.
    /// 
    /// <code escaped="true">
    ///	<bucts:FileMoveStep
    ///     SourcePath="C:\source.txt"
    ///     DestinationPath="C:\destination.txt"
    ///     RunConcurrently="False"
    ///     FailOnError="True" />
    ///	</code>
    ///	
    ///	<list type="table">
    ///		<listheader>
    ///			<term>Tag</term>
    ///			<description>Description</description>
    ///		</listheader>
    ///		<item>
    ///			<term>SourcePath</term>
    ///			<description>Source of the file</description>
    ///		</item>
    ///		<item>
    ///			<term>DestinationPath</term>
    ///			<description>Destination for the file</description>
    ///		</item>
    ///	</list>
    ///	</remarks>

    public class FileMoveStep : TestStepBase
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
			File.Move( _sourcePath, _destinationPath ) ;

            context.LogInfo( "FileMoveStep has moved file: \"{0}\" to \"{1}\"", _sourcePath, _destinationPath ) ;
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
