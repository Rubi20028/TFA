using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TFA.Domain.UseCases.CreateForum;

namespace TFA.Storage.UseCases;

internal class CreateForumStorage : ICreateForumStorage
{
    private readonly IMemoryCache memoryCache;
    private readonly IGuidFactory guidFactory;
    private readonly IMapper mapper;
    private readonly ForumDbContext forumDbContext;

    public CreateForumStorage(
        IMemoryCache memoryCache,
        ForumDbContext forumDbContext, 
        IGuidFactory guidFactory,
        IMapper mapper
       )
    {
        this.memoryCache = memoryCache;
        this.forumDbContext = forumDbContext;
        this.guidFactory = guidFactory;
        this.mapper = mapper;
    }

    public async Task<Domain.Models.Forum> Create(string title, CancellationToken cancellationToken)
    {
        var forumId = guidFactory.Create();
        
        var forum = new Forum
        {
            ForumId = forumId,
            Title = title
        };
        await forumDbContext.Forums.AddAsync(forum, cancellationToken);
        await forumDbContext.SaveChangesAsync(cancellationToken);

        memoryCache.Remove(nameof(GetForumsStorage.GetForums));
        
        return await forumDbContext.Forums
            .Where(f => f.ForumId == forumId)
            .ProjectTo<Domain.Models.Forum>(mapper.ConfigurationProvider)                                                 // ProjectTo для использования IQueryable
            .FirstAsync(cancellationToken);
    }
}