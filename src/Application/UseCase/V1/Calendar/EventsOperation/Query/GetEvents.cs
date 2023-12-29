using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;
using TOTS_Calendar.Events.API.Application.Common.Interfaces;
using TOTS_Calendar.Events.API.Domain.ControllerObjects;
using TOTS_Calendar.Events.API.Domain.Dtos;

namespace TOTS_Calendar.Events.API.Application.UseCase.V1.Calendar.EventsOperation.Query;

public class GetEventsRequest : IRequest<Response<List<EventDto>?>>
{
}

public class GetEventsHandler : IRequestHandler<GetEventsRequest, Response<List<EventDto>?>>
{
      private readonly ICalendarService _calendarService;
      private readonly ILogger<GetEventsHandler> _logger;

      public GetEventsHandler(ILogger<GetEventsHandler> logger, ICalendarService calendarService)
      {
            _logger = logger;
            _calendarService = calendarService;
      }

      public async Task<Response<List<EventDto>?>> Handle(GetEventsRequest request, CancellationToken cancellationToken)
      {
            try
            {
                  _logger.LogInformation("call GetEvents");
                  var eventDtos = await _calendarService.GetEvents();

                  return new Response<List<EventDto>?>
                  {
                        Content = eventDtos.ToList(),
                        StatusCode = HttpStatusCode.OK
                  };
            }
            catch (Exception ex)
            {
                  var errorMessage = string.Format("ERROR GetEvents: {0}", ex.Message);
                  _logger.LogError(errorMessage);

                  var r = new Response<List<EventDto>?>
                  {
                        Content = null,
                        StatusCode = HttpStatusCode.InternalServerError
                  };

                  r.AddNotification(new Notify()
                  {
                        Code = "InternalServerError",
                        Message = errorMessage,
                        Property = "GetEvents"
                  });

                  return r;
            }
      }
}