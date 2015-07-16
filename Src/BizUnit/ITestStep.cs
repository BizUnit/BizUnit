//---------------------------------------------------------------------
// File: ITestStep.cs
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

using System.Xml;

namespace BizUnit
{
    /// <summary>
	/// The ITestStep interface is implemented by test steps.
	/// </summary>
	/// 
	/// <remarks>
    /// The following example demonstrates how to implement the ITestStep interface:
	/// 
	/// <code escaped="true">
	///	public class DelayStep : ITestStep
	///	{
	///		public void Execute(XmlNode testConfig, Context context)
	///		{
	///			int timeOut = context.ReadConfigAsInt32( testConfig, "Delay" );
	///
	///			context.LogInfo("About to wait for {0} milli seconds...", timeOut.ToString());
	///
	///			Thread.Sleep(timeOut);
	///
	///			context.LogInfo("A delay of {0} milliseconds has successfully completed.", timeOut.ToString());
	///		}
	///	}
	///	</code>
	///	
	///	</remarks>

	public interface ITestStep
	{
		/// <summary>
		/// Called by the BizUnit framework to execute the test step
		/// </summary>
		/// 
		/// <param name='testConfig'>The Xml fragment containing the configuration for the test step</param>
		/// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
		void Execute(XmlNode testConfig, Context context);
	}
}
