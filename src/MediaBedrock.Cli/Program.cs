using Cocona;
using MediaBedrock.Application.Bootstrap;
using MediaBedrock.Cli.Presentation.Bootstrap;
using MediaBedrock.Domain.Bootstrap;
using MediaBedrock.Infrastructure.Bootstrap;
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

builder.Services.AddDomain();
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

var app = builder.Build();

app.Services.UseInfrastructure(builder.Environment);
app.UsePresentation();

await app.RunAsync();