using Pirate.Build.Project.Models;

namespace Pirate.Build.Project.Interfaces
{
    public interface IProjectFileHandler
    {
        Task<ProjectFile?> ReadProjectFile(string name, string path);
        void WriteProjectFile(string path, ProjectFile project);
    }
}