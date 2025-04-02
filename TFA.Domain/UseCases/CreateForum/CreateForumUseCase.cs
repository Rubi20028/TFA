using FluentValidation;
using MediatR;
using TFA.Domain.Authorization;
using TFA.Domain.Models;
using TFA.Domain.Monitoring;

namespace TFA.Domain.UseCases.CreateForum;

internal class CreateForumUseCase : IRequestHandler<CreateForumCommand, Forum>
{
    private readonly IValidator<CreateForumCommand> validator;
    private readonly IIntentionManager intentionManager;
    private readonly ICreateForumStorage storage;
    private readonly DomainMetrics metrics;
    
    public CreateForumUseCase(
        IValidator<CreateForumCommand> validator,
        IIntentionManager intentionManager,
        ICreateForumStorage storage, DomainMetrics metrics)
    {
        this.validator = validator;
        this.intentionManager = intentionManager;
        this.storage = storage;
        this.metrics = metrics;
    }

    public async Task<Forum> Handle(CreateForumCommand command, CancellationToken cancellationToken)
    {
        try
        {
            await validator.ValidateAndThrowAsync(command, cancellationToken);
            intentionManager.ThrowIfForbidden(ForumIntention.Create);
            var result = await storage.Create(command.Title, cancellationToken);
            metrics.ForumCreated(true);
            return result;
        }
        catch 
        {
            metrics.ForumCreated(false);
            throw;
        }
    }
}