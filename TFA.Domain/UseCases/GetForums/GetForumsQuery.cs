using MediatR;
using TFA.Domain.Models;

namespace TFA.Domain.UseCases.GetForums;

public record GetForumsQuery : IRequest<IEnumerable<Forum>>;