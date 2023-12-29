using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;
using TOTS_Calendar.Events.API.Application.Common;
using TOTS_Calendar.Events.API.Application.Common.Interfaces;
using TOTS_Calendar.Events.API.Domain.ControllerObjects;
using TOTS_Calendar.Events.API.Domain.Dtos;
using TOTS_Calendar.Events.API.Domain.Validation;

namespace TOTS_Calendar.Events.API.Application.UseCase.V1.Calendar.EventsOperation.Commands;

public class UpdateEventRequest : IRequest<Response<EventDto>>
{
      public string Id { get; set; }
      public RequestEventDto UpdatedEvent { get; set; }
}

public class UpdateEventHandler : IRequestHandler<UpdateEventRequest, Response<EventDto>>
{
      private readonly ICalendarService _calendarService;
      private readonly ILogger<UpdateEventHandler> _logger;

      public UpdateEventHandler(ILogger<UpdateEventHandler> logger, ICalendarService calendarService)
      {
            _logger = logger;
            _calendarService = calendarService;
      }

      public async Task<Response<EventDto>> Handle(UpdateEventRequest request, CancellationToken cancellationToken)
      {
            try
            {
                  RequestEventDtoValidation validator = new();
                  var validationResult = validator.Validate(request.UpdatedEvent);

                  if (!validationResult.IsValid)
                  {
                        return ResponseCreator.ErrorResponse<EventDto>(HttpStatusCode.BadRequest,
                            validationResult.Errors);
                  }

                  _logger.LogInformation("call UpdateEvent {ID}", request.Id);
                  var eventDto = await _calendarService.UpdateEvent(request.Id, request.UpdatedEvent.GetEventDto());

                  return new Response<EventDto>
                  {
                        Content = eventDto,
                        StatusCode = HttpStatusCode.OK
                  };
            }
            catch (Exception ex)
            {
                  var errorMessage = string.Format("ERROR UpdateEvent: {0}", ex.Message);
                  _logger.LogError(errorMessage);

                  var r = new Response<EventDto>
                  {
                        Content = null,
                        StatusCode = HttpStatusCode.InternalServerError
                  };

                  r.AddNotification(new Notify()
                  {
                        Code = "InternalServerError",
                        Message = errorMessage,
                        Property = "UpdateEvent"
                  });

                  return r;
            }
      }
}