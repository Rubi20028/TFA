using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TFA.Domain.UseCases.GetForums;

namespace TFA.Storage.UseCases;

internal class GetForumStorage : IGetForumsStorage
{
    private readonly ForumDbContext forumDbContext;
    private readonly IMemoryCache memoryCache;

    public GetForumStorage(
        ForumDbContext forumDbContext,
        IMemoryCache memoryCache)
    {
        this.forumDbContext = forumDbContext;
        this.memoryCache = memoryCache;
    }

    public async Task<IEnumerable<Domain.Models.Forum>> GetForums(CancellationToken cancellationToken) =>
        await memoryCache.GetOrCreateAsync<Domain.Models.Forum[]>(
            nameof(GetForums),
            entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);
                return forumDbContext.Forums
                    .Select(f => new Domain.Models.Forum
                    {
                        Id = f.ForumId,
                        Title = f.Title
                    })
                    .ToArrayAsync(cancellationToken);
            });
}