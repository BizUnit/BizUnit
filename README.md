# BizUnit
**Framework for automated testing of distributed systems**

More information avaiable in the *Getting Started Guide* installed with BizUnit or take a look at my blog: http://kevinsmi.wordpress.com.

BizUnit is a framework and as such does not have any dependency on either NUnit of VS Unit Testing, either of these make a great way to drive BizUnit test cases, though equally you could write custom code to do the same.

##Test Case Format
A test case can be represented as an Xml document or using the BizUnit object model, it is made up of three stages, *_test setup_*, *_test execution_* and *_test cleanup_*, the cleanup stage is always executed (even if the main execution stage fails) and intended to leave the platform in the same state that it started.
Each stage may consist of zero or more test steps, test steps are in general autonomous, state can be flowed between them if required using the ‘context’ object that is passed to each test step by the framework.
BizUnit also has the notion of TestGroupSetup and TestGroupTearDown, these are test cases that are executed at the beginning and end of a suite of unit tests.

In addition to test steps, BizUnit has the notion of *_validation_* steps and *_context loader_* steps. These can be thought of as sub-steps and can in general be independantly executed from any test step. For example, an MSMQ-read step might be used to read and validate both Xml and Flat File data from a queue, the same step can be used with both the RegExValidationStep and the XmlValidationStep to validate the data read.

A test step within a test case can be marked with the attribute - *_runConcurrently_* which causes subsequent test steps to be started before it has completed. In addition test steps maybe marked with the attribute - *_failOnError_*, setting it to false cause BizUnit to ignore a failure of that test step, this is particularly useful for the setup and cleanup stages of test cases.

###Lets look at an Example Scenario...
BizUnit takes a black box approach to testing solutions, lets look at the BizTalk scenario below, a BizTalk solution receives a request-response message over HTTP, the message is routed to an Orchestration which, sends a message to MSMQ and another to a FILE drop, the Orchestration waits for a FILE to be received, after which the Orchestration sends the response back to the waiting HTTP client. The solution also uses BAM, writing business data to the BAM database.

In order to test this scenario, a BizUnit test case is defined that has 5 test steps:

* The HttpRequestResponseStep sends the request to the two-way receive port and waits for the response. This step is executed concurrently so that the other test steps may execute whilst it waiting for the response 
* The MSMQReadStep waits for a message to appear on an MSMQ queue. 
** When it reads the message it uses the XmlValidationStep to perform schema validation and also execute a number of XPath expression to ensure the message contains the correct data 
* The FileValidateStep waits for a FILE to be written to a given directory
** When it reads the FILE it validates the data using the RegExValidationStep validation step since the FILE picked up was a flat file format 
* The FileCreateStep creates a new FILE in the specified directory containing the data that the backend system would typically create. This allows the Orchestration to complete and send the response back to the waiting HttpRequestResponseStep step 
* Finally, DBQueryStep is used to check that all of the BAM data has been successfully written to the BAMPrimaryImportDB
 
####Test Steps
A test step is a .NET class which implements the *_ITestStep_* interface:


    public interface ITestStep
    {
        void Execute(XmlNode testConfig, Context context);
    }


BizUnit will create and execute the test steps as dictated by the Xml test case. The test case will list the steps that need to be excuted in each stage of the test. The example below will cause BizUnit to create the _Microsoft.Services.BizTalkApplicationFramework.BizUnit.FileCreateStep_. BizUnit uses the *_assemblyPath_* and *_typeName_* to load and create the type:


    <TestStep assemblyPath="" typeName="Microsoft.Services.BizTalkApplicationFramework.BizUnit.FileCreateStep">
        ...
    </TestStep>

####Executing Steps Concurrently
Test steps can be maked to execute concurrently by decorating them with the *_runConcurrently_* attribute:

    <TestStep assemblyPath="" typeName="Microsoft.Services.BizTalkApplicationFramework.BizUnit.HttpRequestResponseStep" runConcurrently="true">
        ...
    </TestStep>

###Reading Configuration from Context
BizUnit enables state to be flowed between test steps using the Context object, subsequent steps may read configuration from the Context object which was written by a previous test step, this is acheived using the *_takeFromCtx_* attribute within a steps configuration. For example:


    <TestStep assemblyPath="" typeName="Microsoft.Services.BizTalkApplicationFramework.BizUnit.HttpPostStep">
        <SourcePath>.\TestData\InDoc1.xml</SourcePath>    
        <DestinationUrl takeFromCtx="HTTPDest">http://localhost/TestFrameworkDemo/BTSHTTPReceive.dll</DestinationUrl>
        <RequestTimeout>60000</RequestTimeout>
    </TestStep>

