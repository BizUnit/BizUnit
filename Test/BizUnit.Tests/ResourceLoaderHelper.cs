
namespace BizUnit.Tests
{
    using System.IO;
    using System.Reflection;
    using System.Globalization;

    public class ResourceLoaderHelper
    {
        public static string GetResourceData(string folder, string fileName)
        {
            // Build extended file name 
            Assembly assem = Assembly.GetExecutingAssembly();
            string resourceName = System.String.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}", assem.GetName().Name, folder, fileName);

            Stream configStream = assem.GetManifestResourceStream(resourceName);
            StreamReader sr = new StreamReader(configStream);
            return sr.ReadToEnd();
        }

        public static Stream GetResourceDataAsStream(string folder, string fileName)
        {
            // Build extended file name 
            Assembly assem = Assembly.GetExecutingAssembly();
            string resourceName = System.String.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}", assem.GetName().Name, folder, fileName);

            return assem.GetManifestResourceStream(resourceName);
        }
    }
}
