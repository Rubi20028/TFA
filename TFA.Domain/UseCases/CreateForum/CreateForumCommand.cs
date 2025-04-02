using MediatR;
using TFA.Domain.Models;

namespace TFA.Domain.UseCases.CreateForum;

public record CreateForumCommand(string Title) : IRequest<Forum>;