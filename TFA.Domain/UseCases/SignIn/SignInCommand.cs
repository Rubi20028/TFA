using MediatR;
using TFA.Domain.Authentication;

namespace TFA.Domain.UseCases.SignIn;

public record SignInCommand(string Login, string Password) : IRequest<(IIdentity identity, string token)>;