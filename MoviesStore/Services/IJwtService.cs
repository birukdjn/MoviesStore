using MoviesStore.models;

namespace MoviesStore.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
