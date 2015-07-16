//---------------------------------------------------------------------
// File: OrchestrationConductorStep.cs
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
using System.Management;
using BizUnit.Xaml;

namespace BizUnit.TestSteps.BizTalk.Orchestration
{
	/// <summary>
	/// The OrchestrationConductorStep may be used to stop/start an orchestration.
	/// </summary>
	/// 
	/// <remarks>
	/// The following shows an example of the Xml representation of this test step.
	/// 
	/// <code escaped="true">
    /// <TestStep assemblyPath="" typeName="BizUnit.BizTalkSteps.OrchestrationConductorStep, BizUnit.BizTalkSteps, Version=3.1.0.0, Culture=neutral, PublicKeyToken=7eb7d82981ae5162">
	///		<DelayForCompletion>5</DelayForCompletion> <!-- Optional, seconds to delay for this step to complete -->
	///		<AssemblyName>BizUnitTest.Process</AssemblyName>
	///		<OrchestrationName>BizUnitTest.Process.SubmitToLedger</OrchestrationName>
	///		<Action>Start</Action>
	/// </TestStep>
	/// </code>
	///	<list type="table">
	///		<listheader>
	///			<term>Tag</term>
	///			<description>Description</description>
	///		</listheader>
	///		<item>
	///			<term>DelayForCompletion</term>
	///			<description>The delay before executing the step <para>(optional)</para></description>
	///		</item>
	///		<item>
	///			<term>AssemblyName</term>
	///			<description>The name of the assembly containing the orchestration, e.g. BizUnitTest.Process</description>
	///		</item>
	///		<item>
	///			<term>OrchestrationName</term>
	///			<description>The name of the orchestration to start/stop</description>
	///		</item>
	///		<item>
	///			<term>Action</term>
	///			<description>Start|Stop</description>
	///		</item>
	///	</list>
	///	</remarks>	

    public class OrchestrationConductorStep : TestStepBase
	{
        public string AssemblyName { get; set; }
        public string OrchestrationName { get; set; }
        public OrchestrationAction Action { get; set; }
        public int DelayForCompletion { get; set; }

        public enum OrchestrationAction 
		{
			Start,
			Stop,
		}

        /// <summary>
        /// ITestStep.Execute() implementation
        /// </summary>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public override void Execute(Context context)
        {
            try
            {
                if (Action == OrchestrationAction.Start)
                {
                    Start(AssemblyName, OrchestrationName, context);
                }
                else
                {
                    Stop(AssemblyName, OrchestrationName, context);
                }

                // Delay if necessary to allow the orchestration to start/stop
                if (DelayForCompletion > 0)
                {
                    context.LogInfo("Waiting for {0} seconds before recommencing testing.", DelayForCompletion);
                    System.Threading.Thread.Sleep(DelayForCompletion * 1000);
                }
            }
            catch (System.Runtime.InteropServices.COMException e)
            {
                context.LogException(e);
                throw;
            }
        }

	    public override void Validate(Context context)
	    {
	        ;
	    }

	    private static void Start(string assemblyName, string orchestrationName, Context context)
		{
			var started = false;
			var wmiQuery = 
				String.Format("Select * from MSBTS_Orchestration where Name=\"{0}\" and AssemblyName=\"{1}\"", orchestrationName, assemblyName);

			var enumOptions = new EnumerationOptions {ReturnImmediately = false};

	        var OrchestrationSearcher = new ManagementObjectSearcher("root\\MicrosoftBizTalkServer",	wmiQuery, enumOptions);							

			foreach( ManagementObject OrchestrationInstance in OrchestrationSearcher.Get())
			{
				context.LogInfo("Starting Orchestration: {0}", orchestrationName);
				OrchestrationInstance.InvokeMethod("Start", new object[] {2,2});

				context.LogInfo("Orchestration: {0} was successfully started", orchestrationName);
				started = true;
			}				
			
			if ( !started )
			{
				throw new ApplicationException(string.Format("Failed to start the orchestration: \"{0}\"", orchestrationName));
			}							
		}

		private static void Stop( string assemblyName, string orchestrationName, Context context )
		{
			bool stopped = false;
			string wmiQuery = 
				String.Format( "Select * from MSBTS_Orchestration where Name=\"{0}\" and AssemblyName=\"{1}\"", orchestrationName, assemblyName);

			var enumOptions = new EnumerationOptions {ReturnImmediately = false};

		    var OrchestrationSearcher = new ManagementObjectSearcher("root\\MicrosoftBizTalkServer",	wmiQuery, enumOptions);							

			foreach( ManagementObject OrchestrationInstance in OrchestrationSearcher.Get())
			{
				OrchestrationInstance.InvokeMethod("Stop", new object[] {2,2});

				context.LogInfo("Orchestration: {0} was successfully stopped", orchestrationName);
				stopped = true;
			}	
			
			if ( !stopped )
			{
				throw new ApplicationException(string.Format("Failed to stop the orchestration: \"{0}\"", orchestrationName));
			}																									
		}
	}
}