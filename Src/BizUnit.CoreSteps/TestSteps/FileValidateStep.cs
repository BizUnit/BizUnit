//---------------------------------------------------------------------
// File: FileValidateStep.cs
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
using System.Threading;
using BizUnit.BizUnitOM;
using BizUnit.CoreSteps.Utilities;

namespace BizUnit.CoreSteps.TestSteps
{
	/// <summary>
	/// The FileValidateStep reads a FILE from a given locaton and validates the contents.
	/// </summary>
	/// 
	/// <remarks>
	/// The following shows an example of the Xml representation of this test step.
	/// 
	/// <code escaped="true">
	///	<TestStep assemblyPath="" typeName="BizUnit.FileValidateStep">
	///		<Timeout>2000</Timeout>
	///		<Directory>C:\Recv2\</Directory>
	///		<SearchPattern>TransactionId_*.xml</SearchPattern>
	///		<DeleteFile>true</DeleteFile>
	///			
	///		<!-- Note: ContextLoader Step could be any generic validation step -->	
	///		<ContextLoaderStep assemblyPath="" typeName="BizUnit.RegExContextLoader">
	///			<RegEx contextKey="HTTP_Url">/def:html/def:body/def:p[2]/def:form</RegEx>
	///			<RegEx contextKey="ActionID">/def:html/def:body/def:p[2]/def:form/def:input[3]</RegEx>
	///			<RegEx contextKey="ActionType">/def:html/def:body/def:p[2]/def:form/def:input[4]</RegEx>
	///			<RegEx contextKey="HoldEvent">/def:html/def:body/def:p[2]/def:form/def:input[2]</RegEx>
	///		</ContextLoaderStep>
	///	
	///		<!-- Note: Validation step could be any generic validation step -->	
	///		<ValidationStep assemblyPath="" typeName="BizUnit.XmlValidationStep">
	///			<XmlSchemaPath>.\TestData\PurchaseOrder.xsd</XmlSchemaPath>
	///			<XmlSchemaNameSpace>http://SendMail.PurchaseOrder</XmlSchemaNameSpace>
	///			<XPathList>
	///				<XPathValidation query="/*[local-name()='PurchaseOrder' and namespace-uri()='http://SendMail.PurchaseOrder']/*[local-name()='PONumber' and namespace-uri()='']">PONumber_0</XPathValidation>
	///			</XPathList>
	///		</ValidationStep>			
	///	</TestStep>
	///	</code>
	///	
	///	<list type="table">
	///		<listheader>
	///			<term>Tag</term>
	///			<description>Description</description>
	///		</listheader>
	///		<item>
	///			<term>Timeout</term>
	///			<description>Timeout to wait for the FILE to be written, in milisecs</description>
	///		</item>
	///		<item>
	///			<term>Directory</term>
	///			<description>The directory where the FILE is located</description>
	///		</item>
	///		<item>
	///			<term>SearchPattern</term>
	///			<description>The search pattern, such as "*.txt"</description>
	///		</item>
	///		<item>
	///			<term>DeleteFile</term>
	///			<description>true if the file should be deleted, false if it should not</description>
	///		</item>
	///		<item>
	///			<term>ContextLoaderStep</term>
	///			<description>The configuration for the context loader step used to load data into the BizUnit context which may be used by subsequent test steps<para>(optional)</para></description>
	///		</item>
	///		<item>
	///			<term>ValidationStep</term>
	///			<description>The configuration for the validation step used to validate the contents of the file, the validation step should implement IValidationTestStep<para>(optional)</para></description>
	///		</item>
	///	</list>
	///	</remarks>
    [Obsolete("FileValidateStep has been deprecated. Investigate the BizUnit.TestSteps namespace.")]
    public class FileValidateStep : ITestStepOM
	{
	    private double _timeout;
	    private string _directory;
	    private string _searchPattern;
	    private bool _deleteFile;
	    private IValidationStepOM _validationStep;
	    private IContextLoaderStepOM _contextLoaderStep;
	    private XmlNode _validationConfig;
	    private XmlNode _contextConfig;

	    ///<summary>
	    ///</summary>
	    public double Timeout
	    {
	        set
	        {
	            _timeout = value;
	        }
	    }

	    public string Directory
	    {
	        set
	        {
	            _directory = value;
	        }
	    }

