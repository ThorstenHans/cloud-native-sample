using AuthenticationService;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;
using Duende.IdentityServer;
using Microsoft.IdentityModel.Tokens;
using AuthenticationService.Configuration;

namespace AuthenticationService;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddRazorPages();

        var isBuilder = builder.Services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
 
                options.EmitStaticAudienceClaim = true;
            })
            .AddTestUsers(new List<Duende.IdentityServer.Test.TestUser> { });

        var cfg = builder.Configuration.GetSection("IdentityServer").Get<IdentityServerConfig>();
        // in-memory, code config
        isBuilder.AddInMemoryIdentityResources(IdentityResourcesProvider.GetAll);
        isBuilder.AddInMemoryApiScopes(ApiScopesProvider.GetAll);
        isBuilder.AddInMemoryClients(ClientsProvider.GetAll(cfg.InteractiveClient));

        builder.Services.AddAuthentication()
            .AddOpenIdConnect("azuread", "Sign-in with Azure AD", options =>
            {
                options.Authority = cfg.AzureAd.Authority; // "https://login.microsoftonline.com/common";
                options.ClientId = cfg.AzureAd.ClientId; // "b33fe330-1cca-4d71-a286-edabfa622a0b";
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                options.SignOutScheme = IdentityServerConstants.SignoutScheme;

                options.ResponseType = "id_token";
                options.CallbackPath = "/signin-aad";
                options.SignedOutCallbackPath = "/signout-callback-aad";
                options.RemoteSignOutPath = "/signout-aad";

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidAudience = cfg.AzureAd.ClientId, // "b33fe330-1cca-4d71-a286-edabfa622a0b",

                    NameClaimType = "name",
                    RoleClaimType = "role"
                };
            })
            .AddLocalApi(l => l.ExpectedScope = "sample");

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseStaticFiles();
        app.UseRouting();
        app.UseIdentityServer();
        app.UseAuthorization();

        app.MapRazorPages()
            .RequireAuthorization();

        return app;
    }
}