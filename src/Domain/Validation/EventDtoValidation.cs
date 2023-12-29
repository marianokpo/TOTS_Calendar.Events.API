using FluentValidation;
using TOTS_Calendar.Events.API.Domain.Dtos;

namespace TOTS_Calendar.Events.API.Domain.Validation;

public class EventDtoValidation : AbstractValidator<EventDto>
{
      public EventDtoValidation()
      {
            RuleFor(request => request.Id).NotNull().WithMessage(errorMessage: "Id cannot be null.");
            RuleFor(request => request.Subject).NotNull().WithMessage(errorMessage: "Subject cannot be null.");
            RuleFor(request => request.Start).NotNull().WithMessage(errorMessage: "Start cannot be null.");
            RuleFor(request => request.End).NotNull().WithMessage(errorMessage: "End cannot be null.");
            RuleFor(x => x.Body).NotEmpty().WithMessage(errorMessage: "Body cannot be empty.");

            RuleFor(x => x.Body)
             .Matches(@"((\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)*([;])*)*")
             .WithMessage(errorMessage: "Please enter one or more email addresses separated by a semi - colon(;)"); ;
      }
}