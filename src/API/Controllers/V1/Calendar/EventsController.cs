using API.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using TOTS_Calendar.Events.API.Application.UseCase.V1.Calendar.EventsOperation.Commands;
using TOTS_Calendar.Events.API.Application.UseCase.V1.Calendar.EventsOperation.Query;
using TOTS_Calendar.Events.API.Domain.ControllerObjects;
using TOTS_Calendar.Events.API.Domain.Dtos;

namespace API.Controllers.V1.Calendar;

[ApiController]
//[ApiVersion("1.0")]
[Authorize]
[AuthorizeForScopes(Scopes = new[] { "access_as_user" })]
[Route("api/v1/Calendar/[controller]")]
public class EventsController : ApiControllerBase
{
      /// <summary>
      /// Obtener los eventos
      /// </summary>
      /// <param name="body"></param>
      /// <returns></returns>
      [HttpGet]
      [ProducesResponseType(typeof(List<EventDto>), StatusCodes.Status200OK)]
      [ProducesResponseType(typeof(List<Notify>), StatusCodes.Status500InternalServerError)]
      public async Task<IActionResult> GetEvents() => Result(await Mediator.Send(new GetEventsRequest()));

      /// <summary>
      /// Creación de nueva eventos
      /// </summary>
      /// <param name="body"></param>
      /// <returns></returns>
      [HttpPost]
      [ProducesResponseType(typeof(EventDto), StatusCodes.Status200OK)]
      [ProducesResponseType(typeof(List<Notify>), StatusCodes.Status400BadRequest)]
      [ProducesResponseType(typeof(List<Notify>), StatusCodes.Status500InternalServerError)]
      public async Task<IActionResult> CreateEvent([FromBody] RequestEventDto newEvent) => Result(await Mediator.Send(new CreateEventRequest() { newEvent = newEvent }));

      /// <summary>
      /// Actualización de eventos
      /// </summary>
      /// <param name="body"></param>
      /// <returns></returns>
      [HttpPatch("{id}")]
      [ProducesResponseType(typeof(EventDto), StatusCodes.Status200OK)]
      [ProducesResponseType(typeof(List<Notify>), StatusCodes.Status400BadRequest)]
      [ProducesResponseType(typeof(List<Notify>), StatusCodes.Status500InternalServerError)]
      public async Task<IActionResult> UpdateEvent(string id, [FromBody] RequestEventDto updatedEvent) => Result(await Mediator.Send(new UpdateEventRequest() { Id = id, UpdatedEvent = updatedEvent }));

      /// <summary>
      /// Eliminación de eventos
      /// </summary>
      /// <param name="body"></param>
      /// <returns></returns>
      [HttpDelete("{id}")]
      [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
      [ProducesResponseType(typeof(List<Notify>), StatusCodes.Status400BadRequest)]
      public async Task<IActionResult> DeleteEvent(string id) => Result(await Mediator.Send(new DeleteEventRequest() { id = id }));
}