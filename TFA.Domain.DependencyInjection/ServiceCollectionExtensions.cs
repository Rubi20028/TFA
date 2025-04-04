﻿using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TFA.Domain.Authentication;
using TFA.Domain.Authorization;
using TFA.Domain.Models;
using TFA.Domain.Monitoring;
using TFA.Domain.UseCases.CreateForum;
using TFA.Domain.UseCases.CreateTopic;

namespace TFA.Domain.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddForumDomain(this IServiceCollection services)
    {
        services
            .AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Forum>());
            
        services
            .AddScoped<IIntentionManager, IntentionManager>()
            .AddScoped<IIntentionResolver, ForumIntentionResolver>()
            .AddScoped<IIntentionResolver, TopicIntentionResolver>();
        
        services
            .AddScoped<IIdentityProvider, IdentityProvider>()
            .AddScoped<IPasswordManager, PasswordManager>()
            .AddScoped<IAuthenticationService, AuthenticationService>()
            .AddScoped<ISymmetricDecryptor, AesSymmetricDecryptorEncryptor>()
            .AddScoped<ISymmetricEncryptor, AesSymmetricDecryptorEncryptor>();
        
        services
            .AddValidatorsFromAssemblyContaining<Forum>(includeInternalTypes: true);

        services.AddSingleton<DomainMetrics>();
        
        return services;
    }
}