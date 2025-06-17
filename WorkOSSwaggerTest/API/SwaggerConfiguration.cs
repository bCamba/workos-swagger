using Microsoft.OpenApi.Models;

namespace API;

internal static class SwaggerConfiguration
{
    public static IServiceCollection AddSwaggerConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGen(
            options =>
            {
                var securityScheme = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference()
                    {
                        Id = "bearer",
                        Type = ReferenceType.SecurityScheme
                    }
                };

                options.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
                options.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            securityScheme, new string[]
                            {
                            }
                        }
                    });
                
                var authKitUrl = configuration.GetValue<string>("AuthKitUrl");
                options.AddSecurityDefinition(
                    "WorkOSAuthKit",
                    new OpenApiSecurityScheme
                    {
                        Type  = SecuritySchemeType.OAuth2,
                        Flows = new OpenApiOAuthFlows
                        {
                            AuthorizationCode = new OpenApiOAuthFlow
                            {
                                AuthorizationUrl = new Uri($"{authKitUrl}/oauth2/authorize"),
                                TokenUrl         = new Uri($"{authKitUrl}/oauth2/token"),
                                Scopes = new Dictionary<string,string>
                                {
                                    { "openid",          "Basic sign-in"        },
                                    { "profile",         "Read user profile"    },
                                    { "email",           "Read user e-mail"     },
                                    { "offline_access",  "Refresh-token scope"  }
                                }
                            }
                        }
                    });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    [ new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id   = "WorkOSAuthKit"
                            }
                        }
                    ] = new[] { "openid" }
                });
            });

        return services;
    }
}
