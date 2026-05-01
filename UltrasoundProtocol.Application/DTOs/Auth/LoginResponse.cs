namespace UltrasoundProtocol.Application.DTOs.Auth;

public class LoginResponse
{
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public Guid? UserId { get; set; }
}
