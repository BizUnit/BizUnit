
namespace BizUnit.Tests
{
    using System.IO;
    using System.Reflection;
    using System.Xml;

    public class BizUnitTestUtils
    {
        public static XmlNode LoadTestStepConfig(string projectFolder, string resourceName)
        {
            string xmlStr = GetResourceData(projectFolder, resourceName);
            XmlDocument dom = new XmlDocument();
            dom.LoadXml(xmlStr);
            return dom.SelectSingleNode("TestStep");
        }

        public static XmlNode LoadContextLoaderStepConfig(string projectFolder, string resourceName)
        {
            string xmlStr = GetResourceData(projectFolder, resourceName);
            XmlDocument dom = new XmlDocument();
            dom.LoadXml(xmlStr);
            return dom.SelectSingleNode("ContextLoaderStep");
        }        

        public static string GetResourceData(string folder, string fileName)
        {
            // Build extended file name 
            Assembly assem = Assembly.GetExecutingAssembly();
            string resourceName = System.String.Format("{0}.{1}.{2}", assem.GetName().Name, folder, fileName);

            Stream configStream = assem.GetManifestResourceStream(resourceName);
            StreamReader sr = new StreamReader(configStream);
            return sr.ReadToEnd();
        }

        public static Stream GetResourceDataAsStream(string folder, string fileName)
        {
            // Build extended file name 
            Assembly assem = Assembly.GetExecutingAssembly();
            string resourceName = System.String.Format("{0}.{1}.{2}", assem.GetName().Name, folder, fileName);

            return assem.GetManifestResourceStream(resourceName);
        }
    }
}
