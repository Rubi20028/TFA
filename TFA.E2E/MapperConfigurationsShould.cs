using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace E2E;

public class MapperConfigurationsShould : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> factory;

    public MapperConfigurationsShould(WebApplicationFactory<Program> factory)
    {
        this.factory = factory;
    }

    [Fact]
    public void BeValid()
    {
        factory.Services.GetRequiredService<IMapper>()
            .ConfigurationProvider.Invoking(p=>p.AssertConfigurationIsValid())
            .Should().NotThrow();
    }
}