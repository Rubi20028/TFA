using FluentValidation;
using TFA.Domain.Authorization;
using TFA.Domain.Models;

namespace TFA.Domain.UseCases.CreateForum;

internal class CreateForumUseCase : ICreateForumUseCase
{
    private readonly IValidator<CreateForumCommand> validator;
    private readonly IntentionManager intentionManager;
    private readonly ICreateForumStorage storage;
    
    public CreateForumUseCase(
        IValidator<CreateForumCommand> validator,
        IntentionManager intentionManager,
        ICreateForumStorage storage)
    {
        this.validator = validator;
        this.intentionManager = intentionManager;
        this.storage = storage;
    }
    
    public async Task<Forum> Execute(CreateForumCommand command, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);
        
        intentionManager.ThrowIfForbidden(ForumIntention.Create);
        
        return await storage.CreateForum(command.Title, cancellationToken);
    }
}