namespace Stubbl.Identity
{
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using CodeContrib.AspNetCore.Identity.MongoDB;
    using Microsoft.AspNetCore.Authentication.OAuth;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using MongoDB.Driver;
    using Newtonsoft.Json.Linq;
    using NWebsec.AspNetCore.Middleware;
    using Stubbl.Identity.Services;
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Reflection;
    using System.Security.Claims;

    public class Startup
    {
        private readonly static Assembly s_assembly;
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;

        static Startup()
        {
            s_assembly = typeof(Startup).GetTypeInfo().Assembly;
        }

        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        public IConfiguration Configuration { get; }

        public void Configure(IApplicationBuilder app)
        {
            if (_hostingEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            if (_hostingEnvironment.IsProduction())
            {
                app.UseHsts(o => o.MaxAge(days: 30).IncludeSubdomains());
            }

            Action<IFluentCspOptions> cspo = o =>
            {
                o.DefaultSources(s => s.Self());
                o.ImageSources(s => s.Self());
                o.FontSources(s => s.Self().CustomSources("data:")); // TODO Remove the `data:` source
                o.StyleSources(s => s.Self().CustomSources("maxcdn.bootstrapcdn.com"));

                o.ReportUris(r => r.Uris("/csp-report"));
            };

            if (_hostingEnvironment.IsDevelopment())
            {
                app.UseCspReportOnly(cspo);
            }
            else
            {
                app.UseCsp(cspo);
            }

            app.UseReferrerPolicy(o => o.NoReferrer());

            app.UseStaticFiles();

            app.UseRedirectValidation(o =>
            {
                o.AllowedDestinations
                (
                    "https://accounts.google.com",
                    "https://github.com",
                    "https://login.microsoftonline.com"
                );
            });

            if (_hostingEnvironment.IsProduction())
            {
                app.UseCsp(o => o.UpgradeInsecureRequests());
            }

            app.UseXfo(o => o.SameOrigin());
            app.UseXContentTypeOptions();
            app.UseXDownloadOptions();
            app.UseXRobotsTag(o => o.NoIndex().NoFollow());
            app.UseXXssProtection(o => o.EnabledWithBlockMode());

            app.UseIdentityServer();

            app.UseMvc();
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, ApplicationRole>(o =>
                {
                    o.Password.RequireDigit = false;
                    o.Password.RequireLowercase = false;
                    o.Password.RequireNonAlphanumeric = false;
                    o.Password.RequireUppercase = false;
                    o.Password.RequiredLength = 8;
                    o.Password.RequiredUniqueChars = 0;
                    o.SignIn.RequireConfirmedEmail = true;
                    o.Tokens.ChangePhoneNumberTokenProvider = "Phone";
                })
                .AddErrorDescriber<StubblIdentityErrorDescriber>()
                .AddMongoDBStores<ApplicationUser, ApplicationRole>(new MongoUrl(_configuration.GetValue<string>("MongoDB:ConnectionString")))
                .AddDefaultTokenProviders();

            services.AddIdentityServer(o =>
                {
                    o.UserInteraction.LoginUrl = "/login";
                })
                .AddAspNetIdentity<ApplicationUser>()
                .AddDeveloperSigningCredential()
                .AddInMemoryApiResources(IdentityServerConfig.GetApiResources())
                .AddInMemoryClients(IdentityServerConfig.GetClients(_configuration))
                .AddInMemoryIdentityResources(IdentityServerConfig.GetIdentityResources())
                .AddInMemoryPersistedGrants()
                .AddProfileService<StubblProfileService>();

            services.AddAuthentication()
                // TODO AddGitHub()
                .AddOAuth("GitHub", "GitHub", o =>
                {
                    var clientId = _configuration.GetValue<string>("GitHub:ClientId");
                    var clientSecret = _configuration.GetValue<string>("GitHub:ClientSecret");

                    o.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
                    o.CallbackPath = "/signin-oidc";
                    o.ClientId = clientId;
                    o.ClientSecret = clientSecret;
                    o.Events = new OAuthEvents
                    {
                        OnCreatingTicket = async context =>
                        {
                            var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
                            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                            var response = await context.Backchannel.SendAsync(request, context.HttpContext.RequestAborted);
                            response.EnsureSuccessStatusCode();

                            var json = await response.Content.ReadAsStringAsync();
                            var user = JObject.Parse(json);

                            var userId = user.Value<string>("id");

                            if (!string.IsNullOrWhiteSpace(userId))
                            {
                                context.Identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId, ClaimValueTypes.String, context.Options.ClaimsIssuer));
                            }

                            var name = user.Value<string>("name");

                            if (!string.IsNullOrWhiteSpace(name))
                            {
                                context.Identity.AddClaim(new Claim(ClaimTypes.Name, name, ClaimValueTypes.String, context.Options.ClaimsIssuer));
                            }

                            var emailAddress = user.Value<string>("email");

                            if (!string.IsNullOrWhiteSpace(emailAddress))
                            {
                                context.Identity.AddClaim(new Claim(ClaimTypes.Email, emailAddress, ClaimValueTypes.String, context.Options.ClaimsIssuer));
                            }
                        }
                    };
                    o.Scope.Add("user:email");
                    o.TokenEndpoint = "https://github.com/login/oauth/access_token";
                    o.UserInformationEndpoint = "https://api.github.com/user";
                })
                .AddGoogle(o =>
                {
                    var clientId = _configuration.GetValue<string>("Google:ClientId");
                    var clientSecret = _configuration.GetValue<string>("Google:ClientSecret");

                    o.ClientId = clientId;
                    o.ClientSecret = clientSecret;
                })
                .AddMicrosoftAccount(o =>
                {
                    var clientId = _configuration.GetValue<string>("Microsoft:ClientId");
                    var clientSecret = _configuration.GetValue<string>("Microsoft:ClientSecret");

                    o.ClientId = clientId;
                    o.ClientSecret = clientSecret;
                });

            services.AddMvc();

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterInstance(_configuration)
               .As<IConfiguration>()
               .SingleInstance();
            containerBuilder.RegisterAssemblyModules(s_assembly);
            containerBuilder.Populate(services);
            var container = containerBuilder.Build();
            var serviceProvider = new AutofacServiceProvider(container);

            return serviceProvider;
        }
    }
}
