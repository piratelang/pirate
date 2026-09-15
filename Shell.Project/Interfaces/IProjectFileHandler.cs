using Shell.Project.Models;

namespace Shell.Project.Interfaces
{
    public interface IProjectFileHandler
    {
        ProjectFile? ReadProjectFile(string name, string path);
        void WriteProjectFile(string path, ProjectFile project);
    }
}