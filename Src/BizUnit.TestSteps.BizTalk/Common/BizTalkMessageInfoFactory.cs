//---------------------------------------------------------------------
// File: BizTalkMessageInfoFactory.cs
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

namespace BizUnit.TestSteps.BizTalk.Common
{
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.BizTalk.Message.Interop;
    using Microsoft.Test.BizTalk.PipelineObjects;
    
    public static class BizTalkMessageInfoFactory
    {
        public static MessageInfo CreateMessageInfo(IBaseMessage message, string destination)
        {
            var mc = (MessageContext)message.Context;

            var miciapael = new List<MessageInfoContextInfoArrayPropertyArrayElement1>(message.PartCount);
            var mipimpl = new List<MessageInfoPartInfoMessagePart>(message.PartCount);
            for (int partIndex = 0; partIndex < message.PartCount; partIndex++)
            {
                string partName = string.Empty;
                var mp = message.GetPartByIndex(partIndex, out partName);

                var miciapae = new MessageInfoContextInfoArrayPropertyArrayElement1 {Value = partName};
                miciapael.Add(miciapae);

                var mipimp = new MessageInfoPartInfoMessagePart {Charset = mp.Charset, ContentType = mp.ContentType};
                if (null != destination)
                {
                    mipimp.FileName = destination;
                }
                mipimp.ID = mp.PartID.ToString();
                mipimp.Name = partName;
                mipimpl.Add(mipimp);
            }

            var micipl = new List<MessageInfoContextInfoProperty>(mc.Properties.Count);
            foreach (DictionaryEntry pde in mc.Properties)
            {
                string key = pde.Key.ToString();
                string val = pde.Value.ToString();
                int at = key.IndexOf('@');

                var micip = new MessageInfoContextInfoProperty
                                {
                                    Name = key.Substring(0, at),
                                    Namespace = key.Substring(at + 1),
                                    Value = val
                                };
                micip.Promoted = mc.IsPromoted(micip.Name, micip.Namespace);
                micip.PromotedSpecified = true;

                micipl.Add(micip);
            }

            var miciap = new MessageInfoContextInfoArrayProperty
                             {
                                 Name = "PartNames",
                                 Namespace = "http://schemas.microsoft.com/BizTalk/2003/messageagent-properties",
                                 ArrayElement1 = miciapael.ToArray()
                             };

            var mici = new MessageInfoContextInfo
                           {
                               PropertiesCount = message.Context.CountProperties.ToString(),
                               ArrayProperty = new MessageInfoContextInfoArrayProperty[] {miciap},
                               Property = micipl.ToArray()
                           };

            var mipi = new MessageInfoPartInfo
                           {
                               PartsCount = message.PartCount.ToString(),
                               MessagePart = mipimpl.ToArray()
                           };

            var items = new ArrayList {mici, mipi};

            var mi = new MessageInfo {Items = items.ToArray()};
            return mi;
        }
    }
}
