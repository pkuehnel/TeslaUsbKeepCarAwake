using Quartz.Spi;
using Quartz;
using TeslaUsbKeepCarAwake.Dtos;

namespace TeslaUsbKeepCarAwake.Scheduling;

public class JobManager
{
    private readonly ILogger<JobManager> _logger;
    private readonly IJobFactory _jobFactory;
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly Settings _settings;

    private IScheduler _scheduler;


#pragma warning disable CS8618
    public JobManager(ILogger<JobManager> logger, IJobFactory jobFactory, ISchedulerFactory schedulerFactory, Settings settings)
#pragma warning restore CS8618
    {
        _logger = logger;
        _jobFactory = jobFactory;
        _schedulerFactory = schedulerFactory;
        _settings = settings;
    }

    public async Task StartJobs()
    {
        _logger.LogTrace("{Method}()", nameof(StartJobs));
        _scheduler = _schedulerFactory.GetScheduler().GetAwaiter().GetResult();
        _scheduler.JobFactory = _jobFactory;

        var wakeUpJob = JobBuilder.Create<WakeUpJob>().Build();
        

        var wakeUpTrigger =
            TriggerBuilder.Create().WithSchedule(SimpleScheduleBuilder.RepeatSecondlyForever(_settings.JobIntervallSeconds)).Build();

        var triggersAndJobs = new Dictionary<IJobDetail, IReadOnlyCollection<ITrigger>>
        {
            {wakeUpJob,  new HashSet<ITrigger> { wakeUpTrigger }},
        };

        await _scheduler.ScheduleJobs(triggersAndJobs, false).ConfigureAwait(false);

        await _scheduler.Start().ConfigureAwait(false);
    }

}