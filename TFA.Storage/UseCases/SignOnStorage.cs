using TFA.Domain.UseCases.SignOn;
using TFA.Storage.Entities;

namespace TFA.Storage.UseCases;

internal class SignOnStorage : ISignOnStorage
{
    private readonly ForumDbContext forumDbContext;
    private readonly IGuidFactory guidFactory;

    public SignOnStorage(ForumDbContext forumDbContext, IGuidFactory guidFactory)
    {
        this.forumDbContext = forumDbContext;
        this.guidFactory = guidFactory;
    }

    public async Task<Guid> CreateUser(string login, byte[] salt, byte[] hash, CancellationToken cancellationToken)
    {
        var userId = guidFactory.Create();
        await forumDbContext.AddAsync(new User
        {
            UserId = userId,
            Login = login,
            Salt = salt,
            PasswordHash = hash
        }, cancellationToken);
        await forumDbContext.SaveChangesAsync(cancellationToken);

        return userId;
    }
    
}