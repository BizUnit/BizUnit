
using System.IO;

namespace BizUnit.ExtensionMethods
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'StreamExtensionMethods'
    public static class StreamExtensionMethods
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'StreamExtensionMethods'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'StreamExtensionMethods.GetAsString(Stream)'
        public static string GetAsString(this Stream data)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'StreamExtensionMethods.GetAsString(Stream)'
        {
            var sr = new StreamReader(data);
            return sr.ReadToEnd();
        }
    }
}
