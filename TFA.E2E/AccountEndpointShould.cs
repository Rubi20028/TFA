using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Serilog;
using TFA.Domain.Authentication;

namespace E2E;

public class AccountEndpointShould : IClassFixture<ForumApiApplicationFactory>
{
    private readonly ForumApiApplicationFactory factory;
    
    public AccountEndpointShould(ForumApiApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task SignInAfterSignOn()
    {
        using var httpClient = factory.CreateClient();

        using var signOnResponse = await httpClient.PostAsync("account",
            JsonContent.Create(new { login = "Test", password = "qwerty123" }));
        signOnResponse.IsSuccessStatusCode.Should().BeTrue();
        var createdUser = await signOnResponse.Content.ReadFromJsonAsync<User>();

        using var signInResponse = await httpClient.PostAsync(
            "account/signin", JsonContent.Create(new { login = "Test", password = "qwerty123" }));
        signInResponse.IsSuccessStatusCode.Should().BeTrue();
        signInResponse.Headers.Should().ContainKey("TFA-Auth-Token");
        
        var signedInUser = await signInResponse.Content.ReadFromJsonAsync<User>();
        signedInUser.Should()
            .NotBeNull().And
            .BeEquivalentTo(createdUser);
    }
}