using TFA.Domain.Identity;

namespace TFA.Domain.Authentication;

internal class IdentityProvider : IIdentityProvider
{
    public IIdentity Current => new User(Guid.Parse("E8896A5B-116F-4BDE-9CAB-B28FBC33C69E"));
}