	    public string SearchPattern
	    {
	        set
	        {
	            _searchPattern = value;
	        }
	    }

	    public bool DeleteFile
	    {
	        set
	        {
	            _deleteFile = value;
	        }
	    }

	    public IValidationStepOM ValidationStep
	    {
	        set
	        {
	            _validationStep = value;
	        }
	    }

	    public IContextLoaderStepOM ContextLoaderStep
	    {
	        set
	        {
	            _contextLoaderStep = value;
	        }
	    }

        /// <summary>
		/// ITestStep.Execute() implementation
		/// </summary>
		/// <param name='testConfig'>The Xml fragment containing the configuration for this test step</param>
		/// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
		public void Execute(XmlNode testConfig, Context context)
		{
            _directory = context.ReadConfigAsString(testConfig, "Directory");
            _searchPattern = context.ReadConfigAsString(testConfig, "SearchPattern");
            _deleteFile = context.ReadConfigAsBool(testConfig, "DeleteFile");
            _timeout = context.ReadConfigAsDouble(testConfig, "Timeout");

            _validationConfig = testConfig.SelectSingleNode("ValidationStep");
            _contextConfig = testConfig.SelectSingleNode("ContextLoaderStep");

            Execute(context);
		}

        /// <summary>
        /// ITestStep.Execute() implementation
        /// </summary>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public void Execute(Context context)
	    {
            MemoryStream data = null;

            try
            {
                context.LogInfo("Searching for files in: \"{0}{1}\"", _directory, _searchPattern);

                DateTime endTime = DateTime.Now + TimeSpan.FromMilliseconds(_timeout);
                FileInfo[] files;

                do
                {
                    var di = new DirectoryInfo(_directory);
                    files = di.GetFiles(_searchPattern);

                    Thread.Sleep(100);
                } while ((files.Length == 0) && (endTime > DateTime.Now));

                if (files.Length == 0)
                {
                    throw new ApplicationException(string.Format("No files were found at: {0}{1}", _directory, _searchPattern));
                }

                context.LogInfo("{0} fies were found at : \"{1}{2}\"", files.Length, _directory, _searchPattern);

                IOException ex = null;
                do
                {
                    try
                    {
                        using (var fs = new FileStream(files[0].FullName, FileMode.Open, FileAccess.Read))
                        {
                            data = StreamHelper.LoadMemoryStream(fs);
                        }
                    }
                    catch (IOException ioex)
                    {
                        context.LogWarning("IOException caught trying to load file, will re-try if within timeout");
                        ex = ioex;
                        Thread.Sleep(100);
                    }
                } while ((null == data) && (endTime > DateTime.Now));

                if (null != ex)
                {
                    throw ex;
                }

                context.LogData(string.Format("Loaded FILE: {0}", files[0].FullName), data);

                data.Seek(0, SeekOrigin.Begin);
                if (null != _contextLoaderStep)
                {
                    _contextLoaderStep.ExecuteContextLoader(data, context);
                }

                if (null != _contextConfig)
                {
                    context.ExecuteContextLoader(data, _contextConfig);
                }

                data.Seek(0, SeekOrigin.Begin);
                if (null != _validationStep)
                {
                    context.ExecuteValidator(data, _validationStep);
                }

                if (null != _validationConfig)
                {
                    context.ExecuteValidator(data, _validationConfig);
                }

                if (_deleteFile)
                {
                    File.Delete(files[0].FullName);
                }
            }
            finally
            {
                if (null != data)
                {
                    data.Close();
                }
            }
        }

        /// <summary>
        /// ITestStepOM.Validate() implementation
        /// </summary>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public void Validate(Context context)
	    {
            if (0 > _timeout)
            {
                throw new ArgumentNullException("Timeout must be greater than zero");
            }

            if (string.IsNullOrEmpty(_directory))
            {
                throw new ArgumentNullException("Directory is either null or of zero length");
            }
            _directory = context.SubstituteWildCards(_directory);

            if (string.IsNullOrEmpty(_searchPattern))
            {
                throw new ArgumentNullException("SearchPattern is either null or of zero length");
            }
            _searchPattern = context.SubstituteWildCards(_searchPattern);

            if(null != _validationStep)
            {
                _validationStep.Validate(context);
            }

            if(null != _contextLoaderStep)
            {
                _contextLoaderStep.Validate(context);
            }
	    }
	}
}
