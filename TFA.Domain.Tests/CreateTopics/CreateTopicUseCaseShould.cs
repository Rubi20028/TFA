using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Moq.Language.Flow;
using TFA.Domain.Authentication;
using TFA.Domain.Authorization;
using TFA.Domain.Exceptions;
using TFA.Domain.Models;
using TFA.Domain.Monitoring;
using TFA.Domain.UseCases.CreateTopic;
using TFA.Domain.UseCases.GetForums;
using Topic = TFA.Domain.Models.Topic;

namespace TFA.Domain.Tests.CreateTopics;

public class CreateTopicUseCaseShould       
{
    private readonly CreateTopicUseCase sut;
    private readonly Mock<ICreateTopicStorage> storage;
    private readonly ISetup<ICreateTopicStorage, Task<Topic>> createTopicSetup;
    private readonly ISetup<IIdentity,Guid> getCurrentUserIdSetup;
    private readonly ISetup<IIntentionManager, bool> intentionIsAllowedSetup;
    private readonly Mock<IIntentionManager> intentionManager;
    private readonly Mock<IGetForumsStorage> getForumStorage;
    private readonly ISetup<IGetForumsStorage, Task<IEnumerable<Forum>>> getForumsSetup;

    public CreateTopicUseCaseShould()
    {
        storage = new Mock<ICreateTopicStorage>();
        
        createTopicSetup = storage.Setup(s =>
            s.CreateTopic(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()));

        getForumStorage = new Mock<IGetForumsStorage>();
        getForumsSetup = getForumStorage.Setup(s => s.GetForums(It.IsAny<CancellationToken>()));
        
        var identity = new Mock<IIdentity>();
        var identityProvider = new Mock<IIdentityProvider>();
        identityProvider.Setup(p => p.Current).Returns(identity.Object);
        getCurrentUserIdSetup = identity.Setup(s=>s.UserId);

        intentionManager = new Mock<IIntentionManager>();
        intentionIsAllowedSetup = intentionManager.Setup(m => m.IsAllowed(It.IsAny<TopicIntention>()));
        
        var validator = new Mock<IValidator<CreateTopicCommand>>();
        validator.Setup(v=> v.ValidateAsync(It.IsAny<CreateTopicCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult()); // чтобы вернуть успешный успех используй (( ValidatorResult ))
        
        sut = new(validator.Object, intentionManager.Object, identityProvider.Object,getForumStorage.Object, storage.Object, new DomainMetrics());
    }

    
    [Fact]
    public async Task ThrowIntentionManagerException_WhenTopicCreationIsNotAllowed()
    {
        var forumId = Guid.Parse("4AFACF96-0C8E-4238-A2C6-B27A119C6F19");

        intentionIsAllowedSetup.Returns(false);
        
        await sut.Invoking(s => s.Handle(new CreateTopicCommand(forumId, "Whatever"), CancellationToken.None))
            .Should().ThrowAsync<IntentionManagerException>();
        intentionManager.Verify(m => m.IsAllowed(TopicIntention.Create));
    }
    
    
    [Fact]
    public async Task ThrowForumNotFoundException_WhenNoMatchingForum()
    {
        var forumId = Guid.Parse("3CA9380D-94BE-4EA1-A121-296C85CF8EA4");
        
        intentionIsAllowedSetup.Returns(true);
        getForumsSetup.ReturnsAsync(Array.Empty<Forum>());

        (await sut.Invoking(s => s.Handle(new CreateTopicCommand(forumId, "Some title"), CancellationToken.None))
            .Should().ThrowAsync<ForumNotFoundException>())
            .Which.DomainErrorCode.Should().Be(DomainErrorCode.Gone);
    }

    
    [Fact]
    public async Task ReturnNewlyCreatedTopicWhenMatchingForumExists()
    {
        var forumId = Guid.Parse("81802260-46FE-487B-8102-B839A9F4C005");
        var userId = Guid.Parse("AEB5ECF3-6AD2-4A86-948A-623E358DF9EA");
        
        intentionIsAllowedSetup.Returns(true);
        getForumsSetup.ReturnsAsync(new Forum[] {new() {Id = forumId}});
        getCurrentUserIdSetup.Returns(userId);
        var expected = new Topic();
        createTopicSetup.ReturnsAsync(expected);

        var actual = await sut.Handle(new CreateTopicCommand(forumId, "Hello world"), CancellationToken.None);
        actual.Should().Be(expected);
        
        storage.Verify(s=> s.CreateTopic(forumId, userId, "Hello world", It.IsAny<CancellationToken>()), Times.Once);
    }
}