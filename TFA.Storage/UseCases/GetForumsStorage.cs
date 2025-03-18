using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TFA.Domain.UseCases.GetForums;

namespace TFA.Storage.UseCases;

internal class GetForumsStorage : IGetForumsStorage
{
    private readonly IMemoryCache memoryCache;
    private readonly ForumDbContext forumDbContext;
    private readonly IMapper mapper;

    public GetForumsStorage(
        IMemoryCache memoryCache,
        ForumDbContext forumDbContext,
        IMapper mapper)
    {
        this.memoryCache = memoryCache;
        this.forumDbContext = forumDbContext;
        this.mapper = mapper;
    }

    public async Task<IEnumerable<Domain.Models.Forum>> GetForums(CancellationToken cancellationToken) =>
        await memoryCache.GetOrCreateAsync<Domain.Models.Forum[]>(
            nameof(GetForums),
            entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);
                return forumDbContext.Forums
                    .ProjectTo<Domain.Models.Forum>(mapper.ConfigurationProvider)                           // ProjectTo для использования IQueryable
                    .ToArrayAsync(cancellationToken);
            });
}