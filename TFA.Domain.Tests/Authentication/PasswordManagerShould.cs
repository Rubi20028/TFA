using FluentAssertions;
using TFA.Domain.Authentication;

namespace TFA.Domain.Tests.Authentication;

public class PasswordManagerShould
{
    private readonly PasswordManager sut = new();
    private static readonly byte[] emptySalt = Enumerable.Repeat((byte)0, 100).ToArray();
    private static readonly byte[] emptyHash = Enumerable.Repeat((byte)0, 32).ToArray();

    [Theory]
    [InlineData("qwerty")]
    [InlineData("qwerty123")]
    public void GenerateMeaningfulSaltAndHash(string password)
    {
        var (salt,hash) = sut.GeneratePasswordParts(password);
        salt.Should().HaveCount(100).And.NotBeEquivalentTo(emptySalt);
        hash.Should().HaveCount(32).And.NotBeEquivalentTo(emptyHash);
    }

    [Fact]
    public void ReturnTrue_WhenPasswordIsMatch()
    {
        var password = "qwerty123";
        var (salt, hash) = sut.GeneratePasswordParts(password);
        sut.ComparePassword(password, salt, hash).Should().BeTrue();
    }
    
    [Fact]
    public void ReturnTrue_WhenPasswordDoesntMatch()
    {
        var (salt, hash) = sut.GeneratePasswordParts("qwerty123");
        sut.ComparePassword("NotMatchPassword", salt, hash).Should().BeFalse();
    }
} 