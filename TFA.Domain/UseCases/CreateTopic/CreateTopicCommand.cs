using MediatR;
using TFA.Domain.Models;

namespace TFA.Domain.UseCases.CreateTopic;

public record CreateTopicCommand(Guid ForumId, string Title) : IRequest<Topic>;