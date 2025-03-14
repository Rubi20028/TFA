using FluentAssertions;
using TFA.Domain.UseCases.GetTopics;

namespace TFA.Domain.Tests.GetTopics;

public class GetTopicsQueryValidatorShould
{
    private readonly GetTopicsQueryValidator sut = new();

    [Fact]
    public void ReturnSuccess_WhenQueryIsValid()
    {
        var query = new GetTopicsQuery(
            Guid.Parse("6C39FDF2-0B33-4E51-AC25-362D87B033D0"),
            10,
            5);
        sut.Validate(query).IsValid.Should().BeTrue();
    }
    
    public static IEnumerable<object[]> GetInvalidQuery()
    {
        var query = new GetTopicsQuery(Guid.Parse("6C39FDF2-0B33-4E51-AC25-362D87B033D0"), 10, 5);
        
        yield return new object[] { query with {ForumId = Guid.Empty}};
        yield return new object[] { query with {Skip = -40}};
        yield return new object[] { query with {Take = -40}};
    }
    
    [Theory]
    [MemberData(nameof(GetInvalidQuery))]
    public void ReturnFailure_WhenQueryIsInvalid(GetTopicsQuery query)
    {
        var actual = sut.Validate(query);
        actual.IsValid.Should().BeFalse();
    }
}