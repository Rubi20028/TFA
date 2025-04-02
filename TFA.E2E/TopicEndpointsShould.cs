using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TFA.Storage;
using Forum = TFA.API.Models.Forum;

namespace E2E;

public class TopicEndpointsShould : IClassFixture<ForumApiApplicationFactory>, IAsyncLifetime
{
    private readonly ForumApiApplicationFactory factory;
    private readonly Guid forumId = Guid.Parse("CA0C8D88-16C4-4B56-B748-BBE27330F36C");

    public TopicEndpointsShould(ForumApiApplicationFactory factory)
    {
        this.factory = factory;
    }
    
    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task ReturnForbidden_WhenNotAuthenticated()
    {
        using var httpClient = factory.CreateClient();

        using var forumCreatedResponse = await httpClient.PostAsync("forums", 
            JsonContent.Create(new { title = "test forum" }));

        forumCreatedResponse.EnsureSuccessStatusCode();

        var createdForum = await forumCreatedResponse.Content.ReadFromJsonAsync<Forum>();
        createdForum.Should().NotBeNull();
        
        var responseMessage = await httpClient.PostAsync($"forums/{createdForum!.Id}/topics", 
            JsonContent.Create(new { title = "Hello world" }));
        responseMessage.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }
}