//---------------------------------------------------------------------
// File: Deploy.cs
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

using System.Diagnostics;
using BizUnit.Xaml;

namespace BizUnit.TestSteps.BizTalk.Bam
{
	/// <summary>
    /// The Deploy deloys a BAM definition.
	/// </summary>
    public class Deploy : TestStepBase
	{
        ///<summary>
        ///</summary>
        public enum BamDeploymentAction 
		{
			Deploy,
			Undeploy
		}

	    ///<summary>
        /// The Tracking directory for BizTalk Server.
	    ///</summary>
	    public string TrackingFolderPath { get; set; }

        ///<summary>
        /// The path to the BAM definition file
        ///</summary>
        public string BamDefinitionXmlFilePath { get; set;}

	    ///<summary>
        /// The delay for this step to complete in seconds, -1 to delay until BAM deployment has finished
	    ///</summary>
	    public int DelayForCompletion { get; set; }

        ///<summary>
        /// Deploy|Undeploy
        ///</summary>
        public BamDeploymentAction Action { get; set; }

        /// <summary>
        /// TestStepBase.Execute() implementation
		/// </summary>
		/// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
		public override void Execute(Context context)
		{
			var sb = new System.Text.StringBuilder();
			sb.Append('"');
			sb.Append(BamDefinitionXmlFilePath);
			sb.Append('"');
			
			var bamProcess = new Process
			                     {
			                         StartInfo = {WorkingDirectory = TrackingFolderPath, FileName = "bm.exe"}
			                     };

            if (Action == BamDeploymentAction.Deploy)
            {
                context.LogInfo("Deploying BAM definition file {0}", BamDefinitionXmlFilePath);
                bamProcess.StartInfo.Arguments = "deploy-all -DefinitionFile:" + sb.ToString();
            }
            else
            {
                context.LogInfo("Undeploying BAM definition file {0}", BamDefinitionXmlFilePath);
                bamProcess.StartInfo.Arguments = "remove-all -DefinitionFile:" + sb.ToString();
            }

			bamProcess.StartInfo.UseShellExecute = true;

			bamProcess.Start();

            // If a positive delay period is specfied then wait, if -1 wait for the BAM deployment process to complete
            if (DelayForCompletion > 0)
            {
                // Wait for deployment process to complete
                if (DelayForCompletion == -1)
                {
                    context.LogInfo("Waiting for the BAM deployment process to complete before recommencing testing.", DelayForCompletion);
                    bamProcess.WaitForExit();
                }
                else
                {
                    context.LogInfo("Waiting for {0} seconds before recommencing testing.", DelayForCompletion);
                    System.Threading.Thread.Sleep(DelayForCompletion * 1000);
                }
            }
        }

	    public override void Validate(Context context)
	    {
	        ;
	    }
	}
}
