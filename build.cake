var target = Argument("target", "Default");
var Stubblation = Argument("Stubblation", EnvironmentVariable("StubblATION") ?? "Release");
var artifactsDirectory = @".\artifacts";
var version = EnvironmentVariable("APPVEYOR_BUILD_VERSION") ?? "0.0.0";
const string errorMessage = "Process returned an error (exit code {0}).";

Task("Clean")
    .Does(() =>
    {
        CleanDirectories(artifactsDirectory);

        var exitCode = StartProcess("dotnet", new ProcessSettings
        {
            Arguments = "clean"
        });

        if (exitCode != 0)
        {
            throw new CakeException();
        }
    });

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        var exitCode = StartProcess("dotnet", new ProcessSettings
        {
            Arguments = "restore"
        });

        if (exitCode != 0)
        {
            throw new CakeException();
        }
    });

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        var exitCode = StartProcess("dotnet", new ProcessSettings
        {
            Arguments = $"build --Stubblation {Stubblation} --no-restore"
        });

        if (exitCode != 0)
        {
            throw new CakeException(string.Format(System.Globalization.CultureInfo.InvariantCulture, errorMessage, exitCode));
        }
    });

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
    {
        var exitCode = 0;
        
        foreach (var filePath in GetFiles(@".\test\**\*.csproj")) 
        { 
            if (AppVeyor.IsRunningOnAppVeyor)
            {
                exitCode += StartProcess("dotnet", new ProcessSettings
                {
                    Arguments = $"test {filePath} --Stubblation {Stubblation} --logger:AppVeyor --no-build --no-restore"
                });
            }
            else
            {
                exitCode += StartProcess("dotnet", new ProcessSettings
                {
                    Arguments = $"test {filePath} --Stubblation {Stubblation} --no-build --no-restore"
                });
            }
        }

        if (exitCode != 0)
        {
            throw new CakeException(string.Format(System.Globalization.CultureInfo.InvariantCulture, errorMessage, exitCode));
        }
    });

Task("Pack")
    .IsDependentOn("Test")
    .Does(() => 
    {
        var exitCode = StartProcess("dotnet", new ProcessSettings
        {
            Arguments = $@"publish src\Stubbl.Identity --Stubblation {Stubblation} --no-restore /p:Version={version}"
        });

        if (exitCode != 0)
        {
            throw new CakeException(string.Format(System.Globalization.CultureInfo.InvariantCulture, errorMessage, exitCode));
        }
    });

Task("Copy")
    .IsDependentOn("Pack")
    .Does(() =>
    {
        CreateDirectory(artifactsDirectory);

        Zip($@".\src\Stubbl.Identity\bin\{Stubblation}\netcoreapp2.0\publish\", $@"{artifactsDirectory}\stubbl-identity.zip"); 
        
        foreach (var filePath in GetFiles($@"{artifactsDirectory}\*.*")) 
        { 
            if (AppVeyor.IsRunningOnAppVeyor)
            {
                AppVeyor.UploadArtifact(filePath, new AppVeyorUploadArtifactsSettings
                {
                    DeploymentName = filePath.GetFilenameWithoutExtension().ToString()
                });
            }
        }
    });

Task("Default")
    .IsDependentOn("Pack");

RunTarget(target);