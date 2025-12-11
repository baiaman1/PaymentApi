using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using PaymentApi.Application.Features.Auth;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace PaymentApi.Api.Configurations
{
    public static class SwaggerConfiguration
    {
        public static IServiceCollection AddCustomSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Payment API",
                    Version = "v1",
                    Description = "Payment Processing System API",
                    Contact = new OpenApiContact
                    {
                        Name = "Development Team Kyzmat",
                        Email = "kyzmat@dev.com"
                    }
                });
               

                options.DescribeAllParametersInCamelCase();

                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.\n\n" +
                                  "Enter your token in the text input below.\n" +
                                  "Example: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = JwtBearerDefaults.AuthenticationScheme
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                options.OrderActionsBy(apiDesc => apiDesc.RelativePath);
            });

            return services;
        }

        public static IApplicationBuilder UseCustomSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Payment API v1");
                options.DisplayRequestDuration();
                options.EnableDeepLinking();
                options.DefaultModelsExpandDepth(-1);
                options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
            });

            return app;
        }
    }
}
