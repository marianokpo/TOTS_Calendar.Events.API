using API.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using TOTS_Calendar.Events.API.Application.UseCase.V1.AuthorizationOperation.Query;
using TOTS_Calendar.Events.API.Domain.ControllerObjects;

namespace API.Controllers.V1;

[ApiController]
//[ApiVersion("1.0")]
[Authorize]
[AuthorizeForScopes(Scopes = new[] { "access_as_user" })]
[Route("api/v1/[controller]")]
public class AuthorizationController : ApiControllerBase
{
      /// <summary>
      /// Autorizaciones de permisos para utilizar la api
      /// </summary>
      /// <param name="body"></param>
      /// <returns></returns>
      [HttpGet]
      [ProducesResponseType(typeof(Uri), StatusCodes.Status200OK)]
      [ProducesResponseType(typeof(List<Notify>), StatusCodes.Status500InternalServerError)]
      public async Task<IActionResult> GetAuthorizationUrl() => Result(await Mediator.Send(new GetAuthorizationUrlRequest()));
}