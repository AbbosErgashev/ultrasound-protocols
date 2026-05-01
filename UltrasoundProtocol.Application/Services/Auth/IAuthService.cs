using UltrasoundProtocol.Application.DTOs.Auth;

namespace UltrasoundProtocol.Application.Services.Auth;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
}
