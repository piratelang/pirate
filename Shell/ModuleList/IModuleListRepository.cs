using Common.Interfaces;

namespace Shell.ModuleList;

public interface IModuleListRepository
{
    List<Module> GetList(string location, ILogger logger);
    void SetList(string[] foundFiles, string location, ILogger logger);
}
