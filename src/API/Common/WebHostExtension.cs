using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using System.Text.Json;

namespace API.Common;

public static class WebHostExtension
{
      private const string VERSION = "v.6.0.x";

      private const string FRAMEWORK = ".NET 6";

      private const string HEALTH = "/health";

      private const string HEALTHCHECK = "/healthcheck";

      private static IConfiguration? Configuration { get; set; }

      public static IServiceCollection ConfigureTotsServices(this IServiceCollection services, IConfiguration Configuration)
      {
            services.AddControllers();
            services.AddResponseCompression(delegate (ResponseCompressionOptions options)
            {
                  options.Providers.Add<GzipCompressionProvider>();
            });
            services.AddSwagger(Configuration);
            services.AddHealthChecks();
            services.AddScoped<FluentValidationMiddleware>();
            return services;
      }

      public static IApplicationBuilder ConfigureTots(this IApplicationBuilder app, Action<IEndpointRouteBuilder>? configEndopint = null, Action<IApplicationBuilder>? middleware = null)
      {
            Action<IEndpointRouteBuilder> configEndopint2 = configEndopint;
            IServiceProvider serviceProvider = app.ApplicationServices.CreateScope().ServiceProvider;
            IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
            app.ConfigureSwagger(configuration);
            app.UseResponseCompression();
            app.UseMiddleware<FluentValidationMiddleware>(Array.Empty<object>());
            app.UseSerilogRequestLogging();
            app.UseRouting();
            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();
            middleware?.Invoke(app);
            app.UseEndpoints(delegate (IEndpointRouteBuilder endpoints)
            {
                  endpoints.MapGet("/", async delegate (HttpContext context)
                  {
                        await context.Response.WriteAsync(JsonSerializer.Serialize(new Info
                        {
                              ApplicationName = (configuration["ApplicationName"] ?? string.Empty),
                              Platform = "v.6.0.x",
                              Framework = ".NET 6"
                        }));
                  });
                  endpoints.MapHealthChecks("/health");
                  endpoints.MapHealthChecks("/healthcheck", new HealthCheckOptions
                  {
                        Predicate = (HealthCheckRegistration _) => true,
                        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                  });
                  endpoints.MapControllers();
                  configEndopint2?.Invoke(endpoints);
            });
            return app;
      }

      public static IServiceCollection ConfigureTotsWorkerServices(this IServiceCollection services)
      {
            services.AddHealthChecks();
            return services;
      }

      public static IApplicationBuilder ConfigureTotsWorker(this IApplicationBuilder app, Action<IEndpointRouteBuilder>? configEndopint = null, Action<IApplicationBuilder>? middleware = null)
      {
            Action<IEndpointRouteBuilder> configEndopint2 = configEndopint;
            IServiceProvider serviceProvider = app.ApplicationServices.CreateScope().ServiceProvider;
            IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
            app.UseRouting();
            middleware?.Invoke(app);
            app.UseEndpoints(delegate (IEndpointRouteBuilder endpoints)
            {
                  endpoints.MapGet("/", async delegate (HttpContext context)
                  {
                        await context.Response.WriteAsync(JsonSerializer.Serialize(new Info
                        {
                              ApplicationName = (configuration["ApplicationName"] ?? "Tots Api"),
                              Platform = "v.6.0.x",
                              Framework = ".NET 6"
                        }));
                  });
                  endpoints.MapHealthChecks("/health");
                  endpoints.MapHealthChecks("/healthcheck", new HealthCheckOptions
                  {
                        Predicate = (HealthCheckRegistration _) => true,
                        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                  });
                  configEndopint2?.Invoke(endpoints);
            });
            return app;
      }

      public static IHostBuilder ConfigureTotsWebHost(this IHostBuilder WebHost, string[] args)
      {
            string[] args2 = args;
            RunInteractive();
            return WebHost.UseSerilog(delegate (HostBuilderContext ctx, LoggerConfiguration l)
            {
                  ConfigurationLogging.Configure(ctx, l);
            }).ConfigureAppConfiguration(delegate (HostBuilderContext hostingContext, IConfigurationBuilder config)
            {
                  config = SetConfig(config, args2, hostingContext);
                  Configuration = config.Build();
            });
      }

      public static ICorsWebHostService AddDefaultCors(this IServiceCollection services, IConfiguration configuration)
      {
            CorsOption? option = configuration.GetSection("Cors").Get<CorsOption>() ?? throw new ArgumentException("The field Cors is required");
            CorsPolicy policy = new CorsPolicy
            {
                  Headers = { "*" },
                  Methods = { "*" }
            };
            return new CorsWebHostService(option, services, policy);
      }

      public static IServiceCollection AddDefaultCors(this IServiceCollection service, Action<CorsOptions> action)
      {
            service.AddCors(action);
            return service;
      }