#####Wild Cards
BizUnit supports wild card for reading configuration, the following wild cards are supported
 
* *_%DateTime%_* - will replace the wild card with the current date time in the format HHmmss-ddMMyyyy 
* *_%ServerName%_* - will replace the wild card with the name of the server BizUnit is being executed on 
* *_%Guid%_* - will be replaced by a new Guid

For example, for the test step configuration below:

    <TestStep assemblyPath="" typeName="Microsoft.Services.BizTalkApplicationFramework.BizUnit.FileCreateStep">
        <SourcePath>..\..\..\TestData\InDoc1.xml</SourcePath>         
        <CreationPath>..\..\..\Rec_03\TransactionId_%Guid%_%ServerName%.xml</CreationPath>
    </TestStep>

CreationPath becomes _"..\..\..\Rec_03\TransactionId_12345678-D6AB-4aa9-A772-938972E3FD51_ZEUS001.xml"_

####Validation Steps
BizUnit supports the notion of validation steps which may be nested within test steps which support validation. This means that an MSMQ read step may use an XML validation step or a regular expression validation step to validate the data that it receives. Validation steps need to implement the *_IValidationStep_* interface. 

    public interface IValidationStep
    {
        void ExecuteValidation(Stream data, XmlNode validatorConfig, Context context);
    }

A test step may use the Context object utilities to execute the appropriate validation step as shown below:
 
    public void Execute(XmlNode testConfig, Context context)
    {
        XmlNode validationConfig = testConfig.SelectSingleNode("ValidationStep");
        MemoryStream data = null;

        ...

        context.ExecuteValidator( data, validationConfig );
    }

The test step snippet below illustrates how a validation step is embeded in a file read step:

    <TestStep assemblyPath="" typeName="Microsoft.Services.BizTalkApplicationFramework.BizUnit.FileValidateStep">
        <Timeout>3000</Timeout>
        <Directory>..\..\..\Rec_03\</Directory>
        <SearchPattern>TransactionId_*.xml</SearchPattern>
        <DeleteFile>true</DeleteFile>
			
        <ValidationStep assemblyPath="" typeName="Microsoft.Services.BizTalkApplicationFramework.BizUnit.XmlValidationStep">
             <XmlSchemaPath>..\..\..\TestData\PurchaseOrder.xsd</XmlSchemaPath>
             <XmlSchemaNameSpace>http://SendMail.PurchaseOrder</XmlSchemaNameSpace>
             <XPathList>
                <XPathValidation query=
                "/*[local-name()='PurchaseOrder' and namespace-uri()='http://SendMail.PurchaseOrder']
                /*[local-name()='PONumber' and namespace-uri()='']">PONumber_0</XPathValidation>
             </XPathList>
        </ValidationStep>			
    </TestStep>

####Context Loader Steps
Similarly, BizUnit supports context loader steps, which are responsible for loading data into the BizUnit context. These types of need to implement the *_IContextLoaderStep_* interface as shown below:

    public interface IContextLoaderStep
    {
        void ExecuteContextLoader(Stream data, XmlNode contextConfig, Context context);
    }

###Getting Started
The best way to get started is to download the latest version, install it and then take a look at the SDK samples. All the source is currently included, so feel free to take a closer look at the code. All the test steps along with the framework itself are documented in the .CHM that is installed.

Finally, my apologies for the shameless plug :-), but Chapter 8 of Professional BizTalk Server 2006: http://www.amazon.com/Professional-BizTalk-Server-Darren-Jefford/dp/0470046422/ref=pd_bbs_sr_4/103-9081803-2555041?ie=UTF8&s=books&qid=1172915706&sr=8-4 book which I co-authored discusses in more detail how BizUnit may be used as an integral part of your development process, and how it may be used to drive stress and performance testing. 

###Acknowledgments
I'd like to thanks the follwing people who have contributed to BizUnit in some way, either by donating test steps, identifying bugs, or by providing requirements which have subsequently been implemented. I may have missed some people off, if so, it's not intensional, please drop me a mail to remind me:

Iain Bapty, Dharshana Kalahejagoda, Jon Fancey, Isaac Young, Mike Becker, Tanveer Rashid, Young Jun Hong, Dave Regan, Ian Cross, Greg Beach, Daren Jefford, Kevin Purcell, Karina Apostolides, Jon Bonnick, Brian Milburn, Rahmatullah Khan, Bram Veldhoen.

Enjoy!
