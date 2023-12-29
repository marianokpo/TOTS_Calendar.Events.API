using System.ComponentModel.DataAnnotations;

namespace TOTS_Calendar.Events.API.Domain.Dtos;

public class EventDto
{
      public string? Id { get; set; }

      public string? Subject { get; set; }

      public DateTime Start { get; set; }

      public DateTime End { get; set; }

      public string? Timezone { get; set; }

      public string? Body { get; set; }
      public string? Attendees { get; set; }

      public void Validate()
      {
            if (Start > End)
            {
                  throw new ValidationException("Event start date should be before than event end date");
            }
            try
            {
                  var timeZone = TimeZoneInfo.FindSystemTimeZoneById(Timezone);
            }
            catch (Exception ex)
            {
                  throw new ValidationException(string.Format("Invalid timezone: {0}", ex.Message));
            }
      }
}