      public static ICorsWebHostService WithOrigins(this ICorsWebHostService webHostService)
      {
            if (!webHostService.Option.Origins.Any())
            {
                  throw new ArgumentException("The field Origins is required");
            }

            webHostService.Policy.IsOriginAllowed = SetIsAllowed(webHostService.Option.Origins);
            return webHostService;
      }

      public static ICorsWebHostService WithHeaders(this ICorsWebHostService webHostService)
      {
            ICorsWebHostService webHostService2 = webHostService;
            if (!webHostService2.Option.Headers.Any())
            {
                  throw new ArgumentException("The field Headers is required");
            }

            webHostService2.Policy.Headers.Clear();
            webHostService2.Option.Headers.ForEach(delegate (string header)
            {
                  webHostService2.Policy.Headers.Add(header);
            });
            return webHostService2;
      }

      public static ICorsWebHostService WithMethods(this ICorsWebHostService webHostService)
      {
            ICorsWebHostService webHostService2 = webHostService;
            if (!webHostService2.Option.Methods.Any())
            {
                  throw new ArgumentException("The field Methods is required");
            }

            webHostService2.Policy.Methods.Clear();
            webHostService2.Option.Methods.ForEach(delegate (string method)
            {
                  webHostService2.Policy.Methods.Add(method);
            });
            return webHostService2;
      }

      public static ICorsWebHostService WithExposedHeaders(this ICorsWebHostService webHostService)
      {
            ICorsWebHostService webHostService2 = webHostService;
            if (!webHostService2.Option.ExposedHeaders.Any())
            {
                  throw new ArgumentException("The field ExposedHeaders is required");
            }

            webHostService2.Option.ExposedHeaders.ForEach(delegate (string exposedHeader)
            {
                  webHostService2.Policy.ExposedHeaders.Add(exposedHeader);
            });
            return webHostService2;
      }

      public static ICorsWebHostService WithMaxAge(this ICorsWebHostService webHostService)
      {
            if (!webHostService.Option.MaxAge.HasValue)
            {
                  throw new ArgumentException("The field MaxAge is required");
            }

            webHostService.Policy.PreflightMaxAge = webHostService.Option.MaxAge.Value;
            return webHostService;
      }

      public static ICorsWebHostService WithCredentials(this ICorsWebHostService webHostService)
      {
            webHostService.Policy.SupportsCredentials = true;
            return webHostService;
      }

      public static ICorsWebHostService WithOutCredentials(this ICorsWebHostService webHostService)
      {
            webHostService.Policy.SupportsCredentials = false;
            return webHostService;
      }

      public static void Build(this ICorsWebHostService webHostService)
      {
            ICorsWebHostService webHostService2 = webHostService;
            if (!webHostService2.Option.Origins.Any())
            {
                  throw new ArgumentException("The field Origins is required");
            }

            webHostService2.Services.AddCors(delegate (CorsOptions options)
            {
                  options.AddDefaultPolicy(webHostService2.Policy);
            });
      }

      private static Func<string, bool> SetIsAllowed(List<string> listIn)
      {
            List<string> listIn2 = listIn;
            return delegate (string value)
            {
                  string value2 = value;
                  return listIn2.Exists((string url) => value2.EndsWith(url));
            };
      }

      private static void RunInteractive()
      {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($@"
 _____ _____ _____ _____   _____       _                _               ___        _
|_   _|  _  |_   _/  ___| /  __ \     | |              | |             / _ \      (_)
  | | | | | | | | \ `--.  | /  \/ __ _| | ___ _ __   __| | __ _ _ __  / /_\ \_ __  _
  | | | | | | | |  `--. \ | |    / _` | |/ _ \ '_ \ / _` |/ _` | '__| |  _  | '_ \| |
  | | \ \_/ / | | /\__/ / | \__/\ (_| | |  __/ | | | (_| | (_| | |    | | | | |_) | |
  \_/  \___/  \_/ \____/   \____/\__,_|_|\___|_| |_|\__,_|\__,_|_|    \_| |_/ .__/|_|
                                                                            | |
                                                                            |_|
                                                              by Mariano Damian Abadie
");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\nRunning interactively, press Control+C to exit.");
      }

      private static IConfigurationBuilder SetConfig(IConfigurationBuilder builder, string[] args, HostBuilderContext hostingContext)
      {
            builder.Sources.Clear();
            IHostEnvironment hostingEnvironment = hostingContext.HostingEnvironment;
            builder.SetBasePath(AppContext.BaseDirectory).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddJsonFile("appsettings." + hostingEnvironment.EnvironmentName + ".json", optional: true, reloadOnChange: true);
            builder.SetBasePath(AppContext.BaseDirectory).AddYamlFile("appsettings.yml", optional: true, reloadOnChange: true).AddYamlFile("appsettings." + hostingEnvironment.EnvironmentName + ".yml", optional: true, reloadOnChange: true);
            builder.AddEnvironmentVariables();
            if (args != null)
            {
                  builder.AddCommandLine(args);
            }

            return builder;
      }
}