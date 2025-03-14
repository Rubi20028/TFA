using TFA.Domain.Models;

namespace TFA.Domain.UseCases.GetTopics;

public interface IGetTopicsUseCase
{
    Task<(IEnumerable<Topic> resources, int totalCount)> Execute(
        GetTopicsQuery getTopicsQuery, 
        CancellationToken cancellationToken);
}