//---------------------------------------------------------------------
// File: ContextManipulatorStep.cs
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
using System.Xml;

namespace BizUnit.CoreSteps.TestSteps
{
	/// <summary>
	/// The ContextManipulator is used to manipulate BizUnit context fields. 
	/// It maybe used to create a new field from one or more existing fields.
	/// </summary>
	/// 
	/// <remarks>
	/// The following shows an example of the Xml representation of this test step.
	/// 
	/// <code escaped="true">
    /// <TestStep assemblyPath="" typeName="BizUnit.ContextManipulatorStep">
	///		<ContextItem contextKey="NewItemToWrite">
	///			<ItemTest>holdEvent=</ItemTest>
	///			<ItemTest takeFromCtx="HoldEvent"></ItemTest>
	///			<ItemTest>actionId=</ItemTest>
	///			<ItemTest takeFromCtx="ActionId"></ItemTest>
	///			<ItemTest>actionType=</ItemTest>
	///			<ItemTest takeFromCtx="ActionType"></ItemTest>
	///		</ContextItem>
	/// </TestStep>
	///	</code>
	///	
	///	The ContextManipulator builds a new context item by appeanding the values of multiple context items
	///	<list type="table">
	///		<listheader>
	///			<term>Tag</term>
	///			<description>Description</description>
	///		</listheader>
	///		<item>
	///			<term>ContextItem</term>
	///			<description>The key for the new context value that will be constructed</description>
	///		</item>
	///		<item>
	///			<term>ItemTest</term>
	///			<description>The item to append, note that a value from the context maybe used if the attribute takeFromCtx is used <para>(one or more)</para></description>
	///		</item>
	///	</list>
	///	</remarks>
    [Obsolete("ContextManipulatorStep has been deprecated. Investigate the BizUnit.TestSteps namespace.")]
	public class ContextManipulatorStep : ITestStep
	{
		/// <summary>
		/// ITestStep.Execute() implementation
		/// </summary>
		/// <param name='testConfig'>The Xml fragment containing the configuration for this test step</param>
		/// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
		public void Execute(XmlNode testConfig, Context context)
		{
			XmlNodeList ctxItems = testConfig.SelectNodes( "ContextItem" );

			foreach (XmlNode ctxItem in ctxItems)
			{
				string newCtxNode = ctxItem.SelectSingleNode( "@contextKey" ).Value;
				string newValue = "";

                XmlNodeList items = ctxItem.SelectNodes("ItemTest");

				foreach (XmlNode item in items)
				{
                    newValue += context.ReadConfigAsString(item, ".");
				}

				context.Add( newCtxNode, newValue );
			}
		}
	}
}
