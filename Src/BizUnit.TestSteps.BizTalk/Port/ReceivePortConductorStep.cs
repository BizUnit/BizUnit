//---------------------------------------------------------------------
// File: ReceivePortConductorStep.cs
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
using BizUnit.Common;
using BizUnit.Xaml;

namespace BizUnit.TestSteps.BizTalk.Port
{
	/// <summary>
	/// The ReceivePortConductorStep enables/dissables a recieve location.
	/// </summary>

	/// <remarks>
	/// The following shows an example of the Xml representation of this test step.
	/// 
	/// <code escaped="true">
    /// <TestStep assemblyPath="" typeName="BizUnit.BizTalkSteps.ReceivePortConductorStep, BizUnit.BizTalkSteps, Version=3.1.0.0, Culture=neutral, PublicKeyToken=7eb7d82981ae5162">
	///		<DelayForCompletion>5</DelayForCompletion> <!-- Optional, seconds to delay for this step to complete -->
	///		<ReceivePortName></ReceivePortName>
	///		<ReceiveLocationName></ReceiveLocationName>
	///		<Action>Enable</Action>
	/// </TestStep>
	/// </code>
	///	
	///	<list type="table">
	///		<listheader>
	///			<term>Tag</term>
	///			<description>Description</description>
	///		</listheader>
	///		<item>
	///			<term>DelayForCompletion</term>
	///			<description>The number of seconds to deplay for this step to complete</description>
	///		</item>
	///		<item>
	///			<term>ReceivePortName</term>
	///			<description>The name of the receive port to containing the receive location to enable/dissable</description>
	///		</item>
	///		<item>
	///			<term>ReceiveLocationName</term>
	///			<description>The name of the receive location to enable/dissable</description>
	///		</item>
	///		<item>
	///			<term>Action</term>
	///			<description>Enable|Disable</description>
	///		</item>
	///	</list>
	///	</remarks>

    public class ReceivePortConductorStep : TestStepBase
	{
		///<summary>
		/// The action to perform on the receive port
		///</summary>
		public enum ReceivePortAction 
		{
			///<summary>
			/// Action to perform
			///</summary>
			Enable,
			///<summary>
			/// Action to perform
			///</summary>
			Disable,
		}

	    ///<summary>
	    /// The name of the receive port to containing the receive location to enable/dissable
	    ///</summary>
	    public string ReceivePortName { get; set; }
	    ///<summary>
	    /// The name of the receive location to enable/dissable
	    ///</summary>
	    public string ReceiveLocationName { get; set; }
	    ///<summary>
	    /// The action to perform on the receive location: Enable|Disable
	    ///</summary>
	    public ReceivePortAction Action { get; set; }
	    ///<summary>
	    /// The number of seconds to deplay for this step to complete
	    ///</summary>
	    public int DelayForCompletion { get; set; }

		/// <summary>
		/// Execute() implementation
		/// </summary>
		/// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
		public override void Execute(Context context)
		{
			if ( Action == ReceivePortAction.Enable )
			{
				Enable( ReceivePortName, ReceiveLocationName, context );
			}
			else
			{
				Disable( ReceivePortName, ReceiveLocationName, context );
			}

			// Delay if necessary to allow the orchestration to start/stop
			if (DelayForCompletion > 0)
			{
				context.LogInfo("Waiting for {0} seconds before recommencing testing.", DelayForCompletion);
				System.Threading.Thread.Sleep(DelayForCompletion*1000);
			}
		}

	    ///<summary>
	    /// Validation method called prior to executing the test step
	    ///</summary>
	    ///<param name="context"></param>
	    public override void Validate(Context context)
	    {
	        ArgumentValidation.CheckForEmptyString(ReceivePortName, "ReceivePortName");
	        ArgumentValidation.CheckForEmptyString(ReceiveLocationName, "ReceiveLocationName");
	    }

	    private static void Enable( string receivePortName, string receiveLocationName, Context context )
		{
			string wmiQuery = String.Format(	"Select * from MSBTS_ReceiveLocation where ReceivePortName=\"{0}\" and Name=\"{1}\"",
				receivePortName, receiveLocationName);

			var enumOptions = new EnumerationOptions {ReturnImmediately = false};

		    var orchestrationSearcher = new ManagementObjectSearcher("root\\MicrosoftBizTalkServer",	wmiQuery, enumOptions);							

			foreach( ManagementObject orchestrationInstance in orchestrationSearcher.Get())
			{
				context.LogInfo("Enabling Receive Port {0} at location {1}", receivePortName, receiveLocationName );
				orchestrationInstance.InvokeMethod("Enable", new object[] {2,2});
			}																													
		}

		private static void Disable( string receivePortName, string receiveLocationName, Context context )
		{
			string wmiQuery = String.Format(	"Select * from MSBTS_ReceiveLocation where ReceivePortName=\"{0}\" and Name=\"{1}\"",
				receivePortName, receiveLocationName);

			var enumOptions = new EnumerationOptions {ReturnImmediately = false};

		    var orchestrationSearcher = new ManagementObjectSearcher("root\\MicrosoftBizTalkServer",	wmiQuery, enumOptions);							

			foreach( ManagementObject orchestrationInstance in orchestrationSearcher.Get())
			{
				context.LogInfo("Disabling Receive Port {0} at location {1}", receivePortName, receiveLocationName );
				orchestrationInstance.InvokeMethod("Disable", new object[] {2,2});
			}																													
		}
	}
}
