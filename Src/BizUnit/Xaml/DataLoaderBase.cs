//---------------------------------------------------------------------
// File: DataLoaderBase.cs
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
    /// <summary>
    /// Base class for dataloaders such as the FileDataLoader. 
    /// Test steps may use data loaders in order to de-couple the loading of the
    /// data they accept from the undelying transport, this gives the test step 
    /// much grater flexibility around how its used.
    /// </summary>
    /// 
    /// <remarks>
    /// The following example demonstrates how to create and use a FileDataLoader:
    /// 
    /// <code escaped="true">
    /// // The WebServiceStep allows a DataLoader to be used to set the RequestBody,
    /// // this allows greater flexibility around how data is loaded by a test step.
    /// 
    /// var ws = new WebServiceStep();
    ///	ws.Action = "http://schemas.affinus.com/finservices/tradeflow";
    /// 
    /// // Create the dataloader and configure...
    /// FileDataLoader dl = new FileDataLoader();
    /// dl.FilePath = @"..\..\..\Tests\Affinus.TradeServices.BVTs\TradeFlow\BookTrade_RQ.xml";
    /// 
    /// // Assign the dataloader to the RequestBody
    /// ws.RequestBody = dl;
    /// ws.ServiceUrl = "http://localhost/TradeServices/TradeFlow.svc";
    /// ws.Username = @"domain\user";
    ///	</code>
    /// </remarks>
    public abstract class DataLoaderBase
    {
        ///<summary>
        /// Called to load the specified data
        ///</summary>
        ///<param name="context">The test context being used in the current TestCase</param>
        ///<returns>The data loaded</returns>
        public abstract Stream Load(Context context);

        ///<summary>
        /// Executes the test steps validation logic
        ///</summary>
        ///<param name="context">The test context being used in the current TestCase</param>
        public abstract void Validate(Context context);
    }
}
