﻿using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core.Enrichers;
using Serilog.Events;
using Serilog.Exceptions;

namespace Stubbl.Identity.MvcClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "stubbl-identity-mvcclient";

            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .CaptureStartupErrors(true)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseKestrel()
                .UseIISIntegration()
                .UseSerilog((hostingContext, loggerConfiguration) =>
                {
                    loggerConfiguration.MinimumLevel
                        .Is(hostingContext.Configuration.GetValue<LogEventLevel>("Serilog:LogEventLevel"))
                        .Enrich.FromLogContext()
                        .Enrich.With(new PropertyEnricher("Component", "stubbl-identity-mvcclient"))
                        .Enrich.With(new PropertyEnricher("Environment", hostingContext.HostingEnvironment.EnvironmentName))
                        .Enrich.WithExceptionDetails()
                        .WriteTo.Console(hostingContext.Configuration.GetValue<LogEventLevel>("Serilog:LogEventLevel"));

                    var seqUrl = hostingContext.Configuration.GetValue<string>("Seq:Url");

                    if (!string.IsNullOrWhiteSpace(seqUrl))
                    {
                        loggerConfiguration.WriteTo.Seq
                        (
                            seqUrl,
                            apiKey: hostingContext.Configuration.GetValue<string>("Seq:ApiKey"),
                            restrictedToMinimumLevel: hostingContext.Configuration.GetValue<LogEventLevel>(
                                "Serilog:LogEventLevel")
                        );
                    }
                })
                .UseStartup<Startup>()
                .Build();
        }
    }
}