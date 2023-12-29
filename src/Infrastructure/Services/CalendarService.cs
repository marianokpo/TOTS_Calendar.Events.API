using Microsoft.Graph;
using Microsoft.Graph.Models;
using System.ComponentModel.DataAnnotations;
using TOTS_Calendar.Events.API.Application.Common.Interfaces;
using TOTS_Calendar.Events.API.Domain.Dtos;
using TOTS_Calendar.Events.API.Domain.Mapper;

namespace TOTS_Calendar.Events.API.Infrastructure.Services;

public class CalendarService : ICalendarService
{
      private const int EventsPageSize = 50;

      private readonly GraphServiceClient _graphServiceClient;

      public CalendarService(GraphServiceClient graphServiceClient)
      {
            _graphServiceClient = graphServiceClient;
      }

      public async Task<EventDto> CreateEvent(EventDto eventDto)
      {
            eventDto.Validate();
            Event graphEvent = OutlookCalendarEventMapper.FromDto(eventDto);
            var newEvent = await _graphServiceClient.Me.Events.PostAsync(graphEvent);
            return OutlookCalendarEventMapper.ToDto(newEvent);
      }

      public async Task<string> DeleteEvent(string id)
      {
            await _graphServiceClient.Me.Events[id].DeleteAsync();
            return id;
      }

      public async Task<IList<EventDto>> GetEvents()
      {   //just returning the first page for this poc
            var graphEvents = await _graphServiceClient.Me.Events.GetAsync(c =>
            {
                  c.Headers.Add("Prefer", "outlook.timezone=\"UTC\"");
                  c.QueryParameters.Select = new string[] { "subject", "body", "attendees", "start", "end" };
                  c.QueryParameters.Orderby = new string[] { "start/dateTime  desc" };
                  c.QueryParameters.Top = EventsPageSize;
            });

            return graphEvents.Value.Select(ge => OutlookCalendarEventMapper.ToDto(ge)).ToList();
      }

      public async Task<EventDto> UpdateEvent(string id, EventDto eventDto)
      {
            eventDto.Validate();
            if (!string.IsNullOrEmpty(eventDto.Id) && !string.Equals(id, eventDto.Id))
            {
                  throw new ValidationException("Id in the url path should be the same as the id in the event body");
            }

            Event graphEvent = OutlookCalendarEventMapper.FromDto(eventDto);
            await _graphServiceClient.Me.Events[id].PatchAsync(graphEvent);

            return OutlookCalendarEventMapper.ToDto(graphEvent); ;
      }
}