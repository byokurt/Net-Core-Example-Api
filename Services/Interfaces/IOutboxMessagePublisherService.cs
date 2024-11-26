using System.Threading;
using System.Threading.Tasks;

namespace DotnetCoreExampleApi.Services.Interfaces;

public interface IOutboxMessagePublisherService
{
    Task Publish(CancellationToken cancellationToken);
}