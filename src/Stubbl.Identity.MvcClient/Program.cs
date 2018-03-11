namespace Stubbl.Identity.MvcClient
{
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Serilog;
    using Serilog.Core.Enrichers;
    using Serilog.Events;
    using Serilog.Exceptions;
    using System;
    using System.IO;

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "stubbl-identity-mvcclient";

            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) => WebHost.CreateDefaultBuilder(args)
            .CaptureStartupErrors(true)
            .UseContentRoot(Directory.GetCurrentDirectory())
            .UseKestrel()
            .UseIISIntegration()
            .UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration.MinimumLevel.Is(hostingContext.Configuration.GetValue<LogEventLevel>("Serilog:LogEventLevel"))
                .Enrich.FromLogContext()
                .Enrich.With(new PropertyEnricher("Component", "stubbl-identity-mvcclient"))
                .Enrich.With(new PropertyEnricher("Environment", hostingContext.HostingEnvironment.EnvironmentName))
                .Enrich.WithExceptionDetails()
                .WriteTo.Console(restrictedToMinimumLevel: hostingContext.Configuration.GetValue<LogEventLevel>("Serilog:LogEventLevel"))
                .WriteTo.Seq
                (
                    hostingContext.Configuration.GetValue<string>("Seq:Url"),
                    apiKey: hostingContext.Configuration.GetValue<string>("Seq:ApiKey"),
                    restrictedToMinimumLevel: hostingContext.Configuration.GetValue<LogEventLevel>("Serilog:LogEventLevel")
                )
            )
           .UseStartup<Startup>()
           .Build();
    }
}
