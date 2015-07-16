//---------------------------------------------------------------------
// File: ExecuteSendPipelineStep.cs
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
    ///			<term>SourceDir</term>
    ///			<description>The directory path for the source input file(s) to execute in the pipeline</description>
    ///		</item>
    ///		<item>
    ///			<term>SearchPattern</term>
    ///			<description>The search pattern for the input files. E.g. input*.xml</description>
    ///		</item>
    ///		<item>
    ///			<term>Destination</term>
    ///			<description>The destination to write the pipeline output file</description>
    ///		</item>
    ///		<item>
    ///			<term>InputContextDir</term>
    ///			<description>The directory path for the source context file(s) (optional)</description>
    ///		</item>
    ///		<item>
    ///			<term>InputContextSearchPattern</term>
    ///			<description>The search pattern for the source context file(s) (optional)</description>
    ///		</item>
    ///		<item>
    ///			<term>OutputContextFile</term>
    ///			<description>The location to write the output message context file (optional)</description>
    ///		</item>
    ///		<item>
    ///			<term>SourceEncoding</term>
    ///			<description>The charset to be written on the pipeline input message (optional)</description>
    ///		</item>
    ///	</list>
    ///	</remarks>

    public class ExecuteSendPipelineStep : TestStepBase
    {
        private string _pipelineAssemblyPath;
        private string _pipelineTypeName;
        private Collection<DocSpecDefinition> _docSpecsRawList = new Collection<DocSpecDefinition>();
        private Type[] _docSpecs;
        private string _instanceConfigFile;
        private string _sourceDir;
        private string _sourceEncoding;
        private string _searchPattern;
        private string _destination;
        private string _inputContextDir;
        private string _inputContextSearchPattern;
        private string _outputContextFile;

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
        public string SourceDir
        {
            get { return _sourceDir; }
            set { _sourceDir = value; }
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
        /// Gets and sets the search pattern for the input file
        ///</summary>
        public string SearchPattern
        {
            get { return _searchPattern; }
            set { _searchPattern = value; }
        }

        ///<summary>
        /// Gets and sets the destination of the pipeline output
        ///</summary>
        public string Destination
        {
            get { return _destination; }
            set { _destination = value; }
        }

        ///<summary>
        /// Gets and sets the directory containing the message context file for the input message
        ///</summary>
        public string InputContextDir
        {
            get { return _inputContextDir; }
            set { _inputContextDir = value; }
        }

        ///<summary>
        /// Gets and sets the message context search pattern for the input message
        ///</summary>
        public string InputContextSearchPattern
        {
            get { return _inputContextSearchPattern; }
            set { _inputContextSearchPattern = value; }
        }

        ///<summary>
        /// Gets and sets the file name for the message context for the output message
        ///</summary>
        public string OutputContextFile
        {
            get { return _outputContextFile; }
            set { _outputContextFile = value; }
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

            var pipelineWrapper = PipelineFactory.CreateSendPipeline(pipelineType);
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

            var mc = new MessageCollection();

            FileInfo[] contexts = null;
            if (!string.IsNullOrEmpty(_inputContextDir) && !string.IsNullOrEmpty(_inputContextSearchPattern))
            {
                var cdi = new DirectoryInfo(_inputContextDir);
                contexts = cdi.GetFiles(_inputContextSearchPattern);
            }

            var di = new DirectoryInfo(_sourceDir);
            int index = 0;
            foreach (FileInfo fi in di.GetFiles(_searchPattern))
            {
                Stream stream = new FileStream(fi.FullName, FileMode.Open, FileAccess.Read);
                var inputMessage = MessageHelper.CreateFromStream(stream);
                if (!string.IsNullOrEmpty(_sourceEncoding))
                {
                    inputMessage.BodyPart.Charset = _sourceEncoding;
                }

                // Load context file, add to message context.
                if ((null != contexts) && (contexts.Length > index))
                {
                    string cf = contexts[index].FullName;
                    if (System.IO.File.Exists(cf))
                    {
                        MessageInfo mi = MessageInfo.Deserialize(cf);
                        mi.MergeIntoMessage(inputMessage);
                    }
                }

                mc.Add(inputMessage);
                index++;
            }

            var outputMsg = pipelineWrapper.Execute(mc);
            PersistMessageHelper.PersistMessage(outputMsg, _destination);

            if (!string.IsNullOrEmpty(_outputContextFile))
            {
                var omi = BizTalkMessageInfoFactory.CreateMessageInfo(outputMsg, _destination);
                MessageInfo.Serialize(omi, _outputContextFile);
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

            _destination = context.SubstituteWildCards(_destination);
        }
    }
}