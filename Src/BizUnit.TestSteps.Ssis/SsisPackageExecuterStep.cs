//---------------------------------------------------------------------
// File: SsisPackageExecuterStep.cs
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

using BizUnit.Xaml;
using System;
using System.Collections.ObjectModel;

namespace BizUnit.TestSteps.Ssis
{
    /// <summary>
    /// The SsisPackageExecuterStep test step executes an SSIS package. 
    /// </summary>
    public class SsisPackageExecuterStep : TestStepBase
    {
        ///<summary>
        /// The file path of the SSIS package to execute
        ///</summary>
        public string PackageLocation { get; set; }

        ///<summary>
        /// The package variables 
        ///</summary>
        public Collection<PackageVariable> Variables { get; set; }

        ///<summary>
        /// Constructor
        ///</summary>
        public SsisPackageExecuterStep()
        {
            Variables = new Collection<PackageVariable>();
        }

        ///<summary>
        /// TestStepBase.Execute() implementation
        ///</summary>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        ///<exception cref="ApplicationException"></exception>
        public override void Execute(Context context)
        {
            context.LogInfo("Executing SSIS package: {0}", PackageLocation);

            var ssisRuntime = new Microsoft.SqlServer.Dts.Runtime.Application();
            using (var package = ssisRuntime.LoadPackage(PackageLocation, null))
            {
                var variables = package.Variables;

                foreach (var variable in Variables)
                {
                    context.LogInfo("Adding variable: {0}, value: {1}", variable.Name, variable.Value);
                    variables[variable.Name].Value = variable.Value;
                }

                var result = package.Execute();

                var errors = package.Errors;
                foreach (var error in errors)
                {
                    context.LogError(error.Description);
                }

                if (Microsoft.SqlServer.Dts.Runtime.DTSExecResult.Success != result || 0 < errors.Count)
                {
                    throw new ApplicationException("Package execution failed.");
                }

                context.LogInfo("Package executed successfully.");
            }
        }

        ///<summary>
        /// Validates the test step is correctly configured prior to execution.
        ///</summary>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        ///<exception cref="StepValidationException"></exception>
        public override void Validate(Context context)
        {
            if(string.IsNullOrEmpty(PackageLocation))
            {
                throw new StepValidationException("PackageLocation may not be null or empty", this);
            }
        }
    }
}
