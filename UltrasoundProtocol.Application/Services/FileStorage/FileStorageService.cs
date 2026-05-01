namespace UltrasoundProtocol.Application.Services.FileStorage;

public interface IFileStorageService
{
    Task<string> SaveImageAsync(byte[] data, string fileName);
    Task<byte[]?> GetImageAsync(string filePath);
    Task DeleteAsync(string filePath);
}

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _basePath;

    public LocalFileStorageService()
    {
        _basePath = Path.Combine(Directory.GetCurrentDirectory(), "storage", "images");
        Directory.CreateDirectory(_basePath);
    }

    public async Task<string> SaveImageAsync(byte[] data, string fileName)
    {
        var uniqueName = $"{Guid.NewGuid():N}_{fileName}";
        var filePath = Path.Combine(_basePath, uniqueName);
        await File.WriteAllBytesAsync(filePath, data);
        return uniqueName;
    }

    public async Task<byte[]?> GetImageAsync(string filePath)
    {
        var fullPath = Path.Combine(_basePath, filePath);
        if (!File.Exists(fullPath)) return null;
        return await File.ReadAllBytesAsync(fullPath);
    }

    public Task DeleteAsync(string filePath)
    {
        var fullPath = Path.Combine(_basePath, filePath);
        if (File.Exists(fullPath)) File.Delete(fullPath);
        return Task.CompletedTask;
    }
}
