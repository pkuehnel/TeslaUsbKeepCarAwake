//var builder = WebApplication.CreateBuilder(args);

using System.Runtime.InteropServices;
using MQTTnet.Adapter;
using MQTTnet.Client;
using MQTTnet.Diagnostics;
using MQTTnet.Implementations;
using MQTTnet;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using Serilog;
using TeslaUsbKeepCarAwake.Dtos;
using TeslaUsbKeepCarAwake.Scheduling;
using TeslaUsbKeepCarAwake.Scheduling.Jobs;
using TeslaUsbKeepCarAwake.Services;
using TeslaUsbKeepCarAwake.Services.Contracts;

var builder = WebApplication.CreateBuilder();
// Add services to the container.

builder.Services.AddControllers()
    .AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables();

var settings = new Settings();
if (File.Exists("settings.json"))
{
    var settingsString = await File.ReadAllTextAsync("settings.json");
    settings = JsonConvert.DeserializeObject<Settings>(settingsString);
}

builder.Services
    .AddSingleton<JobManager>()
    .AddTransient<IJobFactory, JobFactory>()
    .AddTransient<ISchedulerFactory, StdSchedulerFactory>()
    .AddTransient<WakeUpJob>()
    .AddTransient<MqttReconnectionJob>()
    .AddTransient<IMqttNetLogger, MqttNetNullLogger>()
    .AddTransient<IMqttClientAdapterFactory, MqttClientAdapterFactory>()
    .AddTransient<IMqttClient, MqttClient>()
    .AddTransient<MqttFactory>()
    .AddSingleton<IMqttService, MqttService>()
    .AddTransient<ITeslaMateService, TeslaMateService>()
    .AddTransient<HelloService>()
    .AddSingleton<CarState>()
    .AddSingleton<Internals>()
    .AddSingleton(settings ?? new Settings())
    ;

builder.Host.UseSerilog((context, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

settings = app.Services.GetRequiredService<Settings>();

if (settings.CarId > 0)
{
    if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    {

    }
    var jobManager = app.Services.GetRequiredService<JobManager>();
    await jobManager.StartJobs();
}
// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();
