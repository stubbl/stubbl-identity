using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Stubbl.Identity.MvcClient
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            app.UseAuthentication();

            app.UseMvc();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(o =>
                {
                    o.DefaultScheme = "Cookies";
                    o.DefaultChallengeScheme = "oidc";
                })
                .AddCookie("Cookies")
                .AddOpenIdConnect("oidc", o =>
                {
                    o.Authority = "http://localhost:51794";
                    o.ClaimActions.Add(new JsonKeyClaimAction("email_verified", "email_verified", "email_verified"));
                    o.ClaimActions.Add(new JsonKeyClaimAction("preferred_username", "preferred_username", "preferred_username"));
                    o.ClientId = "stubbl-identity-mvc-client";
                    o.ClientSecret = "secret";
                    o.GetClaimsFromUserInfoEndpoint = true;
                    o.ResponseType = "code id_token";
                    o.RequireHttpsMetadata = false;
                    o.SaveTokens = true;
                    o.Scope.Add("email");
                    o.Scope.Add("profile");
                    o.Scope.Add("offline_access");
                    o.Scope.Add("openid");
                    o.SignInScheme = "Cookies";
                });
        }
    }
}