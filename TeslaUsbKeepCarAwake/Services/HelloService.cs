using Newtonsoft.Json;
using TeslaUsbKeepCarAwake.Dtos;

namespace TeslaUsbKeepCarAwake.Services;

public class HelloService
{
    private readonly ILogger<HelloService> _logger;
    private readonly Settings _settings;

    public HelloService(ILogger<HelloService> logger, Settings settings)
    {
        _logger = logger;
        _settings = settings;
    }

    public async Task UpdateSettings(Settings settings)
    {
        _logger.LogTrace("{method}({@settings})", nameof(UpdateSettings), settings);
        _settings.ArchiveUploadPath = settings.ArchiveUploadPath;
        _settings.CarId = settings.CarId;
        _settings.MqttUrl = settings.MqttUrl;
        _settings.RelevantGeofence = settings.RelevantGeofence;
        _settings.TeslaMateBaseUrl = settings.TeslaMateBaseUrl;

        var serializedSettings = JsonConvert.SerializeObject(_settings);
        await File.WriteAllTextAsync("settings.json", serializedSettings);
    }
}