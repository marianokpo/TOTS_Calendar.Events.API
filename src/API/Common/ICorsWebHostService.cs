using Microsoft.AspNetCore.Cors.Infrastructure;

namespace API.Common;

public interface ICorsWebHostService
{
      CorsOption Option { get; }

      CorsPolicy Policy { get; set; }

      IServiceCollection Services { get; }
}