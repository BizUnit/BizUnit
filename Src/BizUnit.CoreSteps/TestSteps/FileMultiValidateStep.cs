//---------------------------------------------------------------------
// File: FileMultiValidateStep.cs
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
using System.Threading;
using System.Xml;
using BizUnit.CoreSteps.Utilities;

namespace BizUnit.CoreSteps.TestSteps
{
	internal class MultiUnknownException : Exception
    {
        internal MultiUnknownException( string message ) : base( message )
        {
        }
    }

    /// <summary>
    /// The FileMultiValidateStep step checks a given directory for files matching the file masks and iterates around all of the specified validate steps
    /// to validate the file.
    /// </summary>
    /// 
    /// <remarks>
    /// The following shows an example of the Xml representation of this test step.
    /// 
    /// <code escaped="true">
    /// <TestStep assemblyPath="" typeName="BizUnit.FileMultiValidateStep">
    ///     <Timeout>6000</Timeout>
    ///     <DirectoryPath>.\Rec_01</DirectoryPath>
    ///     <SearchPattern>*.xml</SearchPattern>
    ///			
    ///		<!-- Note: Validation step could be any generic validation step -->	
    ///		<ValidationStep assemblyPath="" typeName="BizUnit.XmlValidationStep">
    ///		    <XmlSchemaPath>.\TestData\PurchaseOrder.xsd</XmlSchemaPath>
    ///			<XmlSchemaNameSpace>http://SendMail.PurchaseOrder</XmlSchemaNameSpace>
    ///			<XPathList>
    ///			    <XPathValidation query="/*[local-name()='PurchaseOrder' and namespace-uri()='http://SendMail.PurchaseOrder']/*[local-name()='PONumber' and namespace-uri()='']">PONumber_0</XPathValidation>
    ///			</XPathList>
    ///     </ValidationStep>
    ///     			
    ///		<ValidationStep assemblyPath="" typeName="BizUnit.XmlValidationStep">
    ///		    <XmlSchemaPath>.\TestData\SalesOrder.xsd</XmlSchemaPath>
    ///			<XmlSchemaNameSpace>http://SendMail.SalesOrder</XmlSchemaNameSpace>
    ///			<XPathList>
    ///			    <XPathValidation query="/*[local-name()='PurchaseOrder' and namespace-uri()='http://SendMail.SalesOrder']/*[local-name()='PONumber' and namespace-uri()='']">PONumber_0</XPathValidation>
    ///			</XPathList>
    ///     </ValidationStep>
    /// </TestStep>
    /// </code>
    ///	
    ///	<list type="table">
    ///		<listheader>
    ///			<term>Tag</term>
    ///			<description>Description</description>
    ///		</listheader>
    ///		<item>
    ///			<term>Timeout</term>
    ///			<description>Time to wait before checking</description>
    ///		</item>
    ///		<item>
    ///			<term>DirectoryPath</term>
    ///			<description>Directory path to check</description>
    ///		</item>
    ///		<item>
    ///			<term>SearchPattern</term>
    ///			<description>Matching pattern for files</description>
    ///		</item>
	///		<item>
	///			<term>ValidationStep</term>
	///			<description>The validation step to use against the files <para>(one or more)</para></description>
	///		</item>
	///	</list>
    ///	</remarks>
    [Obsolete("FileMultiValidateStep has been deprecated. Investigate the BizUnit.TestSteps namespace.")]
    public class FileMultiValidateStep : ITestStep
    {
		/// <summary>
		/// ITestStep.Execute() implementation
		/// </summary>
		/// <param name='testConfig'>The Xml fragment containing the configuration for this test step</param>
		/// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
		public void Execute(XmlNode testConfig, Context context)
        {
            int timeout = context.ReadConfigAsInt32(testConfig, "Timeout");
            Thread.Sleep(timeout);
			
            // Get the list of files in the directory
            string directoryPath = context.ReadConfigAsString(testConfig, "DirectoryPath");
            string pattern = context.ReadConfigAsString(testConfig, "SearchPattern");
            string [] filelist = Directory.GetFiles( directoryPath, pattern ) ;

            if ( filelist.Length == 0)
            {
                // Expecting more than one file 
                throw new ApplicationException( String.Format( "Directory contains no files matching the pattern!" ) );
            }

            if ( filelist.Length == 1 )
            {
                // Expecting more than one file 
                throw new ApplicationException( String.Format( "Directory only contains one file matching the pattern!" ) );
            }

            // Get the validate steps
            XmlNodeList validationConfigs = testConfig.SelectNodes( "ValidationStep");
            bool nullException = false ;
            int foundSteps = 0 ;

            // For each file in the file list
            foreach ( string filePath in filelist )
            {
                context.LogInfo("FileXmlValidateStep validating file: {0}", filePath );

                MemoryStream xmlData = StreamHelper.LoadFileToStream(filePath, timeout);
                StreamHelper.WriteStreamToConsole( "File data to be validated", xmlData, context );

                // Check it against the validate steps to see if it matches one of them
                for ( int i = 0 ; i < validationConfigs.Count ; i++, nullException = false )
                {
                    try
                    {
                        // Try the validation and catch the exception
                        xmlData.Seek(0, SeekOrigin.Begin);
                        context.ExecuteValidator( xmlData, validationConfigs.Item( i ) ) ;
                    }
                    catch ( NullReferenceException )
                    {
                        // Not found a node matching XPath, do nothing
                        nullException = true ;
                    }
                    catch ( ApplicationException )
                    {
                        // Not a matching comparision, do nothing
                        nullException = true ;
                    }
                    catch ( Exception )
                    {
                        // Not RegEx validation, do nothing
                        nullException = true ;
                    }   // try

                    // Have we successfully run a validation?
                    if ( nullException == false )
                    {
                        // Yes, must be a match!
                        foundSteps++ ;
                        break ;
                    }   // nullException
                }   // i
            }   // filePath

            if ( foundSteps != filelist.Length )
            {
                throw new MultiUnknownException(string.Format( "FileMultiValidateStep failed, did not match all the files to those specifed in the validate steps." ) );
            }
        }
    }
}
