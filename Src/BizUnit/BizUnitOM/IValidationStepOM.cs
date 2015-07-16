//---------------------------------------------------------------------
// File: IValidationStepOM.cs
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

namespace BizUnit.BizUnitOM
{
    /// <summary>
    /// The IValidationStepOM interface is implemented by validation sub-steps 
    /// that perform validation against data. This interface should be implemented 
    /// by validation steps which wish to be driven by the BizUnit object 
    /// model.
    /// </summary>
    /// 
    /// <remarks>
    /// The following example demonstrates how to implement the IValidationStepOM interface:
    /// 
    /// <code escaped="true">
    /// public class RegExValidationStep : IValidationStepOM
    /// {
    ///     private IList&gt;string&lt; validationRegExs = new List&gt;string&lt;();
    /// 
    ///     public IList&gt;string&lt; ValidationRegEx
    ///     {
    ///         set { validationRegExs = value; }
    ///     }
    /// 
    ///     public void ExecuteValidation(Stream data, XmlNode validatorConfig, Context context)
    ///     {
    ///         XmlNodeList validationNodes = validatorConfig.SelectNodes("ValidationRegEx");
    /// 
    ///         foreach (XmlNode validationNode in validationNodes)
    ///         {
    ///             validationRegExs.Add(validationNode.InnerText);
    ///         }
    /// 
    ///         ExecuteValidation(data, context);
    ///     }
    /// 
    ///     public void ExecuteValidation(Stream data, Context context)
    ///     {
    ///         StreamReader sr = new StreamReader(data);
    ///         string strData = sr.ReadToEnd();
    /// 
    ///         foreach (string validationRegEx in validationRegExs)
    ///         {
    ///             Match match = Regex.Match(strData, validationRegEx);
    /// 
    ///             if (match.Success)
    ///             {
    ///                 context.LogInfo("Regex validation succeeded for pattern \"{0}\".", validationRegEx);
    ///             }
    ///             else { throw new Exception(String.Format("Regex validation failed for pattern \"{0}\".", validationRegEx)); }
    ///         }
    ///     }
    /// 
    ///     public void Validate()
    ///     {
    ///         // validationRegEx - no validation to do
    ///     }
    /// }
    ///	</code>
    ///	
    ///	</remarks>
    [Obsolete("IValidationStepOM has been deprecated. Please investigate the use of BizUnit.Xaml.TestCase.")]
    public interface IValidationStepOM : IValidationStep
    {
        /// <summary>
        /// Called by the BizUnit framework to execute the validation test step
        /// </summary>
        /// 
        /// <param name='data'>The stream cintaining the data to be validated.</param>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        void ExecuteValidation(Stream data, Context context);

        /// <summary>
        /// Called by the BizUnit framework to validate that the validation step has been correctly configured
        /// </summary>
        void Validate(Context context);
    }
}
