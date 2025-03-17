using Microsoft.EntityFrameworkCore;
using TFA.Domain.UseCases.CreateForum;

namespace TFA.Storage.UseCases;

internal class CreateForumStorage : ICreateForumStorage
{
    private readonly IGuidFactory guidFactory;
    private readonly ForumDbContext forumDbContext;
 

    public CreateForumStorage(ForumDbContext forumDbContext, IGuidFactory guidFactory)
    {
        this.forumDbContext = forumDbContext;
        this.guidFactory = guidFactory;
    }

    public async Task<Domain.Models.Forum> CreateForum(string Title, CancellationToken cancellationToken)
    {
        var forumId = guidFactory.Create();
        
        var forum = new Forum
        {
            ForumId = forumId,
            Title = Title
        };
        await forumDbContext.Forums.AddAsync(forum, cancellationToken);
        await forumDbContext.SaveChangesAsync(cancellationToken);

        return await forumDbContext.Forums
            .Where(f => f.ForumId == forumId)
            .Select(f => new Domain.Models.Forum
            {
                Id = f.ForumId,
                Title = f.Title
            })
            .FirstAsync(cancellationToken);
    }
}