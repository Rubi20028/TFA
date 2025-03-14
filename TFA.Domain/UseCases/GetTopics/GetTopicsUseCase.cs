using FluentValidation;
using TFA.Domain.Models;
using TFA.Domain.UseCases.GetForums;

namespace TFA.Domain.UseCases.GetTopics;

internal class GetTopicsUseCase : IGetTopicsUseCase
{
    private readonly IValidator<GetTopicsQuery> validator;
    private readonly IGetForumsStorage getForumsStorage;
    private readonly IGetTopicsStorage storage;

    public GetTopicsUseCase(
        IValidator<GetTopicsQuery> validator,
        IGetForumsStorage getForumsStorage,
        IGetTopicsStorage storage
        )
    {
        this.validator = validator;
        this.getForumsStorage = getForumsStorage;
        this.storage = storage;
    }
    
    public async Task<(IEnumerable<Topic> resources, int totalCount)> Execute(
        GetTopicsQuery query, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(query, cancellationToken);                                              // Используется метод (( ValidateAndThrowAsync )) из FluentValidation. Если валидация не проходит, выбрасывается исключение ValidationException.
        await getForumsStorage.ThrowIfForumNotFound(query.ForumId, cancellationToken);                                // Метод (( ThrowIfForumNotFound проверяет )), существует ли форум с указанным ForumId. Если форум не найден, выбрасывается исключение.
        return await storage.GetTopics(query.ForumId, query.Skip, query.Take, cancellationToken);
    }
}