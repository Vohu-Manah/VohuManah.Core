using System.Diagnostics.CodeAnalysis;

namespace Application.Abstractions.FileStorage;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default);
    Task<bool> DeleteFileAsync(string filePath, CancellationToken cancellationToken = default);
    Task<Stream?> GetFileAsync(string filePath, CancellationToken cancellationToken = default);
    
    [SuppressMessage("Design", "CA1055:URI return values should not be strings", Justification = "Relative URL paths are better represented as strings for frontend compatibility")]
    string GetFileUrl(string filePath);
    
    string GetStoragePath();
}

