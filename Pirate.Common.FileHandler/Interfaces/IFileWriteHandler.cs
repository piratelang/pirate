using Pirate.Common.FileHandler;
using Pirate.Common.FileHandler.Model;

namespace Pirate.Common.FileHandler.Interfaces;

/// <inheritdoc cref="FileWriteHandler"/>
public interface IFileWriteHandler
{
    bool AppendToFile(FileWriteModel fileWriteModel);
    bool WriteToFile(FileWriteModel fileWriteModel);
}
