using API.Common;
using TOTS_Calendar.Events.API.Application;
using TOTS_Calendar.Events.API.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureTotsWebHost(args);
builder.Services.ConfigureTotsServices(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.ConfigureTots();

app.Run();