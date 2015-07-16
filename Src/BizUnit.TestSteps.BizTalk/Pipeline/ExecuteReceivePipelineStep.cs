//---------------------------------------------------------------------
// File: ExecuteReceivePipelineStep.cs
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
using System.Collections.Generic;
using Winterdom.BizTalk.PipelineTesting;
using System.Collections.ObjectModel;
using BizUnit.Common;
using BizUnit.TestSteps.BizTalk.Common;
using BizUnit.Xaml;

namespace BizUnit.TestSteps.BizTalk.Pipeline
{
    /// <summary>
    /// The ExecuteReceivePipelineStep can be used to execute a pipeline and test the output from it
    /// </summary>
    /// 
    /// <remarks>
    /// The following shows an example of the Xml representation of this test step. The step 
    /// can perform a pipeline and output that to an output file
    /// 
    ///	<list type="table">
    ///		<listheader>
    ///			<term>Pipeline:assemblyPath</term>
    ///			<description>The assembly containing the BizTalk pipeline to execute.</description>
    ///		</listheader>
    ///		<item>
    ///			<term>Pipeline:typeName</term>
    ///			<description>The typename of the BizTalk pipeline to execute</description>
    ///		</item>
    ///		<item>
    ///			<term>DocSpecs:assembly</term>
    ///			<description>The assembly containing the BizTalk docspec schema assembly path (multiple)</description>
    ///		</item>
    ///		<item>
    ///			<term>DocSpecs:type</term>
    ///			<description>The assembly containing the BizTalk docspec schema type name (multiple)</description>
    ///		</item>
    ///		<item>
    ///			<term>Source</term>
    ///			<description>The file path of the source input file to execute in the pipeline</description>
    ///		</item>
    ///		<item>
    ///			<term>Source:Encoding</term>
    ///			<description>The excoding type of the input file to execute in the pipeline</description>
    ///		</item>
    ///		<item>
    ///			<term>DestinationDir</term>
    ///			<description>The destination directory to write the pipeline output file(s)</description>
    ///		</item>
    ///		<item>
    ///			<term>DestinationFileFormat</term>
    ///			<description>The file format of the output file(s)</description>
    ///		</item>
    ///		<item>
    ///			<term>ContextFileFormat</term>
    ///			<description>The file format of the output message context file(s)</description>
    ///		</item>
    ///	</list>
    ///	</remarks>

    public class ExecuteReceivePipelineStep : TestStepBase
    {
        private string _pipelineAssemblyPath;
        private string _pipelineTypeName;
        private Collection<DocSpecDefinition> _docSpecsRawList = new Collection<DocSpecDefinition>();
        private Type[] _docSpecs;
        private string _instanceConfigFile;
        private string _source;
        private string _sourceEncoding;
        private string _destinationDir;
        private string _destinationFileFormat;
        private string _inputContextFile;
        private string _outputContextFileFormat;

        ///<summary>
        /// Gets and sets the assembly path for the .NET type of the pipeline to be executed
        ///</summary>
        public string PipelineAssemblyPath
        {
            get { return _pipelineAssemblyPath; }
            set { _pipelineAssemblyPath = value; }
        }

        ///<summary>
        /// Gets and sets the type name for the .NET type of the pipeline to be executed
        ///</summary>
        public string PipelineTypeName
        {
            get { return _pipelineTypeName; }
            set { _pipelineTypeName = value; }
        }

        ///<summary>
        /// Gets and sets the docspecs for the pipeline to be executed. Pairs of typeName, assemblyPath.
        ///</summary>
        public Collection<DocSpecDefinition> DocSpecs
        {
            get { return _docSpecsRawList; }
            private set { _docSpecsRawList = value; }
        }

        ///<summary>
        /// Gets and sets the pipeline instance configuration for the pipeline to be executed
        ///</summary>
        public string InstanceConfigFile
        {
            get { return _instanceConfigFile; }
            set { _instanceConfigFile = value; }
        }

        ///<summary>
        /// Gets and sets the source file path for the input file to the pipeline
        ///</summary>
        public string Source
        {
            get { return _source; }
            set { _source = value; }
        }

        ///<summary>
        /// Gets and sets the source encoding
        ///</summary>
        public string SourceEncoding
        {
            get { return _sourceEncoding; }
            set { _sourceEncoding = value; }
        }

