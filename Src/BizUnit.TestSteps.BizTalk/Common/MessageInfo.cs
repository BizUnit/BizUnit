//---------------------------------------------------------------------
// File: MessageInfo.cs
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

using System.Xml;
using System.Xml.Serialization;
using Microsoft.BizTalk.Message.Interop;
using BizUnit.TestSteps.BizTalk.Map;

namespace BizUnit.TestSteps.BizTalk
{
    public partial class MessageInfo
    {
        public void MergeIntoMessage(IBaseMessage message)
        {
            foreach (MessageInfoContextInfoProperty prop in this.MessageContextProperties)
            {
                if (prop.Promoted)
                {
                    message.Context.Promote(prop.Name, prop.Namespace, prop.Value);
                }
                else
                {
                    message.Context.Write(prop.Name, prop.Namespace, prop.Value);
                }
            }
        }

        public MessageInfoContextInfoProperty[] MessageContextProperties
        {
            get
            {
                if (null != this.MessageInfoContextInfo)
                {
                    return this.MessageInfoContextInfo.Property;
                }
                return null;
            }
        }

        private MessageInfoContextInfo _mici = null;
        public MessageInfoContextInfo MessageInfoContextInfo
        {
            get
            {
                if (null == _mici)
                {
                    bool found = false;
                    int index = 0;
                    while (!found && (index < this.Items.Length))
                    {
                        _mici = this.Items[index] as MessageInfoContextInfo;
                        found = (null == _mici) ? false : true;
                        index++;
                    }
                }
                return _mici;
            }
        }

        private MessageInfoPartInfo _mipi = null;
        public MessageInfoPartInfo MessageInfoPartInfo
        {
            get
            {
                if (null == _mipi)
                {
                    bool found = false;
                    int index = 0;
                    while (!found && (index < this.Items.Length))
                    {
                        _mipi = this.Items[index] as MessageInfoPartInfo;
                        found = (null == _mipi) ? false : true;
                        index++;
                    }
                }
                return _mipi;
            }
        }

        private static XmlSerializer messageInfoSerializer = new XmlSerializer(typeof(MessageInfo));
        public static MessageInfo Deserialize(string path)
        {
            object obj = null;
            using (XmlReader reader = XmlReader.Create(path))
            {
                obj = messageInfoSerializer.Deserialize(reader);
            }

            return obj as MessageInfo;
        }

        public static void Serialize(MessageInfo mi, string path)
        {
            using (XmlWriter writer = XmlWriter.Create(path, BizTalkMapTester.WriterSettings))
            {
                messageInfoSerializer.Serialize(writer, mi);
            }
        }
    }
}
