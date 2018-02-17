var target = Argument("target", "Default");
var configuration = Argument("configuration", EnvironmentVariable("APPVEYOR_BUILD_VERSION") ?? "Release");
var artifactsDirectory = "./artifacts";
var publishDirectory = "./publish";

Task("Clean")
   .Does(() =>
   {
      var deleteDirectorySettings = new DeleteDirectorySettings
      {
          Force = true,
          Recursive = true
      };

      if (DirectoryExists(artifactsDirectory))
      {
         Console.Write("Cleaning artifacts directory...");

         DeleteDirectory(artifactsDirectory, deleteDirectorySettings);
         
         Console.ForegroundColor = ConsoleColor.Green;
         Console.Write("DONE");
         Console.ResetColor();
         Console.WriteLine("");
      }

      if (DirectoryExists(publishDirectory))
      {
         Console.Write("Cleaning publish directory...");

         DeleteDirectory(publishDirectory, deleteDirectorySettings);
         
         Console.ForegroundColor = ConsoleColor.Green;
         Console.Write("DONE");
         Console.ResetColor();
         Console.WriteLine("");
      }
   });

Task("Version")
   .Does(() =>
   {
      var assemblyVersion = "1.0.0.0";
      var fileVersion = "1.0.0.0";
      var version = "1.0.0";
      var appVeyorBuildVersion = EnvironmentVariable("APPVEYOR_BUILD_VERSION");

      if (!string.IsNullOrWhiteSpace(appVeyorBuildVersion))
      {
         version = appVeyorBuildVersion;

         var versionMatch = System.Text.RegularExpressions.Regex.Match(version, @"(\d+\.\d+\.\d+(?:\.\d+)?).*");

         if (versionMatch.Success)
         {
            assemblyVersion = versionMatch.Groups[1].Value;
            fileVersion = versionMatch.Groups[1].Value;
         }
      }

      Console.WriteLine($"Assembly Version: {assemblyVersion}");
      Console.WriteLine($"File Version: {fileVersion}");
      Console.WriteLine($"Version: {version}");

      foreach (var filePath in GetFiles(@".\src\**\*.csproj"))
      {
         var text = System.IO.File.ReadAllText(filePath.FullPath);
         text = System.Text.RegularExpressions.Regex.Replace(text, @"<AssemblyVersion>[^<]+<\/AssemblyVersion>", $"<AssemblyVersion>{assemblyVersion}</AssemblyVersion>");
         text = System.Text.RegularExpressions.Regex.Replace(text, @"<FileVersion>[^<]+<\/FileVersion>", $"<FileVersion>{fileVersion}</FileVersion>");
         text = System.Text.RegularExpressions.Regex.Replace(text, @"<Version>[^<]+<\/Version>", $"<Version>{version}</Version>");
         
         System.IO.File.WriteAllText(filePath.FullPath, text);
      }
   });

Task("Build")
   .IsDependentOn("Clean")
   .IsDependentOn("Version")
   .Does(() =>
   {
      foreach (var filePath in GetFiles(@".\**\*.csproj"))
      {
         Console.WriteLine($"Building project {filePath.GetFilename()}...");

         Console.WriteLine("");

         DotNetCoreBuild(filePath.FullPath, new DotNetCoreBuildSettings
         {
            Configuration = configuration
         });
      }
   });

Task("Test")
   .IsDependentOn("Build")
   .Does(() => {
      foreach(var filePath in GetFiles(@".\test\**\*.csproj"))
      {
         Console.WriteLine($"Testing project {filePath.GetFilename()}...");
         Console.WriteLine("");

         DotNetCoreTest(filePath.FullPath, new DotNetCoreTestSettings
         {
            Configuration = configuration,
            Logger = "trx;LogFileName=TestResult.xml"
         });
      }

      if (AppVeyor.IsRunningOnAppVeyor)
      {
         foreach(var filePath in GetFiles(@".\test\**\TestResult.xml"))
         {
            AppVeyor.UploadTestResults(filePath.FullPath, AppVeyorTestResultsType.NUnit);
         }
      }
   });

Task("Pack")
   .IsDependentOn("Test")
   .Does(() => 
   {
      CreateDirectory(publishDirectory);

      var projectName = "Stubbl.Api";
      var projectFilename = $"{projectName}.csproj";
      var projectFilePath = $"./src/{projectName}/{projectFilename }";

      Console.WriteLine($"Publishing project {projectFilename}...");
      Console.WriteLine("");

      DotNetCorePublish(projectFilePath, new DotNetCorePublishSettings
      {
         Configuration = configuration,
         OutputDirectory = publishDirectory
      });
      
      Console.WriteLine("");

      CreateDirectory(artifactsDirectory);

      var artifactFilename = $"stubbl-identity.zip";
      var artifactFilePath = $"{artifactsDirectory}/{artifactFilename}";

      Console.Write($"Creating artifact {artifactFilename}...");
      
      Zip(publishDirectory, artifactFilePath);
      
      Console.ForegroundColor = ConsoleColor.Green;
      Console.Write("DONE");
      Console.ResetColor();
      Console.WriteLine("");

      if (AppVeyor.IsRunningOnAppVeyor)
      {
         foreach (var filePath in GetFiles(artifactsDirectory + "/**/*"))
         {
            AppVeyor.UploadArtifact(filePath.FullPath, new AppVeyorUploadArtifactsSettings
            {
               DeploymentName = "stubbl-identity"
            });
         }
      }
   });

Task("Default")
   .IsDependentOn("Pack");

RunTarget(target);