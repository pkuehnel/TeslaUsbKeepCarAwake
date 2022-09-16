using MQTTnet.Client;
using MQTTnet;
using TeslaUsbKeepCarAwake.Dtos;

namespace TeslaUsbKeepCarAwake.Services;

public class MqttService
{
    private readonly ILogger<MqttService> _logger;
    private readonly IMqttClient _mqttClient;
    private readonly MqttFactory _mqttFactory;
    private readonly CarState _carState;
    private readonly Settings _settings;

    private const string TopicGeofence = "geofence";
    private const string TopicState = "state";

    public MqttService(ILogger<MqttService> logger, IMqttClient mqttClient, MqttFactory mqttFactory, CarState carState, Settings settings)
    {
        _logger = logger;
        _mqttClient = mqttClient;
        _mqttFactory = mqttFactory;
        _carState = carState;
        _settings = settings;
    }


    public async Task ConnectMqttClient()
    {
        _logger.LogTrace("{method}()", nameof(ConnectMqttClient));
        var mqqtClientId = "TeslaUsbKeepCarAwake";
        var mosquitoServer = GetMqttServerAndPort(out var mqttServerPort, _settings.MqttUrl);
        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithClientId(mqqtClientId)
            .WithTcpServer(mosquitoServer, mqttServerPort)
            .Build();

        _mqttClient.ApplicationMessageReceivedAsync += e =>
        {
            var value = GetValueFromMessage(e.ApplicationMessage);
            _logger.LogDebug("Car Id: {carId}, Topic: {topic}, Value: {value}", value.CarId, value.Topic, value.Value);
            UpdateCar(value, _settings.RelevantGeofence);
            return Task.CompletedTask;
        };

        try
        {
            if (_mqttClient.IsConnected)
            {
                await _mqttClient.DisconnectAsync(MqttClientDisconnectReason.AdministrativeAction,
                    "Reconnecting with new configuration").ConfigureAwait(false);
            }
            await _mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not connect to TeslaMate mqtt server");
            return;
        }

        var topicPrefix = $"teslamate/cars/{_settings.CarId}/";

        var mqttSubscribeOptions = _mqttFactory.CreateSubscribeOptionsBuilder()
            .WithTopicFilter(f =>
            {
                f.WithTopic($"{topicPrefix}{TopicGeofence}");
            })
            .WithTopicFilter(f =>
            {
                f.WithTopic($"{topicPrefix}{TopicState}");
            })
            .Build();

        await _mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None).ConfigureAwait(false);
    }

    private void UpdateCar(TeslaMateValue value, string relevantGeoFence)
    {
        switch (value.Topic)
        {
            case TopicGeofence:
                if (value.Value == relevantGeoFence)
                {
                    _carState.HomeGeofenceSince = DateTime.Now;
                }
                else
                {
                    _carState.HomeGeofenceSince = null;
                }
                _carState.LastGeofence = _carState.Geofence;
                _carState.Geofence = value.Value;
                break;
            case TopicState:
                _carState.LastState = _carState.State;
                _carState.State = value.Value;
                break;
            default:
                break;
        }
    }


    private TeslaMateValue GetValueFromMessage(MqttApplicationMessage mqttApplicationMessage)
    {
        var relevantString = mqttApplicationMessage.Topic.Substring(15, mqttApplicationMessage.Topic.Length - 15);

        var splittedString = relevantString.Split("/");

        return new TeslaMateValue()
        {
            CarId = Convert.ToInt32(splittedString[0]),
            Topic = splittedString[1],
            Value = mqttApplicationMessage.ConvertPayloadToString(),
        };
    }

    internal string? GetMqttServerAndPort(out int? mqttServerPort, string mqttUrl)
    {
        var mqttServerIncludingPort = mqttUrl;
        var mqttServerAndPort = mqttServerIncludingPort?.Split(":");
        var mqttServer = mqttServerAndPort?.FirstOrDefault();
        mqttServerPort = null;
        if (mqttServerAndPort != null && mqttServerAndPort.Length > 1)
        {
            mqttServerPort = Convert.ToInt32(mqttServerAndPort[1]);
        }

        return mqttServer;
    }


    
}

public class TeslaMateValue
{
    public int CarId { get; set; }
    public string? Topic { get; set; }
    public string? Value { get; set; }
}