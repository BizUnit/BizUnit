//---------------------------------------------------------------------
// File: DelayStep.cs
// 
// Summary: 
//
//---------------------------------------------------------------------
// Copyright (c) 2004-2010, Kevin B. Smith. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------

using BizUnit.Xaml;

namespace BizUnit.TestSteps.Time
{
    using System.Threading;

	/// <summary>
	/// The DelayStep is used to perform a delay/sleep.
	/// </summary>

    public class Delay : TestStepBase
	{
	    private int _timeOut;

        public int DelayMilliSeconds
	    {
	        set { _timeOut = value; }
            get { return _timeOut; }
	    }

        /// <summary>
        /// TestStepBase.Execute() implementation
        /// </summary>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public override void Execute(Context context)
	    {
            context.LogInfo("About to wait for {0} milli seconds...", _timeOut.ToString());

            Thread.Sleep(_timeOut);

            context.LogInfo("A delay of {0} milli second has successfully completed.", _timeOut.ToString());
        }

        public override void Validate(Context context)
	    {
	        // _timeOut - no validation required
	    }
	}
}
