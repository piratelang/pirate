using System.Xml.Serialization;

namespace Shell.Project.Models;

[XmlRoot(ElementName = "Module")]
public class Module
{

    [XmlElement(ElementName = "EntryPoint")]
    public bool? EntryPoint { get; set; }

    [XmlAttribute(AttributeName = "File")]
    public string? File { get; set; }

}
