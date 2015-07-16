//---------------------------------------------------------------------
// File: DelayStep.cs
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

namespace BizUnit.CoreSteps.TestSteps
{
	using System.Xml;
	using System.Threading;
    using BizUnitOM;

	/// <summary>
	/// The DelayStep is used to perform a delay/sleep.
	/// </summary>
	/// 
	/// <remarks>
	/// The following shows an example of the Xml representation of this test step.
	/// 
	/// <code escaped="true">
	///	<TestStep assemblyPath="" typeName="BizUnit.DelayStep">
	///		<Delay>1000</Delay>
	///	</TestStep>
	///	</code>
	///	
	///	<list type="table">
	///		<listheader>
	///			<term>Tag</term>
	///			<description>Description</description>
	///		</listheader>
	///		<item>
	///			<term>Delay</term>
	///			<description>The length of the delay in miliseconds</description>
	///		</item>
	///	</list>
	///	</remarks>
    [Obsolete("DelayStep has been deprecated. Investigate the BizUnit.TestSteps namespace.")]
	public class DelayStep : ITestStepOM
	{
	    private int _timeOut;

	    public int Delay
	    {
	        set { _timeOut = value; }
	    }

		/// <summary>
		/// ITestStep.Execute() implementation
		/// </summary>
		/// <param name='testConfig'>The Xml fragment containing the configuration for this test step</param>
		/// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
		public void Execute(XmlNode testConfig, Context context)
		{
			_timeOut = context.ReadConfigAsInt32( testConfig, "Delay" );

            Execute(context);
		}

        /// <summary>
        /// ITestStep.Execute() implementation
        /// </summary>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public void Execute(Context context)
	    {
            context.LogInfo("About to wait for {0} milli seconds...", _timeOut.ToString());

            Thread.Sleep(_timeOut);

            context.LogInfo("A delay of {0} milli second has successfully completed.", _timeOut.ToString());
        }

        public void Validate(Context context)
	    {
	        // timeOut - no validation required
	    }
	}
}
