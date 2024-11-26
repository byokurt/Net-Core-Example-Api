using System.Threading.Tasks;
using DotnetCoreExampleApi.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace DotnetCoreExampleApi.Consumers;

public class DemoConsumer : IConsumer<DemoEvent>
{
    private readonly ILogger<DemoConsumer> _logger;

    public DemoConsumer(ILogger<DemoConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<DemoEvent> context)
    {
        _logger.LogInformation("Running Consumer");

        await Task.CompletedTask;
    }
}