using Cocona;
using MediaBedrock.Cli.Application.Bootstrap;
using MediaBedrock.Cli.Infrastructure.Bootstrap;
using MediaBedrock.Cli.Presentation.Bootstrap;
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

builder.Services.AddInfrastructure();
builder.Services.AddApplication();

var app = builder.Build();

app.UsePresentation();

await app.RunAsync();