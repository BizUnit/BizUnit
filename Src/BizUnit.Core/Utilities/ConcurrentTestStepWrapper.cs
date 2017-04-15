//---------------------------------------------------------------------
// File: ConcurrentTestStepWrapper.cs
// 
// Summary: 
//
//---------------------------------------------------------------------
// Copyright (c) 2004-2017, Kevin B. Smith. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------

using System;
using BizUnit.Core.TestBuilder;

namespace BizUnit.Core.Utilites
{
	/// <summary>
	/// Summary description for ConcurrentTestStepWrapper.
	/// </summary>
	internal class ConcurrentTestStepWrapper
	{
		private readonly Context _context;
		private readonly ILogger _logger;

        public ConcurrentTestStepWrapper(TestStepBase testStep, Context ctx )
        {
            TestStep = testStep;
            _logger = new Logger();
		    _logger.ConcurrentExecutionMode = true;
            _context = ctx.CloneForConcurrentUse(_logger);
        }

		public string Name
		{
			get 
			{
				return (null != TestStep) ? TestStep.GetType().ToString() : null;
			}
		}

        public bool FailOnError
        {
            get
            {
                return (null != TestStep) ? TestStep.FailOnError : true;
            }
        }

	    public string StepName
	    {
	        get
	        {
                if (null != TestStep)
                    return TestStep.GetType().ToString();
                
	            return null;
	        }
	    }

	    public TestStepBase TestStep { get; private set; }
		public Exception FailureException { get; private set; }
        public ILogger Logger
        {
            get
            {
                return _logger;
            }
        }

        public void Execute()
		{
			try
			{
                if(null != TestStep)
                {
                    _logger.TestStepStart(TestStep.GetType().ToString(), DateTime.Now, true, TestStep.FailOnError);
                    if (TestStep is ImportTestCaseStep)
                    {
                        ExecuteImportedTestCase(TestStep as ImportTestCaseStep, _context);
                    }
                    else
                    {
                        TestStep.Execute(_context);
                    }
                }
			}
			catch(Exception e)
			{
				_logger.LogException( e );
                FailureException = e;
			}
		}

        private static void ExecuteImportedTestCase(ImportTestCaseStep testStep, Context context)
        {
            var testCase = testStep.GetTestCase();
            var bu = new TestRunner(testCase, context);
            bu.Run();
        } 
	}
}
