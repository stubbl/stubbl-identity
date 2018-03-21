var target = Argument("target", "Default");

var configuration = Argument("configuration", EnvironmentVariable("CONFIGURATION") ?? "Release");

var artifactsDirectory = @".\artifacts";

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
        var version = EnvironmentVariable("APPVEYOR_BUILD_VERSION") ?? "0.0.0";

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

        var artifactFilePath = $@"{artifactsDirectory}\stubbl-identity.zip";
        
        Zip($@".\src\Stubbl.Identity\bin\{configuration}\netcoreapp2.0\publish\", artifactFilePath); 
        
        if (AppVeyor.IsRunningOnAppVeyor)
        {
            AppVeyor.UploadArtifact(artifactFilePath, new AppVeyorUploadArtifactsSettings
            {
                DeploymentName = "stubbl-identity"
            });
        }
    });

Task("Default")
    .IsDependentOn("Pack");

RunTarget(target);