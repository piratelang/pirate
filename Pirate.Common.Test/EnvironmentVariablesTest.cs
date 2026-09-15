using System;
using System.IO;
using Pirate.Common.Errors;
using Pirate.Common.FileHandler;
using Xunit;

namespace Pirate.Common.Test;

public class EnvironmentVariablesTest
{
    [Fact]
    public void ShouldCreateVariablesJsonInUserConfigDirectory()
    {
        var originalDirectory = Directory.GetCurrentDirectory();
        var originalXdgConfigHome = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
        var tempWorkingDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var tempConfigHome = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        try
        {
            Directory.CreateDirectory(tempWorkingDirectory);
            Directory.CreateDirectory(tempConfigHome);
            Directory.SetCurrentDirectory(tempWorkingDirectory);
            Environment.SetEnvironmentVariable("XDG_CONFIG_HOME", tempConfigHome);

            var environmentVariables = new EnvironmentVariables(new FileReadHandler(), new FileWriteHandler());

            var expectedConfigPath = Path.Combine(tempConfigHome, "pirate", "variables.json");

            Assert.True(File.Exists(expectedConfigPath));
            Assert.False(Directory.Exists(Path.Combine(tempWorkingDirectory, "bin")));
            Assert.Equal("1.1.0", environmentVariables.GetVariable("version"));
        }
        finally
        {
            Directory.SetCurrentDirectory(originalDirectory);
            Environment.SetEnvironmentVariable("XDG_CONFIG_HOME", originalXdgConfigHome);
            if (Directory.Exists(tempWorkingDirectory)) Directory.Delete(tempWorkingDirectory, true);
            if (Directory.Exists(tempConfigHome)) Directory.Delete(tempConfigHome, true);
        }
    }

    [Fact]
    public void ShouldThrowFileExceptionWhenVariableIsMissing()
    {
        var originalDirectory = Directory.GetCurrentDirectory();
        var originalXdgConfigHome = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
        var tempWorkingDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var tempConfigHome = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        try
        {
            Directory.CreateDirectory(tempWorkingDirectory);
            Directory.CreateDirectory(tempConfigHome);
            Directory.SetCurrentDirectory(tempWorkingDirectory);
            Environment.SetEnvironmentVariable("XDG_CONFIG_HOME", tempConfigHome);

            var environmentVariables = new EnvironmentVariables(new FileReadHandler(), new FileWriteHandler());

            Assert.Throws<FileException>(() => environmentVariables.GetVariable("unknown-variable"));
        }
        finally
        {
            Directory.SetCurrentDirectory(originalDirectory);
            Environment.SetEnvironmentVariable("XDG_CONFIG_HOME", originalXdgConfigHome);
            if (Directory.Exists(tempWorkingDirectory)) Directory.Delete(tempWorkingDirectory, true);
            if (Directory.Exists(tempConfigHome)) Directory.Delete(tempConfigHome, true);
        }
    }

    [Fact]
    public void ShouldRefreshExistingVariablesTemplate()
    {
        var originalDirectory = Directory.GetCurrentDirectory();
        var originalXdgConfigHome = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
        var tempWorkingDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var tempConfigHome = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        try
        {
            Directory.CreateDirectory(tempWorkingDirectory);
            Directory.CreateDirectory(tempConfigHome);
            Directory.SetCurrentDirectory(tempWorkingDirectory);
            Environment.SetEnvironmentVariable("XDG_CONFIG_HOME", tempConfigHome);

            var configDirectory = Path.Combine(tempConfigHome, "pirate");
            Directory.CreateDirectory(configDirectory);
            File.WriteAllText(Path.Combine(configDirectory, "variables.json"), "{\"version\":\"0.0.1\",\"location\":\"old/path\"}");

            var environmentVariables = new EnvironmentVariables(new FileReadHandler(), new FileWriteHandler());

            Assert.Equal("1.1.0", environmentVariables.GetVariable("version"));
            Assert.Equal("bin/pirate1.1.0", environmentVariables.GetVariable("location"));
        }
        finally
        {
            Directory.SetCurrentDirectory(originalDirectory);
            Environment.SetEnvironmentVariable("XDG_CONFIG_HOME", originalXdgConfigHome);
            if (Directory.Exists(tempWorkingDirectory)) Directory.Delete(tempWorkingDirectory, true);
            if (Directory.Exists(tempConfigHome)) Directory.Delete(tempConfigHome, true);
        }
    }
}
