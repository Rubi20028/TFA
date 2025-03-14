using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Moq.Language.Flow;
using TFA.Domain.Exceptions;
using TFA.Domain.Models;
using TFA.Domain.UseCases.GetForums;
using TFA.Domain.UseCases.GetTopics;

namespace TFA.Domain.Tests.GetTopics;

public class GetTopicsUseCaseShould
{
    private readonly GetTopicsUseCase sut;
    private readonly Mock<IGetTopicsStorage> storage;
    private readonly ISetup<IGetTopicsStorage, Task<(IEnumerable<Topic> resources, int totalCount)>> getTopicSetup;
    private readonly ISetup<IGetForumsStorage, Task<IEnumerable<Forum>>> getForumsSetup;

    public GetTopicsUseCaseShould()
    {
        var validator = new Mock<IValidator<GetTopicsQuery>>();
        validator
            .Setup(v => v.ValidateAsync(It.IsAny<GetTopicsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var getForumsStorage = new Mock<IGetForumsStorage>();
        getForumsSetup = getForumsStorage.Setup(s => s.GetForums(It.IsAny<CancellationToken>()));
        
        storage = new Mock<IGetTopicsStorage>();
        getTopicSetup = storage.Setup(s => 
            s.GetTopics(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()));
        
        sut = new GetTopicsUseCase(validator.Object, getForumsStorage.Object, storage.Object );
    }

    [Fact]
    public async Task ThrowForumNotFoundException_WhenNoMatchingForum()
    {
        var forumId = Guid.Parse("6592F715-345C-48DE-B4E7-733EC386AD52");

        getForumsSetup.ReturnsAsync(new Forum[] { new() {Id = Guid.Parse("973C05EA-9A04-415B-A1B6-751FDD52DE41")} });

        var query = new GetTopicsQuery(forumId, 0, 1);
        await sut.Invoking(s => s.Execute(query, CancellationToken.None))
            .Should().ThrowAsync<ForumNotFoundException>();
    }
    
    [Fact]
    public async Task ReturnTopics_ExtractedFromStorages_WhenForumExists()
    {
        var forumId = Guid.Parse("ECBE6BBC-AEC8-4373-982C-999519379C5E");
        
        getForumsSetup.ReturnsAsync(new Forum[] { new() {Id = Guid.Parse("ECBE6BBC-AEC8-4373-982C-999519379C5E")} });
        var expectedResources = new Topic[] { new() };
        var expectedTotalCount = 6;
        getTopicSetup.ReturnsAsync((expectedResources, expectedTotalCount));
        
        var (actualResources, actualTotalCount) = await sut.Execute(
            new GetTopicsQuery(forumId, 5, 10), CancellationToken.None);

        actualResources.Should().BeEquivalentTo(expectedResources);
        actualTotalCount.Should().Be(expectedTotalCount);
        storage.Verify(s=>s.GetTopics(forumId, 5,10, It.IsAny<CancellationToken>()), Times.Once);
    }
}