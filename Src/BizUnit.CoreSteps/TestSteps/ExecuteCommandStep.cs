//---------------------------------------------------------------------
// File: ExecuteCommandStep.cs
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

namespace BizUnit.CoreSteps.TestSteps
{
	using System;
	using System.Xml;
	using System.Diagnostics;
    using BizUnitOM;

	/// <summary>
	/// The ExecuteCommandStep executes a program from the command line, command line arguments may be supplied also.
	/// </summary>
	/// 
	/// <remarks>
	/// The following shows an example of the Xml representation of this test step.
	/// 
	/// <code escaped="true">
	/// <TestStep assemblyPath="" typeName="BizUnit.ExecuteCommandStep">
	///		<ProcessName>processName</ProcessName>
	///		<ProcessParams>-a32 -bFooBar</ProcessParams>
	///		<WorkingDirectory>..\..\setup</WorkingDirectory>
	/// </TestStep>
	///	</code>
	///	
	///	<list type="table">
	///		<listheader>
	///			<term>Tag</term>
	///			<description>Description</description>
	///		</listheader>
	///		<item>
	///			<term>ProcessName</term>
	///			<description>The name of the program to execute</description>
	///		</item>
	///		<item>
	///			<term>ProcessParams</term>
	///			<description>The command line paramters</description>
	///		</item>
	///		<item>
	///			<term>WorkingDirectory</term>
	///			<description>The working directory to run the program from</description>
	///		</item>
	///	</list>
	///	</remarks>
    [Obsolete("ExecuteCommandStep has been deprecated. Investigate the BizUnit.TestSteps namespace.")]
	public class ExecuteCommandStep : ITestStepOM
	{
	    private string _processName;
	    private string _processParams;
	    private string _workingDirectory;

	    public string ProcessName
	    {
	        set
            { 
                _processName = value;
            }
	    }

	    public string ProcessParams
	    {
	        set
	        {
	            _processParams = value;
	        }
	    }

	    public string WorkingDirectory
	    {
	        set
	        {
	            _workingDirectory = value;
	        }
	    }

        /// <summary>
		/// ITestStep.Execute() implementation
		/// </summary>
		/// <param name='testConfig'>The Xml fragment containing the configuration for this test step</param>
		/// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
		public void Execute(XmlNode testConfig, Context context)
		{
			_processName = context.ReadConfigAsString( testConfig, "ProcessName" );
			_processParams = context.ReadConfigAsString( testConfig, "ProcessParams");
			_workingDirectory = context.ReadConfigAsString( testConfig, "WorkingDirectory" );

            Execute(context);
		}

        /// <summary>
        /// ITestStep.Execute() implementation
        /// </summary>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public void Execute(Context context)
	    {
            context.LogInfo("ExecuteCommandStep about to execute the command: {0} params: {1}, working directory: {2}", _processName, _processParams, _workingDirectory);

            var process = new Process
                              {
                                  StartInfo =
                                      {
                                          UseShellExecute = false,
                                          RedirectStandardOutput = true,
                                          CreateNoWindow = true,
                                          Arguments = _processParams,
                                          FileName = _processName,
                                          WorkingDirectory = _workingDirectory
                                      }
                              };

            process.Start();

            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            var exitCode = process.ExitCode;

            if (0 != exitCode)
            {
                throw new ApplicationException(string.Format("ExecuteCommandStep received an exit code: {0} while executing process {1} {2}\n\nOutput: {3}", exitCode, _processName, _processParams, output));
            }

            context.LogInfo("ExecuteCommandStep {0} output:\n{1}", _processName, output);
        }

        /// <summary>
        /// ITestStep.Validate() implementation
        /// </summary>
        public void Validate(Context context)
	    {
            if (string.IsNullOrEmpty(_processName))
            {
                throw new ArgumentNullException("ProcessName is either null or of zero length");
            }
            _processName = context.SubstituteWildCards(_processName);

            if (string.IsNullOrEmpty(_processParams))
            {
                throw new ArgumentNullException("ProcessParams is either null or of zero length");
            }
            _processParams = context.SubstituteWildCards(_processParams);

            if (string.IsNullOrEmpty(_workingDirectory))
            {
                throw new ArgumentNullException("WorkingDirectory is either null or of zero length");
            }
            _workingDirectory = context.SubstituteWildCards(_workingDirectory);
        }
	}
}
