using Quartz;
using TeslaUsbKeepCarAwake.Dtos;
using TeslaUsbKeepCarAwake.Services.Contracts;

namespace TeslaUsbKeepCarAwake.Scheduling;

[DisallowConcurrentExecution]
public class WakeUpJob : IJob
{
    private readonly ILogger<WakeUpJob> _logger;
    private readonly ITeslaMateService _service;
    private readonly Settings _settings;

    public WakeUpJob(ILogger<WakeUpJob> logger, ITeslaMateService service, Settings settings)
    {
        _logger = logger;
        _service = service;
        _settings = settings;
    }
    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogTrace("Executing Job to keep car awake");
        await _service.KeepCarAwake().ConfigureAwait(false);
    }
}