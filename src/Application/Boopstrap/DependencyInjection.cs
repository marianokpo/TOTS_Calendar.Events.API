using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace TOTS_Calendar.Events.API.Application;

public static class DependencyInjection
{
      public static IServiceCollection AddApplication(this IServiceCollection services)
      {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            return services;
      }
}