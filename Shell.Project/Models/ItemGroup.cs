using System.Xml.Serialization;

namespace Pirate.Build.Project.Models;

[XmlRoot(ElementName = "ItemGroup")]
public class ItemGroup
{

    [XmlElement(ElementName = "Module")]
    public List<Module>? Modules { get; set; }
}
