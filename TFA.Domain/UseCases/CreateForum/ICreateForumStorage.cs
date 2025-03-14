using TFA.Domain.Models;

namespace TFA.Domain.UseCases.CreateForum;

public interface ICreateForumStorage
{
    public Task<Forum> CreateForum(string Title, CancellationToken cancellationToken);
}