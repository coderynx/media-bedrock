using Cocona;
using MediaBedrock.Controller.Application.Bootstrap;
using MediaBedrock.Controller.Domain.Bootstrap;
using MediaBedrock.Controller.Infrastructure.Bootstrap;
using MediaBedrock.Controller.Presentation.Bootstrap;
using MediaBedrock.Worker.Application;
using MediaBedrock.Worker.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

var builder = CoconaApp.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration).Enrich.FromLogContext();
});

builder.Services.Configure<HostOptions>(options =>
{
    options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.StopHost;
});

builder.Services.AddControllerDomain();
builder.Services.AddControllerApplication();
builder.Services.AddControllerInfrastructure();

builder.Services.AddWorkerInfrastructure();
builder.Services.AddWorkerApplication();

var app = builder.Build();

app.Services.UseWorkerInfrastructure(builder.Environment);

app.Services.UseControllerInfrastructure(builder.Environment);
app.UseControllerPresentation();

await app.RunAsync();