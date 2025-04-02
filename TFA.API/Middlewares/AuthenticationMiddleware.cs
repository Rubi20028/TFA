using TFA.API.Authentication;
using TFA.Domain.Authentication;

namespace TFA.API.Middlewares;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate next;

    public AuthenticationMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(
        HttpContext httpcontext,
        IAuthTokenStorage tokenStorage,
        IAuthenticationService authenticationService,
        IIdentityProvider identityProvider,
        CancellationToken cancellationToken)
    {
        var identity = tokenStorage.TryExtract(httpcontext, out var authToken)
            ? await authenticationService.Authenticate(authToken, cancellationToken)
            : User.Guest;
        
        identityProvider.Current = identity;

        await next(httpcontext);
    }
}