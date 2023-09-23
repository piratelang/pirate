namespace PirateLang.Models;

public class OptionDescription
{
    public List<string> Options { get; set; }
    public string Description { get; set; }

    public OptionDescription(List<string> options, string description)
    {
        Options = options;
        Description = description;
    }

    public override string ToString()
    {
        return $"{string.Join(", ", Options)}   {Description}";
    }
}