namespace PirateLang.Models;

public class HelpOption
{
    public string Description { get; set; }
    public string Usage { get; set; }
    public List<OptionDescription> Options { get; set; }

    public HelpOption(string description, string usage, List<OptionDescription> options)
    {
        Description = description;
        Usage = usage;
        Options = options;
    }

    public override string ToString()
    {
        return Environment.NewLine + string.Join(
            Environment.NewLine,
            "Description",
            $"  {Description}",
            "",
            "Usage",
            $"  {Usage}",
            "",
            "Options",
            $"  {string.Join(Environment.NewLine, Options.Select(o => o.ToString()))}"
        );
    }
}
