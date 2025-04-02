using System.Globalization;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TFA.Domain.Authentication;

internal class AuthenticationService : IAuthenticationService
{
    private readonly AuthenticationConfiguration configuration;
    private readonly IAuthenticationStorage storage;
    private readonly ILogger<AuthenticationService> logger;
    private readonly ISymmetricDecryptor decryptor;

    public AuthenticationService(
        IOptions<AuthenticationConfiguration> options, 
        IAuthenticationStorage storage,
        ILogger<AuthenticationService> logger,
        ISymmetricDecryptor decryptor)
    {
        this.storage = storage;
        this.logger = logger;
        this.decryptor = decryptor;
        configuration = options.Value;
    }

    public async Task<IIdentity> Authenticate(string authToken, CancellationToken cancellationToken)
    {
        string sessionIdString;
        try
        {
            sessionIdString = await decryptor.Decrypt(authToken, configuration.Key, cancellationToken);
        }
        catch (CryptographicException cryptographicException)
        {
            logger.LogWarning(cryptographicException, "Unable to decrypt token.");
            return User.Guest;
        }
        
        if (Guid.TryParse(sessionIdString, out var sessionId))
        {
            return User.Guest;
        }

        var session = await storage.FindSession(sessionId, cancellationToken);
        
        if (session is null)
        {
            return User.Guest;
        }

        if (session.ExpiresAt < DateTimeOffset.UtcNow)
        {
            return User.Guest;
        }
        
        return new User(session.UserId, sessionId);
    }
}