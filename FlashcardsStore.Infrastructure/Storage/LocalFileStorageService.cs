using FlashcardsStore.Application.Interfaces.Services;

namespace FlashcardsStore.Infrastructure.Storage;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _webRootPath;

    public LocalFileStorageService(string webRootPath)
    {
        _webRootPath = webRootPath;
    }

    public async Task<string> SaveFileAsync(Stream stream, string fileName, string subdirectory, CancellationToken cancellationToken = default)
    {
        var targetDir = Path.Combine(_webRootPath, "uploads", subdirectory);
        if (!Directory.Exists(targetDir))
        {
            Directory.CreateDirectory(targetDir);
        }

        var ext = Path.GetExtension(fileName);
        var uniqueFileName = $"{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(targetDir, uniqueFileName);

        using (var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
        {
            await stream.CopyToAsync(fileStream, cancellationToken);
        }

        // Return relative path from web root for web serving
        return $"/uploads/{subdirectory}/{uniqueFileName}".Replace('\\', '/');
    }

    public Task DeleteFileAsync(string relativePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            return Task.CompletedTask;

        var cleanPath = relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.Combine(_webRootPath, cleanPath);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }

    public bool FileExists(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath)) return false;
        var cleanPath = relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.Combine(_webRootPath, cleanPath);
        return File.Exists(fullPath);
    }
}
