using MediatR;
using TFA.Domain.Models;

namespace TFA.Domain.UseCases.GetTopics;

public record GetTopicsQuery(Guid ForumId, int Skip, int Take)
    : IRequest<(IEnumerable<Topic> resources, int totalCount)>;