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
        while (!stoppingToken.IsCancellationRequested)
        {
            await _fileService.DoAsync();
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(600000, stoppingToken);
        }
    }

}
