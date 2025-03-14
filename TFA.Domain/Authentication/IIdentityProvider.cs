using TFA.Domain.Authentication;

namespace TFA.Domain.Identity;

public interface IIdentityProvider
{
    IIdentity Current { get; }
}