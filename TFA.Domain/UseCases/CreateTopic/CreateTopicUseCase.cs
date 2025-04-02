using FluentValidation;
using MediatR;
using TFA.Domain.Authentication;
using TFA.Domain.Authorization;
using TFA.Domain.Monitoring;
using TFA.Domain.UseCases.GetForums;
using Topic = TFA.Domain.Models.Topic;

namespace TFA.Domain.UseCases.CreateTopic;

internal class CreateTopicUseCase : IRequestHandler<CreateTopicCommand, Topic>
{
    private readonly IValidator<CreateTopicCommand> validator;
    private readonly IIntentionManager intentionManager;
    private readonly IIdentityProvider identityProvider;
    private readonly IGetForumsStorage getForumsStorage; 
    private readonly ICreateTopicStorage storage;
    private readonly DomainMetrics metrics;
    public CreateTopicUseCase(
        IValidator<CreateTopicCommand> validator,
        IIntentionManager intentionManager,
        IIdentityProvider identityProvider,
        IGetForumsStorage getForumsStorage,
        ICreateTopicStorage storage, 
        DomainMetrics metrics)
    {
        this.validator = validator;
        this.intentionManager = intentionManager;
        this.identityProvider = identityProvider;
        this.getForumsStorage = getForumsStorage;
        this.storage = storage;
        this.metrics = metrics;
    }
    public async Task<Topic> Handle(CreateTopicCommand command, CancellationToken cancellationToken)
    {
        try
        {
            await validator.ValidateAndThrowAsync(command, cancellationToken);
            var (forumId, title) = command; // Деконструктор называется чтоб не писать (( command.ForumId, command.Title ))
            intentionManager.ThrowIfForbidden(TopicIntention.Create);
            await getForumsStorage.ThrowIfForumNotFound(forumId, cancellationToken);
            var result = await storage.CreateTopic(forumId, identityProvider.Current.UserId, title, cancellationToken);
            metrics.TopicCreated(true); 
            return result;
        }
        catch
        {
            metrics.TopicCreated(false);
            throw;
        }
    }
}