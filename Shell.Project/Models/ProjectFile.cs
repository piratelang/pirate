using System.Xml.Serialization;

namespace Shell.Project.Models;

[XmlRoot(ElementName = "Project")]
public class ProjectFile
{

    [XmlElement(ElementName = "PropertyGroup")]
    public PropertyGroup? PropertyGroup { get; set; }

    [XmlElement(ElementName = "ItemGroup")]
    public ItemGroup? ItemGroup { get; set; }
}