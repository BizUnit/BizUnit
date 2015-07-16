//---------------------------------------------------------------------
// File: PersistMessageHelper.cs
// 
// Summary: 
//
//---------------------------------------------------------------------
// Copyright (c) 2004-2011, Kevin B. Smith. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------

using System.IO;
using System.Text;
using Microsoft.BizTalk.Message.Interop;

namespace BizUnit.TestSteps.BizTalk.Common
{
    static class PersistMessageHelper
    {
        /// <summary>
        /// Helper class to persist BizTalk message bodies
        /// </summary>
        /// <param name='message'>The BizTalk message to persist</param>
        /// <param name='destination'>The destination directory to persist the file to</param>
        public static void PersistMessage(IBaseMessage message, string destination)
        {
            using (var fs = new FileStream(destination, FileMode.Create))
            {
                var enc = Encoding.GetEncoding("UTF-8");
                if (!string.IsNullOrEmpty(message.BodyPart.Charset))
                {
                    enc = Encoding.GetEncoding(message.BodyPart.Charset);
                }
                using (var writer = new StreamWriter(fs, enc))
                {
                    var msgStream = message.BodyPart.GetOriginalDataStream();
                    using (var reader = new StreamReader(msgStream, enc))
                    {
                        const int size = 1024;
                        var buf = new char[size];
                        var charsRead = reader.Read(buf, 0, size);
                        while (charsRead > 0)
                        {
                            writer.Write(buf, 0, charsRead);
                            charsRead = reader.Read(buf, 0, size);
                        }
                    }
                }
            }
        }
    }
}
