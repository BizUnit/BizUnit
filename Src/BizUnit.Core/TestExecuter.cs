//---------------------------------------------------------------------
// File: TestExecuter.cs
// 
// Summary: 
//
//---------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------

namespace BizUnit
{
	using System;
	using System.IO;
	using System.Xml;
	using System.Collections;
	using System.Reflection;
	using System.Threading;

	/// <summary>
	/// Summary description for TestExecuter.
	/// </summary>
	public class TestExecuter
	{
		XmlNodeList setupSteps;
		XmlNodeList executeSteps;
		XmlNodeList teardownSteps;
		string testName = "Unknown";
		Exception executionException;
		Context context;
		internal Logger logger;
		Queue completedConcurrentSteps = new Queue();
		int inflightQueueDepth;
	
		public TestExecuter(string configFile)
		{
			Logger logger = new Logger();
			this.context = new Context(this, logger);
			XmlDocument testConfig = new XmlDocument();
			testConfig.Load(configFile);

			// Get test name...
			XmlNode nameNode = testConfig.SelectSingleNode("/TestCase/@testName");
			if ( null != nameNode )
			{
				this.testName = nameNode.Value;
			}

			logger.WriteLine(" ");
			logger.WriteLine(new string('-', 79));
			logger.WriteLine("                                   S T A R T" );
			logger.WriteLine( " " );
			string userName = GetUserName();
			logger.WriteLine(string.Format("Test: {0} started @ {1} by {2}", this.testName, GetNow(), userName));
			logger.WriteLine(new string('-', 79));

			// Get test setup / execution / teardown steps
			this.setupSteps = testConfig.SelectNodes("/TestCase/TestSetup/*");
			this.executeSteps = testConfig.SelectNodes("/TestCase/TestExecution/*");
			this.teardownSteps = testConfig.SelectNodes("/TestCase/TestCleanup/*");

			this.logger = logger;
		}

		public void RunTest()
		{
			Setup();
			Execute();
			TearDown();

			this.logger.Close();
		}

		private void Setup()
		{
			this.logger.TestSetUp(this.testName);

			ExecuteSteps(this.setupSteps);
		}

		private void Execute()
		{
			this.logger.TestExecute(this.testName);

			try
			{
				ExecuteSteps(this.executeSteps);
			}
			catch(Exception e)
			{
				// If we caught an exception on the main test execution, save it, perform cleanup,
				// then throw the exception...
				this.executionException = e;
			}
		}

		private void TearDown()
		{
			this.logger.TestTearDown(this.testName);

			try
			{
				ExecuteSteps(this.teardownSteps);
			}
			catch(Exception e)
			{
				this.logger.LogException( e );
				this.logger.WriteLine(new string('-', 79));
				this.logger.WriteLine(string.Format("Test: {0} ended @ {1}", this.testName, GetNow() ));
				this.logger.WriteLine( "" );
				this.logger.WriteLine("                             ****** F A I L ******" );
				this.logger.WriteLine(new string('-', 79));

				if ( null != this.executionException )
				{
					throw this.executionException;
				}
				else
				{
					throw;
				}
			}

			this.logger.WriteLine(new string('-', 79));
			this.logger.WriteLine(string.Format("Test: {0} ended @ {1}", this.testName, GetNow() ));
			this.logger.WriteLine( "" );

			if ( null != this.executionException )
			{
				this.logger.WriteLine("                             ****** F A I L ******" );
			}
			else
			{
				this.logger.WriteLine("                                    P A S S" );
			}

			this.logger.WriteLine(new string('-', 79));

			if ( null != this.executionException )
			{
				throw this.executionException;
			}
		}

		private void ExecuteSteps(XmlNodeList steps)
		{
			if ( null == steps )
			{
				return;
			}

			foreach (XmlNode stepConfig in steps)
			{
				bool runConcurrently = false;
				XmlNode assemblyPath = stepConfig.SelectSingleNode("@assemblyPath");
				XmlNode typeName = stepConfig.SelectSingleNode("@typeName");
				XmlNode runConcurrentlyNode = stepConfig.SelectSingleNode("@runConcurrently");

				if ( null != runConcurrentlyNode )
				{
					runConcurrently = Convert.ToBoolean(runConcurrentlyNode.Value);
				}

				object obj = CreateStep(typeName.Value, assemblyPath.Value);
				ITestStep step = obj as ITestStep;

				try
				{
					// Should this step be executed concurrently?
					if ( runConcurrently )
					{
						this.logger.WriteLine(string.Format("\nStep: {0} started  c o n c u r r e n t l y  @ {1}", typeName.Value, GetNow() ));
						Interlocked.Increment(ref this.inflightQueueDepth);
						ThreadPool.QueueUserWorkItem(new WaitCallback(this.WorkerThreadThunk), new ConcurrentTestStepWrapper(step, stepConfig, this, typeName.Value));
					}
					else
					{
						this.logger.WriteLine(string.Format("\nStep: {0} started @ {1} ", typeName.Value, GetNow() ));

						step.Execute(stepConfig, context);
					}
				}
				catch(Exception e)
				{
					this.logger.LogException( e );
					throw;
				}

				if ( !runConcurrently )
				{
					this.logger.WriteLine(string.Format("Step: {0} ended @ {1}", typeName.Value, GetNow() ));
				}

				FlushConcurrentQueue(false);
			}

			FlushConcurrentQueue(true);
		}

