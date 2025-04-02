namespace TFA.Domain.Authentication;

internal interface IPasswordManager
{
    bool ComparePassword(string password, byte[] salt, byte[] hash);
    
    (byte[] salt, byte[] hash) GeneratePasswordParts(string password);
}