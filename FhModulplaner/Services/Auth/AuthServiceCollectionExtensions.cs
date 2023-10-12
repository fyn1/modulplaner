using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace FhModulplaner.Services.Auth;

public static class AuthServiceCollectionExtensions
{
    public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("unique_name");

        services.Configure<AuthOptions>(configuration.GetSection(AuthOptions.AuthenticationSectionName));

        services.Configure<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme,
            options => { options.AccessDeniedPath = "/AccessDenied"; });

        services.AddOptions<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme)
            .Configure<IOptions<AuthOptions>>((options, authOptions) =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.ResponseType = "code";

                options.Authority = authOptions.Value.Authority;
                options.ClientId = authOptions.Value.ClientId;
                options.ClientSecret = authOptions.Value.ClientSecret;

                options.EventsType = typeof(OpenIdConnectEventHandler);
            });

        services.AddScoped<OpenIdConnectEventHandler>();

       services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddOpenIdConnect();

       services.AddAuthorization();

        // services.AddAuthorization(options =>
        // {
        //     options.DefaultPolicy = new AuthorizationPolicyBuilder()
        //         .RequireAuthenticatedUser()
        //         .RequireClaim(AppClaimTypes.Role)
        //         .Build();
        //
        //     options.FallbackPolicy = options.DefaultPolicy;
        //
        //     options.AddPolicy(AppPolicies.AzubiPolicy, p => p
        //         .RequireAuthenticatedUser()
        //         .RequireClaim(AppClaimTypes.Role, UserRoleName.Azubi.ToString()));
        //
        //     options.AddPolicy(AppPolicies.AdminPolicy, p => p
        //         .RequireAuthenticatedUser()
        //         .RequireClaim(AppClaimTypes.Role, UserRoleName.Admin.ToString()));
        //
        //     options.AddPolicy(AppPolicies.AusbilderPolicy, p => p
        //         .RequireAuthenticatedUser()
        //         .RequireClaim(AppClaimTypes.Role, UserRoleName.Ausbilder.ToString()));
        // });

        return services;
    }
}