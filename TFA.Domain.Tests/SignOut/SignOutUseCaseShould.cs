using Moq;
using Moq.Language.Flow;
using TFA.Domain.Authentication;
using TFA.Domain.Authorization;
using TFA.Domain.UseCases.SignOut;

namespace TFA.Domain.Tests.SignOut;

public class SignOutUseCaseShould
{
    private readonly SignOutUseCase sut;
    private readonly Mock<ISignOutStorage> storage;
    private readonly ISetup<ISignOutStorage, Task> removeSessionSetup;
    private readonly ISetup<IIdentityProvider, IIdentity> currentIdentitySetup;

    public SignOutUseCaseShould()
    {
        storage = new Mock<ISignOutStorage>();
        removeSessionSetup = storage.Setup(s => s.RemoveSession(It.IsAny<Guid>(), It.IsAny<CancellationToken>()));
            
        var identityProvider = new Mock<IIdentityProvider>();
        currentIdentitySetup = identityProvider.Setup(p => p.Current);
        
        sut = new SignOutUseCase(
            new Mock<IntentionManager>().Object,
            identityProvider.Object,
            storage.Object);
    }

    
    [Fact]
    public async Task RemoveCurrentIdentity()
    {
        var sessionId = Guid.Parse("6F3B5035-1212-4854-9C55-2CC3149478DF");
        currentIdentitySetup.Returns(new User(Guid.Empty, sessionId));
        removeSessionSetup.Returns(Task.CompletedTask);
        
        await sut.Handle(new SignOutCommand(), CancellationToken.None);
        
        storage.Verify(s=>s.RemoveSession(sessionId, It.IsAny<CancellationToken>()), Times.Once);
        storage.VerifyNoOtherCalls();
    }
}