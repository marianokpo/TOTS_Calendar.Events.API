using TOTS_Calendar.Events.API.Domain.Dtos;

namespace TOTS_Calendar.Events.API.Application.Common.Interfaces;

public interface ICalendarService
{
      Task<IList<EventDto>> GetEvents();

      Task<EventDto> CreateEvent(EventDto eventDto);

      Task<EventDto> UpdateEvent(string id, EventDto eventDto);

      Task<string> DeleteEvent(string id);
}