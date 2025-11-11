using Backend.models;

namespace Backend.Services 
{
    public interface IJwtService
    {
        string GenerateUserToken(User user);
        string GenerateRefreshToken();

        string GenerateProfileToken(Profile profile);
    }
}
