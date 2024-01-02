using FluentAssertions;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Graph.Me.Events;
using Moq;
using TOTS_Calendar.Events.API.Domain.Dtos;
using TOTS_Calendar.Events.API.Infrastructure.Services;
using Microsoft.Kiota.Abstractions;
using FluentAssertions.Common;
using Microsoft.Graph.Me;

namespace Infrastructure.Services;

public class CalendarServiceTest
{
      private readonly Mock<IAuthenticationProvider> authenticationProvider;
      private readonly Mock<GraphServiceClient> _graphServiceClient;

      private CalendarService _service;

      public CalendarServiceTest()
      {
            // Arrange
            authenticationProvider = new Mock<IAuthenticationProvider>();
            _graphServiceClient = new Mock<GraphServiceClient>(authenticationProvider.Object, "https://graph.microsoft.com/v1.0");

            _service = new CalendarService(_graphServiceClient.Object);
      }

      [Theory]
      [InlineData(null, "2022-10-10", "2022-10-10", null, null, null)]
      public async Task CreateEvent_Success(string? Subject, DateTime Start, DateTime End, string? Timezone, string? Body, string? Attendees)
      {
            // Arrange
            var resultEvent = new Event();

            Mock<MeRequestBuilder> meResponse = new Mock<MeRequestBuilder>();
            Mock<EventsRequestBuilder> eventsResponse = new Mock<EventsRequestBuilder>();

            eventsResponse.Setup(c => c.PostAsync(It.IsAny<Event>(), It.IsAny<Action<RequestConfiguration<DefaultQueryParameters>>?>(), It.IsAny<CancellationToken>())).ReturnsAsync(resultEvent);

            meResponse.SetupGet(c => c.Events).Returns(eventsResponse.Object);

            _graphServiceClient.SetupGet(c => c.Me).Returns(meResponse.Object);
            /*_graphServiceClient.Setup(c => c.Me.Events.
                  PostAsync(It.IsAny<Event>(),
                        It.IsAny<Action<RequestConfiguration<DefaultQueryParameters>>?>(),
                        It.IsAny<CancellationToken>()))
                              .ReturnsAsync(resultEvent);*/

            // Act
            EventDto result = await _service.CreateEvent(new EventDto()
            {
                  Subject = Subject,
                  Start = Start,
                  End = End,
                  Timezone = Timezone,
                  Body = Body,
                  Attendees = Attendees
            });

            // Assert
            result.Should().NotBeNull();
      }
}