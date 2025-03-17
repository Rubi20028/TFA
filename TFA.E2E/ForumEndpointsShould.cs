using System.Net.Http.Json;
using FluentAssertions;

namespace E2E;

public class ForumEndpointsShould : IClassFixture<ForumApiApplicationFactory>
{
    private readonly ForumApiApplicationFactory factory;

    public ForumEndpointsShould(ForumApiApplicationFactory factory)
    {
        this.factory = factory;
    }
    
    [Fact]
    public async Task CreateNewForum()
    {
        using var httpClient = factory.CreateClient();
        using var response = await httpClient.PostAsync("forums", 
            JsonContent.Create(new{title ="Test"}));
        
        response.Invoking(r=> r.EnsureSuccessStatusCode()).Should().NotThrow();
    }
}