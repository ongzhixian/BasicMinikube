

namespace BackgroundServiceWebApi.BackgroundServices;

public class BeanCounterService : BackgroundService
{
    private readonly ILogger<BeanCounterService> logger;

    public BeanCounterService(ILogger<BeanCounterService> logger)
    {
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        int i = 0;

        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine($"BeanCounterService {i++}");

            await Task.Delay(5000, stoppingToken);
        }
    }
}
