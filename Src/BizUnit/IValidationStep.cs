//---------------------------------------------------------------------
// File: IValidationStep.cs
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
using System.Xml;

namespace BizUnit
{
    /// <summary>
	/// The IValidationStep interface is implemented by validation sub-steps that perform validation against data.
	/// </summary>
	/// 
	/// <remarks>
	/// The following example demonstrates how to create and call BizUnit:
	/// 
	/// <code escaped="true">
	///	public class BinaryValidation : IValidationStep
	///	{
	///		public void ExecuteValidation(Stream data, XmlNode validatorConfig, Context context)
	///		{
	///			MemoryStream dataToValidateAgainst = null;
	///
	///			string comparisonDataPath = context.ReadConfigAsString( validatorConfig, "ComparisonDataPath" );
	///
	///			try
	///			{
	///				try
	///				{
	///					dataToValidateAgainst = StreamHelper.LoadFileToStream(comparisonDataPath);
	///				}
	///				catch(Exception e)
	///				{
	///					context.LogError( "BinaryValidation failed, exception caugh trying to open comparison file: {0}", comparisonDataPath  );
	///					throw;
	///				}
	///
	///				try
	///				{
	///					StreamHelper.CompareStreams( data, dataToValidateAgainst );
	///				}
	///				catch(Exception e)
	///				{
	///					context.LogError( "Binary validation failed while comparing the two data streams with the following exception: {0}", e.ToString() );
	///					throw;
	///				}
	///			}
	///			finally
	///			{
	///				if ( null != dataToValidateAgainst )
	///				{
	///					dataToValidateAgainst.Close();
	///				}
	///			}
	///		}
	///	}
	///	</code>
	///	
	///	</remarks>

	public interface IValidationStep
	{
        /// <summary>
        /// Called by the BizUnit framework to execute the validation test step
        /// </summary>
        /// 
        /// <param name='data'>The stream cintaining the data to be validated.</param>
        /// <param name='validatorConfig'>The Xml fragment containing the configuration for the test step</param>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        void ExecuteValidation(Stream data, XmlNode validatorConfig, Context context);
	}
}
