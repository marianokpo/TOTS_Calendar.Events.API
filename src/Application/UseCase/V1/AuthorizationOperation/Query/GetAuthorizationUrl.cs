using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Encodings.Web;
using TOTS_Calendar.Events.API.Domain.ControllerObjects;

namespace TOTS_Calendar.Events.API.Application.UseCase.V1.AuthorizationOperation.Query;

public class GetAuthorizationUrlRequest : IRequest<Response<Uri>>
{
}

public class GetAuthorizationUrlHandler : IRequestHandler<GetAuthorizationUrlRequest, Response<Uri>>
{
      private readonly IConfiguration _configuration;
      private readonly ILogger<GetAuthorizationUrlHandler> _logger;

      public GetAuthorizationUrlHandler(ILogger<GetAuthorizationUrlHandler> logger, IConfiguration configuration)
      {
            _configuration = configuration;
            _logger = logger;
      }

      public async Task<Response<Uri>> Handle(GetAuthorizationUrlRequest request, CancellationToken cancellationToken)
      {
            try
            {
                  _logger.LogInformation("call getAuthorizationUrl");
                  var auth = string.Format("{0}/{1}/oauth2/v2.0/authorize?client_id={2}&prompt=select_account&scope={3}&response_type=token&redirect_uri={4}",
                  _configuration.GetValue<string>("AzureAd:Instance"),
                  _configuration.GetValue<string>("AzureAd:TenantId"),
                                          _configuration.GetValue<string>("AzureAd:ClientId"),
                                          UrlEncoder.Default.Encode(_configuration.GetValue<string>("DownstreamApi:Scopes")),
                                          UrlEncoder.Default.Encode(_configuration.GetValue<string>("DownstreamApi:RedirectUri")));

                  return new Response<Uri>
                  {
                        Content = new Uri(auth),
                        StatusCode = HttpStatusCode.OK
                  };
            }
            catch (Exception ex)
            {
                  var errorMessage = string.Format("ERROR getAuthorizationUrl: {0}", ex.Message);
                  _logger.LogError(errorMessage);

                  var r = new Response<Uri>
                  {
                        Content = null,
                        StatusCode = HttpStatusCode.InternalServerError
                  };

                  r.AddNotification(new Notify()
                  {
                        Code = "InternalServerError",
                        Message = errorMessage,
                        Property = "Authorization"
                  });

                  return r;
            }
      }
}