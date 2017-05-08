//---------------------------------------------------------------------
// File: ExecuteMapStep.cs
// 
// Summary: 
//
//---------------------------------------------------------------------
// Copyright (c) 2004-2011, Kevin B. Smith. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Collections.ObjectModel;
using BizUnit.TestSteps.Common;
using BizUnit.Core.TestBuilder;
using BizUnit.Core.Common;

namespace BizUnit.TestSteps.BizTalk.Map
{
    /// <summary>
    /// The ExecuteMapStep can be used to execute a map and test the output from it
    /// </summary>
    public class ExecuteMapStep : TestStepBase
    {
        private string _assemblyPath;
        private string _mapTypeName;
        private string _sourcePath;
        private string _destinationPath;

        /// <summary>
        /// The ExecuteMapStep can be used to execute a map and test the output from it
        /// </summary>
        public ExecuteMapStep()
        {
            SubSteps = new Collection<SubStepBase>();
        }

        ///<summary>
        /// The assembly path for the .NET type of the map to be executed
        ///</summary>
        public string AssemblyPath
        {
            get { return _assemblyPath; }
            set { _assemblyPath = value; }
        }

        ///<summary>
        /// The type name for the .NET type of the map to be executed
        ///</summary>
        public string MapTypeName
        {
            get { return _mapTypeName; }
            set { _mapTypeName = value; }
        }

        ///<summary>
        /// The file location of the input document to be mapped
        ///</summary>
        public string SourcePath
        {
            get { return _sourcePath; }
            set { _sourcePath = value; }
        }

        ///<summary>
        /// File location of the ouput document that has been mapped
        ///</summary>
        public string DestinationPath
        {
            get { return _destinationPath; }
            set { _destinationPath = value; }
        }

        /// <summary>
        /// Execute() implementation
        /// </summary>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public override void Execute(Context context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var mapType = AssemblyHelper.LoadAssembly(this.AssemblyPath).GetType(this.MapTypeName, true, false);

            var destDir = Path.GetDirectoryName(_destinationPath);
            if ((destDir.Length > 0) && !Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }

            var bmt = new BizTalkMapTester(mapType);
            context.LogInfo("Executing map '{0}'...", this.MapTypeName);
            bmt.Execute(_sourcePath, _destinationPath);

            Stream data;
            using (var fs = new FileStream(_destinationPath, FileMode.Open, FileAccess.Read))
            {
                data = StreamHelper.LoadMemoryStream(fs);
            }

            data.Seek(0, SeekOrigin.Begin);
            foreach (var subStep in SubSteps)
            {
                data = subStep.Execute(data, context);
            }
        }

        /// <summary>
        /// Validate initializer values.
        /// </summary>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public override void Validate(Context context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            ArgumentValidation.CheckForEmptyString(this.SourcePath, "SourcePath");
            _sourcePath = context.SubstituteWildCards(_sourcePath);
            if (!System.IO.File.Exists(_sourcePath))
            {
                throw new ArgumentException("Source file does not exist.", _sourcePath);
            }

            ArgumentValidation.CheckForEmptyString(_mapTypeName, "MapTypeName");

            ArgumentValidation.CheckForEmptyString(this.DestinationPath, "DestinationPath");
            _destinationPath = context.SubstituteWildCards(_destinationPath);

            foreach(var step in SubSteps)
            {
                step.Validate(context);
            }
        }
    }
}