		private void FlushConcurrentQueue(bool waitingToFinish)
		{
			if ( waitingToFinish && this.inflightQueueDepth == 0 )
			{
				return;
			}

			while ( (this.completedConcurrentSteps.Count > 0) || waitingToFinish )
			{
				object obj = null;

				lock(this.completedConcurrentSteps.SyncRoot)
				{
					if ( this.completedConcurrentSteps.Count > 0 )
					{
						try
						{
							obj = this.completedConcurrentSteps.Dequeue();
						}
						catch(Exception)
						{
						}
					}
				}

				if ( null != obj )
				{
					ConcurrentTestStepWrapper step = (ConcurrentTestStepWrapper)obj;
					string testLog = step.GetLogText();
					this.logger.WriteLine(testLog);

					this.logger.WriteLine(string.Format("Step: {0} ended @ {1} (Concurrent Execution Mode defined)", step.Name, GetNow() ));

					// Check to see if the test step failed, if it did throw the exception...
					if ( null != step.FailureException )
					{
						Interlocked.Decrement(ref this.inflightQueueDepth);
						throw step.FailureException;
					}

					Interlocked.Decrement(ref this.inflightQueueDepth);
				}

				if ( waitingToFinish && (this.inflightQueueDepth > 0) )
				{
					Thread.Sleep(250);
				}
				else if ( waitingToFinish && (this.inflightQueueDepth == 0) )
				{
					break;
				}
			}
		}

		private void WorkerThreadThunk(Object stateInfo)
		{
			ConcurrentTestStepWrapper step = (ConcurrentTestStepWrapper)stateInfo;
			step.Execute();

			// This step is completed, add to queue
			lock(this.completedConcurrentSteps.SyncRoot)
			{
				this.completedConcurrentSteps.Enqueue(step);
			}
		}

		static private IValidationTestStep CreateValidatorStep(string typeName, string assemblyPath)
		{
			return (IValidationTestStep)CreateStep( typeName, assemblyPath );
		}

		static private IContextDataLoader CreateContextDataLoaderStep(string typeName, string assemblyPath)
		{
			return (IContextDataLoader)CreateStep( typeName, assemblyPath );
		}

		static private object CreateStep(string typeName, string assemblyPath)
		{
			object comp = null;
			Type ty = null;

			if (assemblyPath!=null && assemblyPath.Length != 0) 
			{
				Assembly assembly = Assembly.LoadFrom(assemblyPath);
				ty = assembly.GetType(typeName, true, false);
			}
			else 
			{
				ty = Type.GetType(typeName);
			}

			if (ty != null) 
			{
				comp = Activator.CreateInstance(ty);
			}
			
			return comp;
		}

		internal void ExecuteValidator(Stream data, XmlNode validatorConfig, Context context)
		{
			if ( null == validatorConfig )
			{
				return;
			}

			XmlNode assemblyPath = validatorConfig.SelectSingleNode("@assemblyPath");
			XmlNode typeName = validatorConfig.SelectSingleNode("@typeName");

			this.logger.WriteLine(string.Format("\nValidation: {0} started @ {1}", typeName.Value, GetNow() ));

			IValidationTestStep v = CreateValidatorStep(typeName.Value, assemblyPath.Value);
			v.Execute( data, validatorConfig, context);

			this.logger.WriteLine(string.Format("Validation: {0} ended @ {1}\n", typeName.Value, GetNow() ));
		}

		internal void ExecuteContextLoader(Stream data, XmlNode contextConfig, Context context)
		{
			if ( null == contextConfig )
			{
				return;
			}

			XmlNode assemblyPath = contextConfig.SelectSingleNode("@assemblyPath");
			XmlNode typeName = contextConfig.SelectSingleNode("@typeName");

			this.logger.WriteLine(string.Format("\nContextLoad: {0} started @ {1}", typeName.Value, GetNow() ));

			IContextDataLoader cd = CreateContextDataLoaderStep(typeName.Value, assemblyPath.Value);
			cd.LoadContextData(data, contextConfig, context);

			this.logger.WriteLine(string.Format("ContextLoad: {0} ended @ {1}\n", typeName.Value, GetNow() ));
		}

		internal static string GetNow()
		{
			return DateTime.Now.ToString("HH:mm:ss.fff dd/MM/yyyy");
		}

		internal static string GetUserName()
		{
			string usersDomain = Environment.UserDomainName;
			string usersName = Environment.UserName;

			return usersDomain + "\\" + usersName;
		}
	}
}
