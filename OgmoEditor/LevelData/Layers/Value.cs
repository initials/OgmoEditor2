using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OgmoEditor.Definitions.ValueDefinitions;
using System.Xml;

namespace OgmoEditor.LevelData.Layers
{
    public class Value
    {
        public ValueDefinition Definition { get; private set; }
        public string Content;

        public Value(ValueDefinition definition)
        {
            Definition = definition;
            Content = definition.GetDefault();
        }

        public Value(Value value)
        {
            Definition = value.Definition;
            Content = value.Content;
        }

        public XmlAttribute GetXML(XmlDocument doc)
        {
            XmlAttribute xml = doc.CreateAttribute(Definition.Name);
            xml.InnerText = Content;
            return xml;
        }

        public void SetXML(XmlAttribute xml)
        {
            Content = xml.InnerText;
        }
    }
}
