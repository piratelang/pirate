using System.Xml.Serialization;

namespace Shell.Project.Models;

[XmlRoot(ElementName = "Project")]
public class ProjectFile
{
    [XmlElement(ElementName = "PropertyGroup")]
    public PropertyGroup PropertyGroup { get; set; }

    [XmlElement(ElementName = "ItemGroup")]
    public List<ItemGroup>? ItemGroup { get; set; }


    public bool Validate()
    {
        if (PropertyGroup == null) return false;
        if (PropertyGroup.ProjectName == null) return false;
        if (PropertyGroup.TargetFramework == null) return false;

        return true;
    }
}