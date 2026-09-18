namespace FlashcardsStore.Application.Interfaces.Services;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(Stream stream, string fileName, string subdirectory, CancellationToken cancellationToken = default);
    Task DeleteFileAsync(string relativePath, CancellationToken cancellationToken = default);
    bool FileExists(string relativePath);
}