        ///<summary>
        /// Gets and sets the destination of the pipeline output
        ///</summary>
        public string DestinationDir
        {
            get { return _destinationDir; }
            set { _destinationDir = value; }
        }

        ///<summary>
        /// Gets and sets the destination file format 
        ///</summary>
        public string DestinationFileFormat
        {
            get { return _destinationFileFormat; }
            set { _destinationFileFormat = value; }
        }

        ///<summary>
        /// Gets and sets the message context for the input message
        ///</summary>
        public string InputContextFile
        {
            get { return _inputContextFile; }
            set { _inputContextFile = value; }
        }

        ///<summary>
        /// Gets and sets the message context file format for the output message
        ///</summary>
        public string OutputContextFileFormat
        {
            get { return _outputContextFileFormat; }
            set { _outputContextFileFormat = value; }
        }

        /// <summary>
        /// TestStepBase.Execute() implementation
        /// </summary>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public override void Execute(Context context)
        {
            if (_docSpecsRawList.Count > 0)
            {
                var ds = new List<Type>(_docSpecsRawList.Count);
                foreach (var docSpec in _docSpecsRawList)
                {
                    var ass = AssemblyHelper.LoadAssembly((string)docSpec.AssemblyPath);
                    context.LogInfo("Loading DocumentSpec {0} from location {1}.", docSpec.TypeName, ass.Location);
                    var type = ass.GetType(docSpec.TypeName);

                    ds.Add(type);
                }
                _docSpecs = ds.ToArray();
            }

            context.LogInfo("Loading pipeline {0} from location {1}.", _pipelineTypeName, _pipelineAssemblyPath);
            var pipelineType = ObjectCreator.GetType(_pipelineTypeName, _pipelineAssemblyPath);

            var pipelineWrapper = PipelineFactory.CreateReceivePipeline(pipelineType);

            if (!string.IsNullOrEmpty(_instanceConfigFile))
            {
                pipelineWrapper.ApplyInstanceConfig(_instanceConfigFile);
            }

            if (null != _docSpecs)
            {
                foreach (Type docSpec in _docSpecs)
                {
                    pipelineWrapper.AddDocSpec(docSpec);
                }
            }

            MessageCollection mc = null;
            using (Stream stream = new FileStream(_source, FileMode.Open, FileAccess.Read))
            {
                var inputMessage = MessageHelper.CreateFromStream(stream);
                if (!string.IsNullOrEmpty(_sourceEncoding))
                {
                    inputMessage.BodyPart.Charset = _sourceEncoding;
                }

                // Load context file, add to message context.
                if (!string.IsNullOrEmpty(_inputContextFile) && new FileInfo(_inputContextFile).Exists)
                {
                    var mi = MessageInfo.Deserialize(_inputContextFile);
                    mi.MergeIntoMessage(inputMessage);
                }

                mc = pipelineWrapper.Execute(inputMessage);
            }

            for (var count = 0; count < mc.Count; count++)
            {
                string destination = null;
                if (!string.IsNullOrEmpty(_destinationFileFormat))
                {
                    destination = string.Format(_destinationFileFormat, count);
                    if (!string.IsNullOrEmpty(_destinationDir))
                    {
                        destination = Path.Combine(_destinationDir, destination);
                    }

                    PersistMessageHelper.PersistMessage(mc[count], destination);
                }

                if (!string.IsNullOrEmpty(_outputContextFileFormat))
                {
                    var contextDestination = string.Format(_outputContextFileFormat, count);
                    if (!string.IsNullOrEmpty(_destinationDir))
                    {
                        contextDestination = Path.Combine(_destinationDir, contextDestination);
                    }

                    var mi = BizTalkMessageInfoFactory.CreateMessageInfo(mc[count], destination);
                    MessageInfo.Serialize(mi, contextDestination);
                }
            }
        }

        /// <summary>
        /// TestStepBase.Validate() implementation
        /// </summary>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public override void Validate(Context context)
        {
            ArgumentValidation.CheckForEmptyString(_pipelineTypeName, "pipelineTypeName");
            // pipelineAssemblyPath - optional

            _source = context.SubstituteWildCards(_source);
            if (!new FileInfo(_source).Exists)
            {
                throw new ArgumentException("Source file does not exist.", _source);
            }

            // destinationDir - optional
            if (!string.IsNullOrEmpty(_destinationDir))
            {
                _destinationDir = context.SubstituteWildCards(_destinationDir);
            }
        }
    }
}