using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Moq.Language.Flow;
using TFA.Domain.Models;
using TFA.Domain.UseCases.GetTopics;

namespace TFA.Domain.Tests.GetTopics;

public class GetTopicsUseCaseShould
{
    private readonly GetTopicsUseCase sut;
    private readonly Mock<IGetTopicsStorage> storage;
    private readonly ISetup<IGetTopicsStorage, Task<(IEnumerable<Topic> resources, int totalCount)>> getTopicSetup;
    
    public GetTopicsUseCaseShould()
    {
        var validator = new Mock<IValidator<GetTopicsQuery>>();
        validator
            .Setup(v => v.ValidateAsync(It.IsAny<GetTopicsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        
        storage = new Mock<IGetTopicsStorage>();
        getTopicSetup = storage.Setup(s => 
            s.GetTopics(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()));
        
        sut = new GetTopicsUseCase(validator.Object, storage.Object);
    }

    [Fact]
    public async Task ReturnTopics_ExtractedFromStorages()
    {
        var forumId = Guid.Parse("ECBE6BBC-AEC8-4373-982C-999519379C5E");

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