using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using TOTS_Calendar.Events.API.Application.Common.Interfaces;
using TOTS_Calendar.Events.API.Infrastructure.Services;

namespace TOTS_Calendar.Events.API.Infrastructure;

public static class DependencyInjection
{
      public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
      {
            string[] initialScopes = configuration.GetValue<string>("DownstreamApi:Scopes")?.Split(' ') ?? Array.Empty<string>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(configuration.GetSection("AzureAd"))
                .EnableTokenAcquisitionToCallDownstreamApi()
                .AddDownstreamApi("GraphApi", configuration.GetSection("AzureAd"))
                .AddMicrosoftGraph(options =>

                      configuration.GetSection("DownstreamApi").Bind(options)
                )
                .AddDistributedTokenCaches();

            services.AddDistributedSqlServerCache(options =>
            {
                  configuration.Bind("TokenCacheDatabase", options);
            });

            services.AddScoped<ICalendarService, CalendarService>();
            return services;
      }
}