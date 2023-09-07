namespace Shell.ModuleList.interfaces;

public interface IModuleListRepository
{
    List<Module> GetList(string location);
    void SetList(string[] foundFiles, string location);
}
