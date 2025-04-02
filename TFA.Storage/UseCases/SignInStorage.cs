using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TFA.Domain.UseCases.SignIn;
using TFA.Storage.Entities;

namespace TFA.Storage.UseCases;

internal class SignInStorage : ISignInStorage
{
    private readonly IMapper mapper;
    private readonly ForumDbContext forumDbContext;
    private readonly IGuidFactory guidFactory;

    public SignInStorage(
        ForumDbContext forumDbContext,
        IMapper mapper, IGuidFactory guidFactory)
    {
        this.mapper = mapper;
        this.guidFactory = guidFactory;
        this.forumDbContext = forumDbContext;
    }
    
    public Task<RecognisedUser?> FindUser(string login, CancellationToken cancellationToken)
    {
        return forumDbContext.Users
            .Where(u => u.Login.Equals(login))
            .ProjectTo<RecognisedUser>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Guid> CreateSession(
        Guid userId, DateTimeOffset expirationMoment, CancellationToken cancellationToken)
    {
        var sessionId = guidFactory.Create();

        await forumDbContext.Sessions.AddAsync(new Session
        {
            SessionId = sessionId,
            UserId = userId,
            ExpiresAt = expirationMoment
        }, cancellationToken);
        await forumDbContext.SaveChangesAsync(cancellationToken);
        
        return sessionId;
    }
}