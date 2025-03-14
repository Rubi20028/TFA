using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace E2E;

public class ForumEndpointsShould : IClassFixture<ForumApiApplicationFactory>
{
    private readonly ForumApiApplicationFactory factory;

    public ForumEndpointsShould(ForumApiApplicationFactory factory)
    {
        this.factory = factory;
    }
    
    [Fact]
    public async Task ReturnListOfForums()
    {
        using var httpClient = factory.CreateClient();
        using var response = await httpClient.GetAsync("forums");
        response.Invoking(r=> r.EnsureSuccessStatusCode()).Should().NotThrow();
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadAsStringAsync();
        result.Should().Be("[]");
    }
}