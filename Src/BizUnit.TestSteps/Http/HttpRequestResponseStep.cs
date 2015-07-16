
using System;
using System.Collections.ObjectModel;
using System.IO;
using BizUnit.Common;
using BizUnit.TestSteps.Common;
using BizUnit.Xaml;

namespace BizUnit.TestSteps.Http
{
    ///<summary>
    /// The HttpRequestResponseStep is used to post a two way HTTP request-response.
    ///</summary>
    public class HttpRequestResponseStep : TestStepBase
    {
        ///<summary>
        /// The location of the data to be posted over HTTP
        ///</summary>
        public string SourcePath { get; set; }

        ///<summary>
        /// The Url which the data will be posted to
        ///</summary>
        public string DestinationUrl { get; set; }

        ///<summary>
        /// The length of time to wait to the HTTP return code
        ///</summary>
        public int RequestTimeout { get; set; }

        ///<summary>
        /// Default constructor
        ///</summary>
        public HttpRequestResponseStep()
        {
            SubSteps = new Collection<SubStepBase>();
        }

        /// <summary>
        /// Execute() implementation
        /// </summary>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public override void Execute(Context context)
        {
            MemoryStream request = null;
            MemoryStream response = null;

            try
            {
                context.LogInfo("HttpRequestResponseStep about to post data from File: {0} to the Url: {1}", SourcePath, DestinationUrl);

                // Get the data to post...
                request = StreamHelper.LoadFileToStream(SourcePath);
                byte[] data = request.GetBuffer();

                // Post the data...
                response = HttpHelper.SendRequestData(DestinationUrl, data, RequestTimeout, context);

                // Dump the respons to the console...
                StreamHelper.WriteStreamToConsole("HttpRequestResponseStep response data", response, context);

                // Validate the response...
                try
                {
                    response.Seek(0, SeekOrigin.Begin);

                    foreach (var subStep in SubSteps)
                    {
                        subStep.Execute(response, context);
                        response.Seek(0, SeekOrigin.Begin);
                    }
                }
                catch (Exception e)
                {
                    throw new ApplicationException("HttpRequestResponseStep response stream was not correct!", e);
                }
            }
            finally
            {
                if (null != response)
                {
                    response.Close();
                }

                if (null != request)
                {
                    request.Close();
                }
            }
        }

        public override void Validate(Context context)
        {
            ArgumentValidation.CheckForEmptyString(SourcePath, "SourcePath");
            ArgumentValidation.CheckForEmptyString(DestinationUrl, "DestinationUrl");
        }
    }
}
