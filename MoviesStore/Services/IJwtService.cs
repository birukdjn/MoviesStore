using MoviesStore.models;

namespace MoviesStore.Services
{
    public interface IJwtService
    {
        string GenerateUserToken(User user);

        string GenerateProfileToken(Profile profile);
    }
}
