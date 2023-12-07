using Demo.Services;
using Microsoft.Extensions.Options;

namespace Demo;

public class Worker(ILogger<Worker> logger, IOptions<AppSettings> settings, IServiceProvider services)
    : BackgroundService
{
    private FileSystemWatcher? _folderWatcher;
    private readonly string _inputFolder = settings.Value.InputFolder;

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Service Starting");
        if (!Directory.Exists(_inputFolder))
        {
            logger.LogWarning("Please make sure the InputFolder [{inputFolder}] exists, then restart the service.", _inputFolder);
            return Task.CompletedTask;
        }

        logger.LogInformation("Binding Events from Input Folder: {inputFolder}", _inputFolder);
        _folderWatcher = new FileSystemWatcher(_inputFolder, "*.TXT")
        {
            NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.FileName |
                           NotifyFilters.DirectoryName
        };
        _folderWatcher.Created += Input_OnChanged;
        _folderWatcher.EnableRaisingEvents = true;

        return base.StartAsync(cancellationToken);
    }

    protected void Input_OnChanged(object source, FileSystemEventArgs e)
    {
        if (e.ChangeType == WatcherChangeTypes.Created)
        {
            logger.LogInformation("InBound Change Event Triggered by [{e.FullPath}]", e.FullPath);

            // do some work
            using (var scope = services.CreateScope())
            {
                var serviceA = scope.ServiceProvider.GetRequiredService<IServiceA>();
                serviceA.Run();
            }

            logger.LogInformation("Done with Inbound Change Event");
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping Service");
        if (_folderWatcher != null)
        {
            _folderWatcher.EnableRaisingEvents = false;
        }
        return base.StopAsync(cancellationToken);
    }

    public override void Dispose()
    {
        logger.LogInformation("Disposing Service");
        _folderWatcher?.Dispose();
        base.Dispose();
        GC.SuppressFinalize(this);
    }
}