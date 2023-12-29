using FluentValidation.Results;
using System.Net;
using TOTS_Calendar.Events.API.Domain.ControllerObjects;

namespace TOTS_Calendar.Events.API.Application.Common;

public static class ResponseCreator
{
      public static Response<T> ErrorResponse<T>(HttpStatusCode statusCode, string codeError, string propertyError, string messageError)
      {
            Response<T> response = new Response<T>();
            response.StatusCode = statusCode;
            response.AddNotification(new Notify
            {
                  Code = codeError,
                  Property = propertyError,
                  Message = messageError
            });
            return response;
      }

      public static Response<T> ResponseData<T>(HttpStatusCode statusCode, T content, string codeError, string propertyError, string messageError)
      {
            Response<T> response = new Response<T>();
            response.Content = content;
            response.StatusCode = statusCode;
            response.AddNotification(new Notify
            {
                  Code = codeError,
                  Property = propertyError,
                  Message = messageError
            });
            return response;
      }

      public static Response<T> ErrorResponse<T>(HttpStatusCode statusCode, List<ValidationFailure> Errors)
      {
            Response<T> response = new Response<T>
            {
                  StatusCode = statusCode
            };
            foreach (ValidationFailure Error in Errors)
            {
                  response.AddNotification(new Notify
                  {
                        Code = Error.ErrorCode,
                        Property = Error.PropertyName,
                        Message = Error.ErrorMessage
                  });
            }

            return response;
      }
}