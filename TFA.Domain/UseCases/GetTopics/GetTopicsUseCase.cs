using FluentValidation;
using TFA.Domain.Models;

namespace TFA.Domain.UseCases.GetTopics;

internal class GetTopicsUseCase : IGetTopicsUseCase
{
    private readonly IValidator<GetTopicsQuery> validator;
    private readonly IGetTopicsStorage storage;

    public GetTopicsUseCase(
        IValidator<GetTopicsQuery> validator,
        IGetTopicsStorage storage)
    {
        this.validator = validator;
        this.storage = storage;
    }
    
    public async Task<(IEnumerable<Topic> resources, int totalCount)> Execute(
        GetTopicsQuery query, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(query, cancellationToken);
        return await storage.GetTopics(query.ForumId, query.Skip, query.Take, cancellationToken);
    }
}