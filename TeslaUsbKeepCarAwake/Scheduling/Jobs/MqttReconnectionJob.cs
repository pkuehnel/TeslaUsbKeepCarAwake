using Quartz;
using TeslaUsbKeepCarAwake.Services.Contracts;

namespace TeslaUsbKeepCarAwake.Scheduling.Jobs;

[DisallowConcurrentExecution]
public class MqttReconnectionJob : IJob
{
    private readonly ILogger<MqttReconnectionJob> _logger;
    private readonly IMqttService _mqttService;

    public MqttReconnectionJob(ILogger<MqttReconnectionJob> logger, IMqttService mqttService)
    {
        _logger = logger;
        _mqttService = mqttService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogTrace("{method}({context})", nameof(Execute), context);
        await _mqttService.ConnectClientIfNotConnected().ConfigureAwait(false);
    }
}