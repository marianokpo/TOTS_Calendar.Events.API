using Serilog;
using Serilog.Core;
using Serilog.Events;
using System.Reflection;

namespace API.Common;

public static class ConfigurationLogging
{
      public static LoggingLevelSwitch LoggingLevelSwitch { get; set; } = new LoggingLevelSwitch();

      public static void Configure(HostBuilderContext context, LoggerConfiguration loggerConfiguration)
      {
            IConfiguration configuration = context.Configuration;
            if (Enum.TryParse<LogEventLevel>(configuration.GetSection("Serilog:MinimumLevel").Value ?? configuration.GetSection("Serilog:MinimumLevel:Default").Value, out var result))
            {
                  LoggingLevelSwitch.MinimumLevel = result;
            }

            loggerConfiguration.ReadFrom.Configuration(configuration).MinimumLevel.ControlledBy(LoggingLevelSwitch).Enrich.WithProperty("AppDomain", Assembly.GetEntryAssembly()?.GetName().Name);
      }
}