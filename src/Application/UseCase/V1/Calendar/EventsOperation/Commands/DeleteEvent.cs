using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;
using TOTS_Calendar.Events.API.Application.Common.Interfaces;
using TOTS_Calendar.Events.API.Domain.ControllerObjects;

namespace TOTS_Calendar.Events.API.Application.UseCase.V1.Calendar.EventsOperation.Commands;

public class DeleteEventRequest : IRequest<Response<string>>
{
      public string id { get; set; }
}

public class DeleteEventHandler : IRequestHandler<DeleteEventRequest, Response<string>>
{
      private readonly ICalendarService _calendarService;
      private readonly ILogger<DeleteEventHandler> _logger;

      public DeleteEventHandler(ILogger<DeleteEventHandler> logger, ICalendarService calendarService)
      {
            _logger = logger;
            _calendarService = calendarService;
      }

      public async Task<Response<string>> Handle(DeleteEventRequest request, CancellationToken cancellationToken)
      {
            try
            {
                  _logger.LogInformation("call DeleteEvent {ID}", request.id);
                  var deletedEventId = await _calendarService.DeleteEvent(request.id);

                  return new Response<string>
                  {
                        Content = deletedEventId,
                        StatusCode = HttpStatusCode.OK
                  };
            }
            catch (Exception ex)
            {
                  var errorMessage = string.Format("ERROR DeleteEvent: {0}", ex.Message);
                  _logger.LogError(errorMessage);

                  var r = new Response<string>
                  {
                        Content = null,
                        StatusCode = HttpStatusCode.InternalServerError
                  };

                  r.AddNotification(new Notify()
                  {
                        Code = "InternalServerError",
                        Message = errorMessage,
                        Property = "DeleteEvent"
                  });

                  return r;
            }
      }
}