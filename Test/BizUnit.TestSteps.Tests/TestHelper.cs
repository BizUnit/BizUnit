
using System.IO;

namespace BizUnit.TestBuilderteps.Tests
{
    internal static class TestHelper
    {
        /// <summary>
        /// Deletes the file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        internal static void DeleteFile(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.SetAttributes(filePath, FileAttributes.Normal);
                System.IO.File.Delete(filePath);
            }
        }
    }
}
