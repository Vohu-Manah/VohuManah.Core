using Application.Abstractions.FileStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.FileStorage;

internal sealed class LocalFileStorageService : IFileStorageService
{
    private readonly string _storagePath;
    private readonly ILogger<LocalFileStorageService> _logger;
    private const long MaxFileSize = 10 * 1024 * 1024; // 10 MB
    private static readonly string[] AllowedImageTypes = { "IMAGE/JPEG", "IMAGE/PNG", "IMAGE/GIF", "IMAGE/WEBP" };

    public LocalFileStorageService(IConfiguration configuration, ILogger<LocalFileStorageService> logger)
    {
        _logger = logger;
        _storagePath = configuration["FileStorage:Path"] ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        
        // Create directory if it doesn't exist
        if (!Directory.Exists(_storagePath))
        {
            Directory.CreateDirectory(_storagePath);
            _logger.LogInformation("Created storage directory: {StoragePath}", _storagePath);
        }
    }

    public string GetStoragePath() => _storagePath;

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        // Validate file size
        if (fileStream.Length > MaxFileSize)
        {
            throw new InvalidOperationException($"File size exceeds maximum allowed size of {MaxFileSize / (1024 * 1024)} MB");
        }

        // Validate content type for images
        if (!AllowedImageTypes.Contains(contentType.ToUpperInvariant(), StringComparer.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"File type '{contentType}' is not allowed. Allowed types: {string.Join(", ", AllowedImageTypes)}");
        }

        // Generate unique file name
        string extension = Path.GetExtension(fileName);
        string uniqueFileName = $"{Guid.NewGuid()}{extension}";
        string filePath = Path.Combine(_storagePath, uniqueFileName);

        try
        {
            await using FileStream fileStreamWriter = new(filePath, FileMode.Create);
            await fileStream.CopyToAsync(fileStreamWriter, cancellationToken);
            
            _logger.LogInformation("File saved successfully: {FilePath}", filePath);
            return uniqueFileName; // Return relative path
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving file: {FilePath}. Original file name: {FileName}", filePath, fileName);
            throw new InvalidOperationException($"Failed to save file '{fileName}' to storage path '{filePath}'. See inner exception for details.", ex);
        }
    }

    public Task<bool> DeleteFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        try
        {
            string fullPath = Path.Combine(_storagePath, filePath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                _logger.LogInformation("File deleted successfully: {FilePath}", fullPath);
                return Task.FromResult(true);
            }
            
            _logger.LogWarning("File not found for deletion: {FilePath}", fullPath);
            return Task.FromResult(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file: {FilePath}", filePath);
            return Task.FromResult(false);
        }
    }

    public Task<Stream?> GetFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        try
        {
            string fullPath = Path.Combine(_storagePath, filePath);
            if (File.Exists(fullPath))
            {
                return Task.FromResult<Stream?>(new FileStream(fullPath, FileMode.Open, FileAccess.Read));
            }
            
            _logger.LogWarning("File not found: {FilePath}", fullPath);
            return Task.FromResult<Stream?>(null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading file: {FilePath}", filePath);
            return Task.FromResult<Stream?>(null);
        }
    }

    public string GetFileUrl(string filePath)
    {
        // Return relative URL that can be served by static files middleware
        return $"/uploads/{filePath}";
    }
}


