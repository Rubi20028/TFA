using TFA.Domain.UseCases.SignOut;

namespace TFA.Storage.UseCases;

public class SignOutStorage : ISignOutStorage
{
    public Task RemoveSession(Guid sessionId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}