using Microsoft.AspNetCore.Cors.Infrastructure;

namespace API.Common;

public class CorsWebHostService : ICorsWebHostService
{
      public CorsOption Option { get; }

      public CorsPolicy Policy { get; set; }

      public IServiceCollection Services { get; }

      public CorsWebHostService(CorsOption option, IServiceCollection services, CorsPolicy policy)
      {
            Option = option;
            Services = services;
            Policy = policy;
      }
}