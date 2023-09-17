// See https://aka.ms/new-console-template for more information

using Pirate.Common.FileHandler;
using Shell.Project;

var projectFileHandler = new ProjectFileHandler(new FileReadHandler(), new FileWriteHandler());

var project = await projectFileHandler.ReadProjectFile("test", "");

Console.WriteLine($"Hello, {project.PropertyGroup.TargetFramework}!");
project.ItemGroup.Modules.ForEach(module =>
{
    Console.WriteLine($"Module: {module.File} - EntryPoint: {module.EntryPoint}");
});