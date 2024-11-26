using System.Threading;
using System.Threading.Tasks;

namespace NetCoreExampleApi.Services.Interfaces;

public interface IOutboxMessagePublisherService
{
    Task Publish(CancellationToken cancellationToken);
}