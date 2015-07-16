
namespace BizUnit.Tests
{
    using System;
    using System.Globalization;
    using System.IO;

    public class FileHelper
    {
        public static void EmptyDirectory(string directory, string filter)
        {
            DirectoryInfo di = new DirectoryInfo(directory);
            FileInfo[] files = di.GetFiles(filter, SearchOption.TopDirectoryOnly);

            foreach (FileInfo file in files)
            {
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Deleting file: {0}", file.FullName));
                File.Delete(file.FullName);
            }
        }

        public static int NumberOfFilesInDirectory(string directory, string filter)
        {
            DirectoryInfo di = new DirectoryInfo(directory);
            FileInfo[] files = di.GetFiles(filter, SearchOption.TopDirectoryOnly);

            return files.Length;
        }
    }
}
