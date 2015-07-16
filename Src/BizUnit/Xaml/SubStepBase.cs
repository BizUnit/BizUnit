//---------------------------------------------------------------------
// File: SubStepBase.cs
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

using System.IO;

namespace BizUnit.Xaml
{
    ///<summary>
    /// The base class for all test sub-steps. 
    /// Sub-steps may be used for validation of data, loading data into 
    /// the BizUnit context or other purposes that are appropriate.
    /// For example an XmlValidationStep could be a sub-step of a 
    /// FileReaderStep, the FileReaderStep would be responsible for 
    /// reading a file and then validating its content using the 
    /// XmlValidationStep thereby ensuring the content is valid Xml. By 
    /// seperating out the concerns of the test step from the sub-step, a 
    /// given test step may be used in a greater number of scenarios. For 
    /// example, in the scenario above, the same FileReaderStep could also be 
    /// used with a RegExValidationStep, thereby enabling the FileReaderStep
    /// to process Xml and FlatFile format data.
    ///</summary>
    public abstract class SubStepBase
    {
        ///<summary>
        /// Executes the logic in the sub-step
        ///</summary>
        ///<param name="data">The data to be processed by the sub-step</param>
        ///<param name="context">The test context being used in the current TestCase</param>
        ///<returns>The data stream ready to be consumbed, perhaps by another sub-step.</returns>
        public abstract Stream Execute(Stream data, Context context);

        ///<summary>
        /// Validation logic in the sub-step, will be called prior to the TestCase being executed
        ///</summary>
        ///<param name="context">The test context being used in the current TestCase</param>
        public abstract void Validate(Context context);
    }
}
