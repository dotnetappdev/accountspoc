using AccountsPOC.Domain.Entities;

namespace AccountsPOC.WebAPI.Services;

public interface IJwtTokenService
{
    Task<string> GenerateTokenAsync(User user);
}
