//---------------------------------------------------------------------
// File: CrossReferenceSeedLoadStep.cs
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


// NEEDS MAJOR REWORK

using System;
using System.Xml;
using System.Diagnostics;
using Microsoft.Win32;
using BizUnit.Core.TestBuilder;
using BizUnit.Core.Common;

namespace BizUnit.TestSteps.BizTalk.CrossReference
{
    /// <summary>
    /// The CrossReferenceSeedLoadStep 
    /// </summary>
    /// 
    /// <remarks>
    /// The following shows an example of the Xml representation of this test step.
    /// 
    /// <code escaped="true">
    ///	<TestStep assemblyPath="" typeName="BizUnit.BizTalkSteps.CrossReferenceSeedLoadStep, BizUnit.BizTalkSteps, Version=3.1.0.0, Culture=neutral, PublicKeyToken=7eb7d82981ae5162">
    ///		<SeedDataSetupFilePath>c:\SeedData\Setup.xml</SeedDataSetupFilePath> <!-- path relative to the testframework assembly setup file -->
    ///	</TestStep>
    /// </code>
    /// 
    ///	<list type="table">
    ///		<listheader>
    ///			<term>Tag</term>
    ///			<description>Description</description>
    ///		</listheader>
    ///		<item>
    ///			<term>SeedDataSetupFilePath</term>
    ///			<description>The FILE path to the setup FILE</description>
    ///		</item>
    ///	</list>
    ///	</remarks>

    public class CrossReferenceSeedLoadStep : TestStepBase
    {
        /// <summary>
        /// Description
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// The FILE path to the setup FILE
        /// </summary>
        public string SeedDataSetupFilePath { get; set; }


        /// <summary>
        /// ITestStep.Execute() implementation
        /// </summary>
        /// <param name='testConfig'>The Xml fragment containing the configuration for this test step</param>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public override void Execute(Context context)
        {
            RegistryKey rk = null;
            System.Diagnostics.Process BTSXref = null;
            try
            {
                rk = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\BizTalk Server\\3.0\\");
                string btsxrefimportPath = rk.GetValue("InstallPath") + "btsxrefimport.exe";

                string seedSetupPath = this.SeedDataSetupFilePath;

                ProcessStartInfo startinfo = new ProcessStartInfo(btsxrefimportPath, " -file=\"" + seedSetupPath + "\"");
                startinfo.CreateNoWindow = true;
                startinfo.RedirectStandardError = true;
                startinfo.RedirectStandardOutput = true;
                startinfo.UseShellExecute = false; //set to false to redirect error

                BTSXref = System.Diagnostics.Process.Start(startinfo);
                BTSXref.WaitForExit();
                string processOutput = BTSXref.StandardOutput.ReadToEnd();

                if (processOutput.IndexOf("error") != -1)
                {
                    throw new ApplicationException("XRef seed data load failed. The result of the import was - \n" + processOutput);
                }

                context.LogInfo(processOutput);

            }
            finally
            {
                if (rk != null)
                {
                    rk.Close();
                }

                if (BTSXref != null)
                {
                    BTSXref.Dispose();
                }
            }
        }

        public override void Validate(Context context)
        {
            ArgumentValidation.CheckForEmptyString(this.SeedDataSetupFilePath, "SeedDataSetupFilePath");
        }
    }
}





