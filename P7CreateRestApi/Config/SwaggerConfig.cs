using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace P7CreateRestApi.Config
{
    public static class SwaggerConfig
    {
        public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                // Infos générales
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Findexium API",
                    Version = "v1",
                    Description = "API sécurisée avec JWT et Identity"
                });

                // Schéma de sécurité (JWT)
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Entrer le token JWT : **Bearer {votre_token}**"
                });

                // Appliquer le schéma de sécurité globalement
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            return services;
        }
    }
}
