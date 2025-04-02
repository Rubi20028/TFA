namespace TFA.Domain.Authentication;

internal interface ISymmetricEncryptor
{
    Task<string> Encrypt(string plainText, byte[] key, CancellationToken cancellationToken);
}

class SymmetricEncryptor : ISymmetricEncryptor
{
    public Task<string> Encrypt(string plainText, byte[] key, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}