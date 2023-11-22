namespace Y.Application.Services.Interfaces;

public interface IArgon2idPasswordHashAlgorithm
{ 
    string HashPassword(string password, string salt);

    bool VerifyHashedPassword(string password, string hash, string salt);

    string GenerateSalt();


}