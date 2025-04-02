using MediatR;
using TFA.Domain.Monitoring;
using Forum = TFA.Domain.Models.Forum;

namespace TFA.Domain.UseCases.GetForums;

internal class GetForumsUseCase : IRequestHandler<GetForumsQuery, IEnumerable<Forum>>
{
    private readonly IGetForumsStorage storage;
    private readonly DomainMetrics metrics;

    public GetForumsUseCase(
        IGetForumsStorage storage,
        DomainMetrics metrics)
    {
        this.storage = storage;
        this.metrics = metrics;
    }

    public async Task<IEnumerable<Forum>> Handle(GetForumsQuery getForumsQuery, CancellationToken cancellationToken)
    {
        try
        {
            var result = await storage.GetForums(cancellationToken);
            metrics.ForumFetched(true);
            return result;
        }
        catch
        {
            metrics.ForumFetched(false);
            throw;
        }
    }
}