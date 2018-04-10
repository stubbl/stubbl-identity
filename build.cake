var target = Argument("target", "Default");
var configuration = Argument("configuration", EnvironmentVariable("CONFIGURATION") ?? "Release");
var artifactsDirectory = @".\artifacts";
var version = EnvironmentVariable("APPVEYOR_BUILD_VERSION") ?? "0.0.0";

Task("Clean")
    .Does(() =>
    {
        CleanDirectories(artifactsDirectory);

        StartAndReturnProcess("dotnet", new ProcessSettings
            {
                Arguments = "clean"
            })
            .WaitForExit();
    });

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        DotNetCoreRestore();
    });

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        StartAndReturnProcess("dotnet", new ProcessSettings
            {
                Arguments = $"build --configuration {configuration} --no-restore"
            })
            .WaitForExit();
    });

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
    {
        foreach (var filePath in GetFiles(@".\test\**\*.csproj")) 
        { 
            if (AppVeyor.IsRunningOnAppVeyor)
            {
                StartAndReturnProcess("dotnet", new ProcessSettings
                    {
                        Arguments = $"test {filePath} --configuration {configuration} --logger:AppVeyor --no-build --no-restore"
                    })
                    .WaitForExit();
            }
            else
            {
                StartAndReturnProcess("dotnet", new ProcessSettings
                    {
                        Arguments = $"test {filePath} --configuration {configuration} --logger:nunit --no-build --no-restore"
                    })
                    .WaitForExit();
            }
        }
    });

Task("Publish")
    .IsDependentOn("Test")
    .Does(() => 
    {
        StartAndReturnProcess("dotnet", new ProcessSettings
            {
                Arguments = $@"publish src\Stubbl.Identity --configuration {configuration} --no-restore /p:Version={version}"
            })
            .WaitForExit();
    });

Task("Pack")
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