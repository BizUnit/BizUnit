//---------------------------------------------------------------------
// File: BizTalkMapTester.cs
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

using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.XLANGs.BaseTypes;
using BizUnit.Core.Common;

namespace BizUnit.TestSteps.BizTalk.Map
{
    /// <summary>
    /// Helper class to execute BizTalk maps
    /// </summary>
    public class BizTalkMapTester
    {
        internal static XmlWriterSettings WriterSettings = new XmlWriterSettings();
        private readonly TransformBase _map;

        static BizTalkMapTester()
        {
            WriterSettings.Encoding = Encoding.UTF8;
            WriterSettings.Indent = true;
            WriterSettings.OmitXmlDeclaration = true;
        }

        /// <summary>
        /// Constructor for helper class to execute BizTalk maps
        /// </summary>
        /// <param name='mapType'>The type of the BizTalk map to execute</param>
        public BizTalkMapTester(Type mapType)
        {
            _map = CreateMapFromType(mapType);
        }

        private static TransformBase CreateMapFromType(Type mapType)
        {
            ArgumentValidation.CheckForNullReference(mapType, "mapType");

            if (!mapType.IsSubclassOf(typeof(TransformBase)))
            {
                throw new InvalidOperationException("Type must specify a BizTalk map");
            }

            return Activator.CreateInstance(mapType) as TransformBase;
        }

        public TransformBase Map
        {
            get { return _map; }
        }

        /// <summary>
        /// Execute the map
        /// </summary>
        /// <param name='source'>The input Xml instance to map</param>
        /// <param name='destination'>The ouput Xml instance produced by the map</param>
        public void Execute(string source, string destination)
        {
            using (var inReader = new StreamReader(source))
            {
                using(FileStream fs = System.IO.File.OpenWrite(destination))
                using (var xwOut = XmlWriter.Create(fs, WriterSettings))
                {
                    var xpathdoc = new XPathDocument(inReader);
                    Map.Transform.Transform(xpathdoc, Map.TransformArgs, xwOut);
                    xwOut.Flush();
                }
            }
        }
    }
}
