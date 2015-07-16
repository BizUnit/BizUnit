//---------------------------------------------------------------------
// File: ValidationStepBuilder.cs
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
using System.Xml;
using BizUnit.Common;

namespace BizUnit.BizUnitOM
{
    /// <summary>
    /// The ValidationStepBuilder abstracts a validation sub step, it is responsible for 
    /// creating and configuring a validation sub step that implements IValidationStepOM.
    /// </summary>
    [Obsolete("ValidationStepBuilder has been deprecated. Please investigate the use of BizUnit.Xaml.TestCase.")]
    public class ValidationStepBuilder : TestStepBuilderBase
    {
        /// <summary>
        /// ValidationStepBuilder constructor.
        /// </summary>
        /// 
        /// <param name='config'>The Xml configuration for a test step that 
        /// implements the ITestStep interface.</param>
        public ValidationStepBuilder(XmlNode config)
            : base(config) {}

        /// <summary>
        /// ValidationStepBuilder constructor.
        /// </summary>
        /// 
        /// <param name='typeName'>The type name of the test step to be created by the builder.</param>
        /// <param name='assemblyPath'>The assembly path name of the test step to be created by the builder.</param>
        public ValidationStepBuilder(string typeName, string assemblyPath)
            : base(typeName, assemblyPath)
        {
            ArgumentValidation.CheckForEmptyString(typeName, "typeName");
            // assemblyPath - optional

            ValidationStep = RawTestStep as IValidationStepOM;
            if (null == ValidationStep)
            {
                throw new ArgumentException(string.Format("The validation step type: {0}, created is invalid: IValidationStepOM is not implemented", typeName));
            }
        }

        public IValidationStepOM ValidationStep { get; private set; }
    }
}
