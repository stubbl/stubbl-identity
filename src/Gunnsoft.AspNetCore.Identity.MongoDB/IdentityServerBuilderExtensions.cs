using IdentityModel;
using IdentityServer4.AspNetIdentity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Gunnsoft.AspNetCore.Identity.MongoDB
{
    public static class IdentityServerBuilderExtensions
    {
        public static IIdentityServerBuilder AddAspNetIdentity<TUser, TUserClaimsPrincipalFactory>(this IIdentityServerBuilder extended)
            where TUser : class
            where TUserClaimsPrincipalFactory : class, IUserClaimsPrincipalFactory<TUser>
        {
            // The IdentityServer impementation of UserClaimsFactory _might_ add duplicate claims (email address and phone number).
            extended.Services.AddTransient<IUserClaimsPrincipalFactory<TUser>, TUserClaimsPrincipalFactory>();

            extended.Services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserIdClaimType = JwtClaimTypes.Subject;
                options.ClaimsIdentity.UserNameClaimType = JwtClaimTypes.Name;
                options.ClaimsIdentity.RoleClaimType = JwtClaimTypes.Role;
            });

            extended.Services.Configure<SecurityStampValidatorOptions>(opts =>
            {
                opts.OnRefreshingPrincipal = SecurityStampValidatorCallback.UpdatePrincipal;
            });

            extended.Services.Configure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme, cookie =>
            {
                cookie.Cookie.SameSite = SameSiteMode.None;
            });

            extended.AddResourceOwnerValidator<ResourceOwnerPasswordValidator<TUser>>();
            extended.AddProfileService<ProfileService<TUser>>();

            return extended;
        }
    }
}