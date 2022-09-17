using TeslaUsbKeepCarAwake.Dtos;
using TeslaUsbKeepCarAwake.Services.Contracts;

namespace TeslaUsbKeepCarAwake.Services;

public class TeslaMateService : ITeslaMateService
{
    private readonly ILogger<TeslaMateService> _logger;
    private readonly CarState _carState;
    private readonly Settings _settings;
    private readonly Internals _internals;

    public TeslaMateService(ILogger<TeslaMateService> logger, CarState carState, Settings settings, Internals internals)
    {
        _logger = logger;
        _carState = carState;
        _settings = settings;
        _internals = internals;
    }

    public async Task KeepCarAwake()
    {
        _logger.LogTrace("{method}()",  nameof(KeepCarAwake));
        var completedDateTime = File.GetCreationTime(_settings.ArchiveUploadPath);
        _logger.LogDebug("Last Completed Time: {time}", completedDateTime);
        var relevantState = "suspended";
        _logger.LogDebug("Car State: {state}", _carState.State);
        _logger.LogDebug("Car Home Since: {homeSince}", _carState.HomeGeofenceSince);
        _logger.LogDebug("ApplicationStartup: {startup}", _internals.ApplicationStartup);
        if (string.Equals(_carState.State, relevantState, StringComparison.CurrentCultureIgnoreCase) 
            && _carState.Geofence == _settings.RelevantGeofence
            && completedDateTime < _carState.HomeGeofenceSince
            && _internals.ApplicationStartup < _carState.HomeGeofenceSince.Value.AddMinutes(-1))
        {
            await ResumeLogging().ConfigureAwait(false);
        }
    }

    private async Task ResumeLogging()
    {
        _logger.LogTrace("{method}()", nameof(ResumeLogging));
        var url = $"{_settings.TeslaMateBaseUrl}/api/car/{_settings.CarId}/logging/resume";
        _logger.LogDebug("Url: {url}", url);
        using var httpClient = new HttpClient();
        var response = await httpClient.PutAsync(url, null).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
    }
}