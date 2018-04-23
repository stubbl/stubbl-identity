var target = Argument("target", "Default");
var configuration = Argument("configuration", EnvironmentVariable("configuration") ?? "Release");
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
            Arguments = $"build --configuration {configuration} --no-restore"
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
                    Arguments = $"test {filePath} --configuration {configuration} --logger:AppVeyor --no-build --no-restore"
                });
            }
            else
            {
                exitCode += StartProcess("dotnet", new ProcessSettings
                {
                    Arguments = $"test {filePath} --configuration {configuration} --no-build --no-restore"
                });
            }
        }

        if (exitCode != 0)
        {
            throw new CakeException(string.Format(System.Globalization.CultureInfo.InvariantCulture, errorMessage, exitCode));
        }
    });

Task("Publish")
    .IsDependentOn("Test")
    .Does(() => 
    {
        var exitCode = StartProcess("dotnet", new ProcessSettings
        {
            Arguments = $@"publish src\Stubbl.Identity --configuration {configuration} --no-restore /p:Version={version}"
        });

        if (exitCode != 0)
        {
            throw new CakeException(string.Format(System.Globalization.CultureInfo.InvariantCulture, errorMessage, exitCode));
        }
    });

Task("Copy")
    .IsDependentOn("Publish")
    .Does(() =>
    {
        CreateDirectory(artifactsDirectory);

        Zip($@".\src\Stubbl.Identity\bin\{configuration}\netcoreapp2.0\publish\", $@"{artifactsDirectory}\stubbl-identity.zip"); 
        
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