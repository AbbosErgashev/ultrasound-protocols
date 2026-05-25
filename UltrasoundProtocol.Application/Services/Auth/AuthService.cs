using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UltrasoundProtocol.Application.DTOs.Auth;
using UltrasoundProtocol.Application.Settings;
using UltrasoundProtocol.Domain.Interfaces;

namespace UltrasoundProtocol.Application.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly StaticUserSettings _staticUsers;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUnitOfWork unitOfWork,
        IOptions<StaticUserSettings> staticUsers,
        ILogger<AuthService> logger)
    {
        _unitOfWork = unitOfWork;
        _staticUsers = staticUsers.Value;
        _logger = logger;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        _logger.LogDebug("Autentifikatsiya jarayoni boshlandi: {Username}", request.Username);

        // 1. Statik foydalanuvchilar (Doctor, SeniorDoctor, ChiefDoctor, ContentManager)
        var staticResult = CheckStaticUser(request.Username, request.Password);
        if (staticResult is not null)
        {
            _logger.LogInformation("Statik foydalanuvchi tizimga kirdi: {Username}, Rol: {Role}",
                request.Username, staticResult.Role);
            return staticResult;
        }

        // 2. DB dan bemor — Username bo'yicha qidirish
        var users = await _unitOfWork.Users.FindAsync(
            u => u.Username == request.Username && u.IsActive);
        var user = users.FirstOrDefault();

        if (user is null)
        {
            _logger.LogWarning("Foydalanuvchi topilmadi: {Username}", request.Username);
            return null;
        }

        if (request.Password != user.PasswordHash)
        {
            _logger.LogWarning("Noto'g'ri parol: {Username} (UserId: {UserId})", request.Username, user.Id);
            return null;
        }

        _logger.LogInformation("Bemor tizimga kirdi: {FullName} (UserId: {UserId})", user.FullName, user.Id);

        return new LoginResponse
        {
            Username = user.Username,
            FullName = user.FullName,
            Role = user.Role.ToString(),
            UserId = user.Id
        };
    }

    private LoginResponse? CheckStaticUser(string username, string password)
    {
        StaticUserEntry[] entries =
        [
            _staticUsers.Doctor,
            _staticUsers.SeniorDoctor,
            _staticUsers.ChiefDoctor,
            _staticUsers.ContentManager
        ];

        foreach (var entry in entries)
        {
            if (entry.Username == username && entry.Password == password)
            {
                return new LoginResponse
                {
                    Username = entry.Username,
                    FullName = entry.FullName,
                    Role = entry.Role,
                    UserId = null
                };
            }
        }

        return null;
    }
}
