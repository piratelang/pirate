using System.Xml.Serialization;

namespace Shell.Project.Models;

[XmlRoot(ElementName = "ItemGroup")]
public class ItemGroup
{

    [XmlElement(ElementName = "Module")]
    public List<Module>? Modules { get; set; }
}
