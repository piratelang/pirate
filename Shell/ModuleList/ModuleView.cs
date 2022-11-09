namespace Shell.ModuleList;

public class Module
{
    public string moduleName { get; set; }
    public string path { get; set; }
    public DateTime lastModifiedDate { get; set; }

    public Module(string ModuleName, string Path, DateTime LastModifiedDate)
    {
        moduleName = ModuleName;
        path = Path;
        lastModifiedDate = LastModifiedDate;
    }
}