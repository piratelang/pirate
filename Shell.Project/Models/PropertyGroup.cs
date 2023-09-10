using System.Xml.Serialization;

namespace Shell.Project.Models;

[XmlRoot(ElementName = "PropertyGroup")]
public class PropertyGroup
{

    [XmlElement(ElementName = "TargetFramework")]
    public string TargetFramework { get; set; }
}
