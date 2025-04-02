using System.Security.Cryptography;
using FluentAssertions;
using TFA.Domain.Authentication;
using Xunit.Abstractions;

namespace TFA.Domain.Tests.Authentication;

public class AesSymmetricEncryptorDecryptorShould
{
    private readonly ITestOutputHelper output;

    public AesSymmetricEncryptorDecryptorShould(
        ITestOutputHelper output)
    {
        this.output = output;
    }
    
    private readonly AesSymmetricDecryptorEncryptor sut = new();

    [Fact]
    public async Task ReturnMeaningfulEncryptedString()
    {
        var key = RandomNumberGenerator.GetBytes(32);
        var actual = await sut.Encrypt("Hello text", key, CancellationToken.None);
        actual.Should().NotBeEmpty();
    }
    
    [Fact]
    public async Task DecryptEncryptedString_WhenKeyIsSame()
    {
        var key = RandomNumberGenerator.GetBytes(32);
        var encryptedText = await sut.Encrypt("Hello text", key, CancellationToken.None);
        var decryptedText = await sut.Decrypt(encryptedText, key, CancellationToken.None);
        decryptedText.Should().Be("Hello text");
    }

    [Fact]
    public async Task ThrowException_WhenDecryptingWithDifferentKey()
    {
        var encryptedText = await sut.Encrypt("Hello text", RandomNumberGenerator.GetBytes(32), CancellationToken.None);
        await sut.Invoking(s=>s.Decrypt(encryptedText, RandomNumberGenerator.GetBytes(32), CancellationToken.None))
            .Should().ThrowAsync<CryptographicException>();
    }

    [Fact]
    private void GiveMeBase64Key()
    {
        output.WriteLine(Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)));
    }
}