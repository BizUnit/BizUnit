
namespace BizUnit
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.ComponentModel;
    using System.Collections.ObjectModel;

    public class BizUnitTestCaseXaml
    {
        private Collection<TestStepBase> _setupSteps;
        private Collection<TestStepBase> _executionSteps;
        private Collection<TestStepBase> _cleanupSteps;

        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Purpose { get; set; }
        public string Reference { get; set; }
        public string Preconditions { get; set; }
        public string ExpectedResults { get; set; }
        public string BizUnitVersion { get; set; }

        public BizUnitTestCaseXaml()
        {
            BizUnitVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Collection<TestStepBase> SetupSteps 
        {
            get 
            {
                return _setupSteps ?? (_setupSteps = new Collection<TestStepBase>()); 
            } 
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Collection<TestStepBase> ExecutionSteps
        {
            get
            {
                return _executionSteps ?? (_executionSteps = new Collection<TestStepBase>());
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Collection<TestStepBase> CleanupSteps
        {
            get
            {
                return _cleanupSteps ?? (_cleanupSteps = new Collection<TestStepBase>());
            }
        }

        public static void SaveToFile(BizUnitTestCaseXaml testCase, string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None))
            {
                BizUnitSerializationHelper.Serialize(testCase, fs);
                fs.Flush();
            }
        }

        public static string Save(BizUnitTestCaseXaml testCase)
        {
            return BizUnitSerializationHelper.Serialize(testCase);
        }

        public static BizUnitTestCaseXaml LoadFromFile(string filePath)
        {
            string testCase;
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                StreamReader sr = new StreamReader(fs);
                testCase = sr.ReadToEnd();
            }
            return (BizUnitTestCaseXaml)BizUnitSerializationHelper.Deserialize(testCase);
        }

        public static BizUnitTestCaseXaml LoadXaml(string xamlTestCase)
        {
            throw new NotImplementedException();
        }
    }
}
