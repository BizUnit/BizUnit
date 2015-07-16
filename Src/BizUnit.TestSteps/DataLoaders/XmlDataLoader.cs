
namespace BizUnitCoreTestSteps
{
    using System;
    using System.Xml;
    using System.IO;
    using System.Xml.XPath;
    using System.Collections.ObjectModel;
    using BizUnit;
    using BizUnitCoreTestSteps.Common;

    public class XmlDataLoader : DataLoaderBase
    {
        public string FilePath { get; set; }
        public Collection<XPathDefinition> _updateXml = new Collection<XPathDefinition>();

        public Collection<XPathDefinition> UpdateXml
        {
            get
            {
                return _updateXml;
            }

            set
            {
                _updateXml = value;
            }
        }

        public override Stream Load(Context context)
        {
            var doc = new XmlDocument();
            context.LogInfo("Loading file: {0}", FilePath);
            doc.Load(FilePath);

            if (null != UpdateXml)
            {
                foreach (var xpath in UpdateXml)
                {
                    context.LogInfo("Selecting node in document, description: {0}, XPath: {1}", xpath.Description, xpath.XPath);
                    XPathNavigator xpn = doc.CreateNavigator();
                    XPathNavigator node = xpn.SelectSingleNode(xpath.XPath);

                    if (null == node)
                    {
                        context.LogError("XPath expression failed to find node");
                        throw new ApplicationException(String.Format("Node not found: {0}", xpath.Description));
                    }

                    if (!string.IsNullOrEmpty(xpath.ContextKey))
                    {
                        context.LogInfo("Updating XmlNode with value from context key: {0}", xpath.ContextKey);
                        node.SetValue(context.GetValue(xpath.ContextKey));
                    }
                    else
                    {
                        context.LogInfo("Updating XmlNode with value: {0}", xpath.Value);
                        node.SetValue(xpath.Value);
                    }
                }
            }

            MemoryStream ms = new MemoryStream();
            doc.Save(ms);
            ms.Seek(0, SeekOrigin.Begin);

            return ms;
        }
    }
}
