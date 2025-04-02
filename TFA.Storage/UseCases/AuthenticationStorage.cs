using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TFA.Domain.Authentication;

namespace TFA.Storage.UseCases;

internal class AuthenticationStorage : IAuthenticationStorage
{
    private readonly ForumDbContext dbContext;
    private readonly IMapper mapper;

    public AuthenticationStorage(ForumDbContext dbContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    public Task<Session?> FindSession(Guid sessionId, CancellationToken cancellationToken)
    {
        return dbContext.Sessions
            .Where(s => s.SessionId == sessionId)
            .ProjectTo<Session>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
    }
}