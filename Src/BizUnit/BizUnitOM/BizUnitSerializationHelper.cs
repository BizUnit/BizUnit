//---------------------------------------------------------------------
// File: BizUnitSerializationHelper.cs
// 
// Summary: 
//
//---------------------------------------------------------------------
// Copyright (c) 2004-2010, Kevin B. Smith. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------

namespace BizUnit
{
    using System.Text;
    using System.IO;
    using System.Xml;
    using System.Windows.Markup;

    public static class BizUnitSerializationHelper
    {
        public static void Serialize(object toSerialize, Stream stream)
        {
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = true;
            settings.ConformanceLevel = ConformanceLevel.Fragment;

            var sb = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(stream, settings);

            var manager = new XamlDesignerSerializationManager(writer);
            manager.XamlWriterMode = XamlWriterMode.Expression;
            XamlWriter.Save(toSerialize, manager);

            writer.Flush();
        }

        public static string Serialize(object toSerialize)
        {
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = true;
            settings.ConformanceLevel = ConformanceLevel.Fragment;

            var sb = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(sb, settings);

            var manager = new XamlDesignerSerializationManager(writer);
            manager.XamlWriterMode = XamlWriterMode.Expression;
            XamlWriter.Save(toSerialize, manager);

            return sb.ToString();
        }

        public static object Deserialize(string xamlText)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xamlText);

            return XamlReader.Load(new XmlNodeReader(doc));
        }
    }
}
