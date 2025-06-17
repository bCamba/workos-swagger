using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace API;

public static class AuthConfiguration
{
    public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var clientId = configuration["ClientId"];
        var apiKey = configuration["ApiKey"];
        WorkOS.WorkOS.SetApiKey(apiKey);

                    services.AddAuthentication("bearer")
            .AddJwtBearer(
                "bearer",
                options =>
                {
                    var openIdConfig = new OpenIdConnectConfiguration
                    {
                        Issuer = "https://api.workos.com"
                    };

                    options.Configuration = openIdConfig;
                    options.Audience = clientId;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = "https://api.workos.com",
                        ValidateAudience = false, // Ideally I would also like to be able to set this to true, but I couldn't make it work with WorkOS
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var cache = context.HttpContext.RequestServices.GetRequiredService<IMemoryCache>();

                            options.TokenValidationParameters.IssuerSigningKeyResolver = (token, securityToken, kid, validationParameters) =>
                            {
                                string cacheKey = $"jwks_keys_{clientId}";
                                if (!cache.TryGetValue(cacheKey, out IList<SecurityKey>? signingKeys))
                                {
                                    using var httpClient = new HttpClient();
                                    var jwksJson = httpClient.GetStringAsync($"https://api.workos.com/sso/jwks/{clientId}").Result;
                                    var jsonWebKeySet = new JsonWebKeySet(jwksJson);
                                    signingKeys = jsonWebKeySet.GetSigningKeys();

                                    cache.Set(cacheKey, signingKeys, TimeSpan.FromHours(24));
                                }

                                return signingKeys;
                            };

                            return Task.CompletedTask;
                        }
                    };
                });

        services.AddAuthorization();

        return services;
    }
}