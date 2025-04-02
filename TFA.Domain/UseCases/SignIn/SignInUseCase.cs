using FluentValidation;
using MediatR;
using Microsoft.Extensions.Options;
using TFA.Domain.Authentication;

namespace TFA.Domain.UseCases.SignIn;

internal class SignInUseCase : IRequestHandler<SignInCommand, (IIdentity identity, string token)>
{
    private readonly IValidator<SignInCommand> validator;
    private readonly ISignInStorage storage;
    private readonly IPasswordManager passwordManager;
    private readonly ISymmetricEncryptor encryptor;
    private readonly AuthenticationConfiguration configuration;

    public SignInUseCase(
        IValidator<SignInCommand> validator,
        ISignInStorage storage,
        IPasswordManager passwordManager,
        ISymmetricEncryptor encryptor,
        IOptions<AuthenticationConfiguration> options)
    {
        this.validator = validator;
        this.storage = storage;
        this.passwordManager = passwordManager;
        this.encryptor = encryptor;
        configuration = options.Value;
    } 
    
    public async Task<(IIdentity identity, string token)> Handle(
        SignInCommand command, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);
        var recognisedUser = await storage.FindUser(command.Login, cancellationToken);
        if (recognisedUser is null)
        {
            throw new Exception();
        }

        var passwordMatches = passwordManager.ComparePassword(command.Password, recognisedUser.Salt, recognisedUser.PasswordHash);
        if (!passwordMatches)
        {
            throw new Exception();
        }

        var sessionId = await storage.CreateSession(recognisedUser.UserId, DateTimeOffset.UtcNow + TimeSpan.FromHours(1),
            cancellationToken);
        var token = await encryptor.Encrypt(sessionId.ToString(), configuration.Key, cancellationToken);
        return (new User(recognisedUser.UserId, sessionId), token);
    }
}