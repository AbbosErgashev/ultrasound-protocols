namespace UltrasoundProtocol.Application.Settings;

public class StaticUserSettings
{
    public StaticUserEntry Doctor { get; set; } = new();
    public StaticUserEntry SeniorDoctor { get; set; } = new();
    public StaticUserEntry ChiefDoctor { get; set; } = new();
    public StaticUserEntry ContentManager { get; set; } = new();
}

public class StaticUserEntry
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
