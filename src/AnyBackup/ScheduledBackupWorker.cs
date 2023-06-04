namespace AnyBackup;

public class ScheduledBackupWorker : BackgroundService
{
    private readonly ILogger<ScheduledBackupWorker> _logger;
    private readonly IFileService _fileService;

    public ScheduledBackupWorker(ILogger<ScheduledBackupWorker> logger, IFileService fileService)
    {
        _logger = logger;
        _fileService = fileService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _fileService.DoAsync();
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }

}
