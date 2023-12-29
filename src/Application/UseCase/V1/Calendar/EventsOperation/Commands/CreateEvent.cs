using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;
using TOTS_Calendar.Events.API.Application.Common;
using TOTS_Calendar.Events.API.Application.Common.Interfaces;
using TOTS_Calendar.Events.API.Domain.ControllerObjects;
using TOTS_Calendar.Events.API.Domain.Dtos;
using TOTS_Calendar.Events.API.Domain.Validation;

namespace TOTS_Calendar.Events.API.Application.UseCase.V1.Calendar.EventsOperation.Commands;

public class CreateEventRequest : IRequest<Response<EventDto>>
{
      public RequestEventDto newEvent { get; set; }
}

public class CreateEventHandler : IRequestHandler<CreateEventRequest, Response<EventDto>>
{
      private readonly ICalendarService _calendarService;
      private readonly ILogger<CreateEventHandler> _logger;

      public CreateEventHandler(ILogger<CreateEventHandler> logger, ICalendarService calendarService)
      {
            _logger = logger;
            _calendarService = calendarService;
      }

      public async Task<Response<EventDto>> Handle(CreateEventRequest request, CancellationToken cancellationToken)
      {
            try
            {
                  RequestEventDtoValidation validator = new();
                  var validationResult = validator.Validate(request.newEvent);

                  if (!validationResult.IsValid)
                  {
                        return ResponseCreator.ErrorResponse<EventDto>(HttpStatusCode.BadRequest,
                            validationResult.Errors);
                  }

                  _logger.LogInformation("call CreateEvent");
                  var eventDto = await _calendarService.CreateEvent(request.newEvent.GetEventDto());

                  return new Response<EventDto>
                  {
                        Content = eventDto,
                        StatusCode = HttpStatusCode.OK
                  };
            }
            catch (Exception ex)
            {
                  var errorMessage = string.Format("ERROR CreateEvent: {0}", ex.Message);
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
                        Property = "CreateEvent"
                  });

                  return r;
            }
      }
}