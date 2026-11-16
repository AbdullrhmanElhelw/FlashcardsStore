namespace FlashcardsStore.Application.Interfaces.Services;

public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string enteredPassword, string storedHash);
}
