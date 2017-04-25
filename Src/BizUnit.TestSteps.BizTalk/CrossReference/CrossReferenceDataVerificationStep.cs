//---------------------------------------------------------------------
// File: CrossReferenceDataVerificationStep.cs
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

namespace BizUnit.TestSteps.BizTalk.CrossReference
{
    using System;
    using System.Xml;
    using Microsoft.BizTalk.CrossReferencing;
    using BizUnit.Core.TestBuilder;
    using System.Collections.ObjectModel;

    /// <summary>
    /// The CrossReferenceDataVerificationStep reads email from a POP3 server.
    /// </summary>
    /// 
    /// <remarks>
    /// The following shows an example of the Xml representation of this test step.
    /// 
    /// <code escaped="true">
    ///	<TestStep assemblyPath="" typeName="BizUnit.BizTalkSteps.CrossReferenceDataVerificationStep, BizUnit.BizTalkSteps, Version=3.1.0.0, Culture=neutral, PublicKeyToken=7eb7d82981ae5162">
    ///		<VerifyId appInstance="application1" idXRef="Customer">25</VerifyId> 
    ///		<VerifyValue appType="application2" idXRef="Customer">12LK</VerifyValue> 
    ///	</TestStep>
    /// </code>
    /// 
    ///	<list type="table">
    ///		<listheader>
    ///			<term>Tag</term>
    ///			<description>Description</description>
    ///		</listheader>
    ///		<item>
    ///			<term>VerifyId</term>
    ///			<description>The Id to varify</description>
    ///		</item>
    ///		<item>
    ///			<term>VerifyValue</term>
    ///			<description>The expected value</description>
    ///		</item>
    ///	</list>
    ///	</remarks>
    public class CrossReferenceDataVerificationStep : TestStepBase
    {

        public string Tag { get; set; }


        public VerificationDefinition Verification { get; set; }


        /// <summary>
        /// Execute() implementation
        /// </summary>
        /// <param name='testConfig'>The Xml fragment containing the configuration for this test step</param>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public override void Execute(Context context)
        {
            //XmlNodeList nodelist = testConfig.SelectNodes("VerifyId");


            foreach (var verificationId in this.Verification.Ids)
            {
                var appInstance = verificationId.App;
                var entity = verificationId.IdXRef;
                var valueToVerify = verificationId.Val;


                string commonId = CrossReferencing.GetCommonID(entity, appInstance, valueToVerify);

                if (commonId == null || commonId == string.Empty)
                {
                    throw new ApplicationException("AppId " + valueToVerify + " not found");
                }

                context.LogInfo("IdXRef = " + entity + ". AppInstance = " + appInstance + ". AppId = " + valueToVerify + ". CommonId = " + commonId);

            }


            foreach (var verificationValue in this.Verification.Values)
            {
                string valueToVerify = verificationValue.Val;
                string appType = verificationValue.App;
                string entity = verificationValue.IdXRef;

                string commonValue = CrossReferencing.GetCommonValue(entity, appType, valueToVerify);

                if (commonValue == null || commonValue == string.Empty)
                {
                    throw new ApplicationException("AppValue " + valueToVerify + " not found");
                }

                context.LogInfo("IdXRef = " + entity + ". AppType = " + appType + ". AppValue = " + valueToVerify + ". CommonValue = " + commonValue);

            }

            context.LogInfo("Cross Reference Id and Value verification is complete");
        }

        public override void Validate(Context context)
        {
            throw new NotImplementedException();
        }
    }
}
