using FluentAssertions;
using TFA.Domain.UseCases.CreateTopic;

namespace TFA.Domain.Tests.CreateTopics;

public class CreateTopicCommandValidatorShould
{
    private readonly CreateTopicCommandValidator sut;

    public CreateTopicCommandValidatorShould()
    {
        sut = new CreateTopicCommandValidator();
    }

    [Fact]
    public void ReturnSuccess_WhenCommandIsValid()
    {
        var actual = sut.Validate(new CreateTopicCommand(Guid.Parse("7FCCD4C5-86C4-4673-9E77-9864B5A73E7C"), "Hello"));
        actual.IsValid.Should().BeTrue();
    }


    public static IEnumerable<object[]> GetInvalidCommands()
    {
        var validCommand = new CreateTopicCommand(Guid.Parse("3D07829B-1D2C-465E-9CA4-C60A06B68B94"), "Hello");
        
        yield return new object[] { validCommand with {ForumId = Guid.Empty}};
        yield return new object[] { validCommand with {Title = string.Empty}};
        yield return new object[] { validCommand with {Title = "      "}};
        yield return new object[] { validCommand with {Title = string.Join("a", Enumerable.Range(0,100))}};
    }
    

    [Theory]
    [MemberData(nameof(GetInvalidCommands))]
    public void ReturnFailure_WhenCommandIsInvalid(CreateTopicCommand command)
    {
        var actual = sut.Validate(command);
        actual.IsValid.Should().BeFalse();
    }
}