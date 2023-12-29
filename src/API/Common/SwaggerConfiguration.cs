using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;

namespace API.Common;

public static class SwaggerConfiguration
{
      public static string ContentRootPath
      {
            get
            {
                  AppDomain currentDomain = AppDomain.CurrentDomain;
                  if (string.IsNullOrEmpty(currentDomain.RelativeSearchPath))
                  {
                        return currentDomain.BaseDirectory;
                  }

                  return currentDomain.RelativeSearchPath;
            }
      }

      public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration Configuration)
      {
            services.AddSwaggerGen(delegate (SwaggerGenOptions c)
            {
                  c.SwaggerDoc("v1", new OpenApiInfo { Title = "TotsCalendarApi", Version = "v1" });
                  c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                  {
                        Type = SecuritySchemeType.OAuth2,
                        Flows = new OpenApiOAuthFlows
                        {
                              Implicit = new OpenApiOAuthFlow
                              {
                                    AuthorizationUrl = new Uri(string.Format("https://login.microsoftonline.com/{0}/oauth2/v2.0/authorize", Configuration["AzureAd:TenantId"])),
                                    Scopes = Configuration["AzureAd:Scopes"]
                                             .Split(' ')
                                             .ToDictionary(s => s.Trim(), s => ""),
                              }
                        }
                  });
                  c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                  {
                        {
                              new OpenApiSecurityScheme {
                              Reference = new OpenApiReference {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "oauth2"
                              },
                              Scheme = "oauth2",
                              Name = "oauth2",
                              In = ParameterLocation.Header
                              },
                              Array.Empty<string>()
                        }
                  });

                  string[] files = Directory.GetFiles(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty), "*.xml");
                  foreach (string filePath in files)
                  {
                        try
                        {
                              c.IncludeXmlComments(filePath);
                        }
                        catch (Exception value)
                        {
                              Console.WriteLine(value);
                        }
                  }
            });
            return services;
      }

      public static IApplicationBuilder ConfigureSwagger(this IApplicationBuilder app, IConfiguration conf)
      {
            SwaggerOptions swaggerOptions = conf.GetSection("Swagger").Get<SwaggerOptions>();
            app.UseSwagger(delegate (Swashbuckle.AspNetCore.Swagger.SwaggerOptions c)
            {
                  c.PreSerializeFilters.Add(delegate (OpenApiDocument swagger, HttpRequest httpReq)
                  {
                        swagger.Servers = new List<OpenApiServer>
                        {
                              new OpenApiServer
                              {
                                    Url = "https://" + httpReq.Host.Value
                              },
                              new OpenApiServer
                              {
                                    Url = httpReq.Scheme + "://" + httpReq.Host.Value
                              }
                        };
                  });
            });

            app.UseSwaggerUI(delegate (SwaggerUIOptions c)
            {
                  c.SwaggerEndpoint("/swagger/v1/swagger.json", "CodaCalendarRestApi v1");
                  c.OAuthClientId(conf["AzureAd:ClientId"]);

                  c.RoutePrefix = "docs";
            });

            return app;
      }
}