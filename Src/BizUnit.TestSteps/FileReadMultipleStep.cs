//---------------------------------------------------------------------
// File: FileReadMultipleStep.cs
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

namespace BizUnitCoreTestSteps
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Collections.ObjectModel;
    using BizUnit;

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

    public class FileReadMultipleStep : TestStepBase
    {
        private Collection<SubStepBase> _subSteps = new Collection<SubStepBase>();

        public int Timeout { get; set; }
        public string DirectoryPath { get; set; }
        public string SearchPattern { get; set; }
        public bool DeleteFiles { get; set; }

        public Collection<SubStepBase> SubSteps
        {
            set
            {
                _subSteps = value;
            }
            get
            {
                return _subSteps;
            }
        }

		/// <summary>
        /// TestStepBase.Execute() implementation
		/// </summary>
		/// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
		public override void Execute(Context context)
        {
            Thread.Sleep(Timeout);
			
            // Get the list of files in the directory
            string [] filelist = Directory.GetFiles( DirectoryPath, SearchPattern );

            if ( filelist.Length == 0)
            {
                // Expecting more than one file 
                throw new ApplicationException( String.Format( "Directory contains no files matching the pattern!" ) );
            }
            
            // For each file in the file list
            foreach (string filePath in filelist)
            {
                context.LogInfo("FileReadMultipleStep validating file: {0}", filePath);

                Stream fileData = StreamHelper.LoadFileToStream(filePath, Timeout);
                context.LogData("File: " + filePath, fileData);
                fileData.Seek(0, SeekOrigin.Begin);

                // Check it against the validate steps to see if it matches one of them
                foreach(var subStep in _subSteps)
                {
                    try
                    {
                        // Try the validation and catch the exception
                        fileData = subStep.Execute(fileData, context);
                    }
                    catch (Exception ex)
                    {
                        context.LogException(ex);
                        throw;
                    }
                }   

                if(DeleteFiles)
                {
                    File.Delete(filePath);
                }
            }
        }

        public override void Validate(Context context)
        {
            ;
        }
    }
}
