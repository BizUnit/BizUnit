
using System.IO;

namespace BizUnit.Core.ExtensionMethods
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'StringExtensionMethods'
    public static class StringExtensionMethods
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'StringExtensionMethods'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'StringExtensionMethods.GetAsStream(string)'
        public static Stream GetAsStream(this string data)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'StringExtensionMethods.GetAsStream(string)'
        {
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);
            sw.Write(data);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }
    }
}
