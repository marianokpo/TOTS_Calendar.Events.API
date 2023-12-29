namespace TOTS_Calendar.Events.API.Domain.ControllerObjects;

public class Notify
{
      public string Code { get; set; } = string.Empty;

      public string Property { get; set; } = string.Empty;

      public string Message { get; set; } = string.Empty;

      public override string ToString()
      {
            return $"Notify - Code: {Code}, Property: {Property}, Message: {Message}";
      }
}