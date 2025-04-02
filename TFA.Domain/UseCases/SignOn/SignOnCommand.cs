using MediatR;
using TFA.Domain.Authentication;

namespace TFA.Domain.UseCases.SignOn;

public record SignOnCommand(string Login, string Password) : IRequest<IIdentity>;