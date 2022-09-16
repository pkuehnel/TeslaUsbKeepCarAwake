//var builder = WebApplication.CreateBuilder(args);

using MQTTnet.Adapter;
using MQTTnet.Client;
using MQTTnet.Diagnostics;
using MQTTnet.Implementations;
using MQTTnet;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using Serilog;
using TeslaUsbKeepCarAwake.Dtos;
using TeslaUsbKeepCarAwake.Scheduling;
using TeslaUsbKeepCarAwake.Services;
using TeslaUsbKeepCarAwake.Services.Contracts;

var builder = WebApplication.CreateBuilder();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddSingleton<JobManager>()
    .AddTransient<IJobFactory, JobFactory>()
    .AddTransient<ISchedulerFactory, StdSchedulerFactory>()
    .AddTransient<WakeUpJob>()
    .AddTransient<IMqttNetLogger, MqttNetNullLogger>()
    .AddTransient<IMqttClientAdapterFactory, MqttClientAdapterFactory>()
    .AddTransient<IMqttClient, MqttClient>()
    .AddTransient<MqttFactory>()
    .AddSingleton<MqttService>()
    .AddTransient<ITeslaMateService, TeslaMateService>()
    .AddSingleton<CarState>()
    .AddSingleton<Settings>()
    ;

builder.Host.UseSerilog((context, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

var settings = app.Services.GetRequiredService<Settings>();

settings.TeslaMateBaseUrl = args[0];
settings.MqttUrl = args[1];
settings.RelevantGeofence = args[2];
settings.ArchiveUploadPath = args[3];
settings.CarId = Convert.ToInt32(args[4]);

var mqttService = app.Services.GetRequiredService<MqttService>();
await mqttService.ConnectMqttClient();

var jobManager = app.Services.GetRequiredService<JobManager>();
await jobManager.StartJobs();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
