using FluentValidation;
using FluentValidation.Results;
using System.Text.Json;
using TOTS_Calendar.Events.API.Domain.ControllerObjects;

namespace API.Common;

internal sealed class FluentValidationMiddleware : IMiddleware
{
      private readonly ILogger<FluentValidationMiddleware> _logger;

      public FluentValidationMiddleware(ILogger<FluentValidationMiddleware> logger)
      {
            _logger = logger;
      }

      public async Task InvokeAsync(HttpContext context, RequestDelegate next)
      {
            try
            {
                  await next(context);
            }
            catch (Exception ex)
            {
                  if (!(ex is ValidationException))
                  {
                        await HandleExceptionServerAsync(context, ex);
                  }
                  else
                  {
                        await HandleExceptionAsync(context, ex);
                  }
            }
      }

      private async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
      {
            int statusCode = 400;
            List<Notify> errors = GetErrors(exception);
            errors.ForEach(delegate (Notify x)
            {
                  _logger.LogError("BadRequest - {x}", x);
            });
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsync(JsonSerializer.Serialize(errors));
      }

      private static List<Notify> GetErrors(Exception exception)
      {
            if (exception.InnerException is ValidationException)
            {
                  return ((ValidationException)exception).Errors.Select((ValidationFailure x) => new Notify
                  {
                        Code = x.ErrorCode,
                        Message = x.ErrorMessage,
                        Property = x.PropertyName
                  }).ToList();
            }

            return new List<Notify>
            {
                  new Notify
                  {
                        Code = "500 internal server",
                        Property = exception.GetType().ToString(),
                        Message = exception.Message
                  }
            };
      }

      private async Task HandleExceptionServerAsync(HttpContext httpContext, Exception exception)
      {
            int statusCode = 500;
            List<Notify> errors = GetErrors(exception);
            errors.ForEach(delegate (Notify x)
            {
                  _logger.LogError("InternalServerError - {x}", x);
            });
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsync(JsonSerializer.Serialize(errors));
      }
}