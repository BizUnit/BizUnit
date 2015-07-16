//---------------------------------------------------------------------
// File: BinaryValidationStep.cs
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
using System.IO;
using System.Xml;
using BizUnit.BizUnitOM;
using BizUnit.CoreSteps.Utilities;

namespace BizUnit.CoreSteps.ValidationSteps
{
	/// <summary>
	/// The BinaryValidationStep performs a binary validation of the data supplied.
	/// </summary>
	/// 
	/// <remarks>
	/// The following shows an example of the Xml representation of this test step.
	/// 
	/// <code escaped="true">
	///	<ValidationStep assemblyPath="" typeName="BizUnit.BinaryValidationStep">
	///		<ComparisonDataPath>.\TestData\ResultDoc1.xml</ComparisonDataPath>
	///		<CompareAsUTF8>true</CompareAsUTF8>
	///	</ValidationStep>
	///	</code>
	///	
	///	<list type="table">
	///		<listheader>
	///			<term>Tag</term>
	///			<description>Description</description>
	///		</listheader>
	///		<item>
	///			<term>ComparisonDataPath</term>
	///			<description>The path of the data to compare against.</description>
	///		</item>
	///		<item>
	///			<term>CompareAsUTF8</term>
	///			<description>true if both ComparisonDataPath and the data are to be compared to UTF8 before comparing (optional)(default=false)</description>
	///		</item>
	///	</list>
	///	</remarks>	
    [Obsolete("BinaryValidationStep has been deprecated. Investigate the BizUnit.TestSteps namespace.")]
    public class BinaryValidationStep : IValidationStepOM
	{
	    private string _comparisonDataPath;
	    private bool _compareAsUtf8;
        
	    public string ComparisonDataPath
	    {
	        set
	        {
	            _comparisonDataPath = value;
	        }
	    }

	    public bool CompareAsUTF8
	    {
	        set
	        {
	            _compareAsUtf8 = value;
	        }
	    }

        /// <summary>
		/// IValidationStep.ExecuteValidation() implementation
		/// </summary>
		/// <param name='data'>The stream cintaining the data to be validated.</param>
		/// <param name='validatorConfig'>The Xml fragment containing the configuration for the test step</param>
		/// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
		public void ExecuteValidation(Stream data, XmlNode validatorConfig, Context context)
		{
			_comparisonDataPath = context.ReadConfigAsString( validatorConfig, "ComparisonDataPath" );
			_compareAsUtf8 = context.ReadConfigAsBool( validatorConfig, "CompareAsUTF8", true);

            ExecuteValidation(data, context);
		}

	    public void ExecuteValidation(Stream data, Context context)
	    {
            MemoryStream dataToValidateAgainst = null;

            try
            {
                try
                {
                    dataToValidateAgainst = StreamHelper.LoadFileToStream(_comparisonDataPath);

                }
                catch (Exception e)
                {
                    context.LogError("BinaryValidationStep failed, exception caugh trying to open comparison file: {0}", _comparisonDataPath);
                    context.LogException(e);

                    throw;
                }

                try
                {
                    data.Seek(0, SeekOrigin.Begin);
                    dataToValidateAgainst.Seek(0, SeekOrigin.Begin);

                    if (_compareAsUtf8)
                    {
                        // Compare the streams, make sure we are comparing like for like
                        StreamHelper.CompareStreams(StreamHelper.EncodeStream(data, System.Text.Encoding.UTF8), StreamHelper.EncodeStream(dataToValidateAgainst, System.Text.Encoding.UTF8));
                    }
                    else
                    {
                        StreamHelper.CompareStreams(data, dataToValidateAgainst);
                    }
                }
                catch (Exception e)
                {
                    context.LogError("Binary validation failed while comparing the two data streams with the following exception: {0}", e.ToString());

                    // Dump out streams for validation...
                    data.Seek(0, SeekOrigin.Begin);
                    dataToValidateAgainst.Seek(0, SeekOrigin.Begin);
                    context.LogData("Stream 1:", data);
                    context.LogData("Stream 2:", dataToValidateAgainst);

                    throw;
                }
            }
            finally
            {
                if (null != dataToValidateAgainst)
                {
                    dataToValidateAgainst.Close();
                }
            }
        }

	    public void Validate(Context context)
	    {
            // compareAsUTF8 - optional

            if (string.IsNullOrEmpty(_comparisonDataPath))
            {
                throw new ArgumentNullException("ComparisonDataPath is either null or of zero length");
            }

            _comparisonDataPath = context.SubstituteWildCards(_comparisonDataPath);
	    }
	}
}
