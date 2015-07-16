//---------------------------------------------------------------------
// File: ITestStepOM.cs
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

namespace BizUnit.BizUnitOM
{
    /// <summary>
    /// The ITestStepOM interface is implemented by test steps which wish
    /// to be driven via the BizUnit object model.
    /// </summary>
    /// 
    /// <remarks>
    /// The following example demonstrates how to implement the ITestStepOM interface:
    /// 
    /// <code escaped="true">
    ///	public class DelayStep : ITestStepOM
    ///	{
    ///    private int timeOut;
    ///
	///    public int TimeOut
	///    {
	///        set { timeOut = value; }
	///    }
    ///
    /// 	public void Execute(XmlNode testConfig, Context context)
    ///		{
	///		   timeOut = context.ReadConfigAsInt32( testConfig, "Delay" );
    ///
    ///        Execute(context);
	///	    }
    ///
    ///		public void Execute(XmlNode testConfig, Context context)
    ///		{
    ///        context.LogInfo("About to wait for {0} milli seconds...", timeOut.ToString());
    ///
    ///        Thread.Sleep(timeOut);
    ///
    ///        context.LogInfo("A delay of {0} milli second has successfully completed.", timeOut.ToString());
    ///		}
    /// 
    ///     public void Validate()
	///     {
	///        // timeOut - no validation required
	///     }
    ///
    ///	}
    ///	</code>
    ///	
    ///	</remarks>
    [Obsolete("ITestStepOM has been deprecated. Please investigate the use of BizUnit.Xaml.TestCase.")]
    public interface ITestStepOM : ITestStep
    {
        /// <summary>
        /// Called by the BizUnit framework to execute the test step
        /// </summary>
        /// 
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        void Execute(Context context);

        /// <summary>
        /// Called by the BizUnit framework to validate that the test step has been correctly configured
        /// </summary>
        void Validate(Context context);
    }
}
