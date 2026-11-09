using System.IO;
using System.Threading;
using System;
using Application.Abstractions.FileStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.FileStorage;

internal sealed class LocalFileStorageService : IFileStorageService
{
    private readonly string _storagePath;
    private readonly ILogger<LocalFileStorageService> _logger;
    private const long MaxFileSize = 10 * 1024 * 1024;
    private static readonly string[] AllowedImageTypes = { "IMAGE/JPEG", "IMAGE/PNG", "IMAGE/GIF", "IMAGE/WEBP" };
    private static readonly string[] AllowedDocumentTypes = { "APPLICATION/PDF", "APPLICATION/VND.OPENXMLFORMATS-OFFICEDOCUMENT.WORDPROCESSINGML.DOCUMENT", "APPLICATION/MSWORD" };

    public LocalFileStorageService(IConfiguration configuration, ILogger<LocalFileStorageService> logger)
    {
        _logger = logger;
        _storagePath = configuration["FileStorage:Path"] ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

        if (!Directory.Exists(_storagePath))
        {
            Directory.CreateDirectory(_storagePath);
            _logger.LogInformation("محل ذخیره سازی: {StoragePath}", _storagePath);
        }
    }

    public string GetStoragePath() => _storagePath;

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string contentType, string? entityType = null, CancellationToken cancellationToken = default)
    {
        if (fileStream.Length > MaxFileSize)
        {
            throw new InvalidOperationException($"محدودیت پیوست فایل  {MaxFileSize / (1024 * 1024)} MB");
        }

        string contentTypeUpper = contentType.ToUpperInvariant();
        bool isValidType = false;

        if (entityType?.Equals("Book", StringComparison.OrdinalIgnoreCase) == true)
        {
            string fileExtension = Path.GetExtension(fileName).ToUpperInvariant();
            bool isImageExtension = fileExtension is ".JPG" or ".JPEG" or ".PNG" or ".GIF" or ".WEBP";
            bool isDocumentExtension = fileExtension is ".PDF" or ".DOC" or ".DOCX";

            if (isImageExtension)
            {
                isValidType = AllowedImageTypes.Contains(contentTypeUpper, StringComparer.OrdinalIgnoreCase);
                if (!isValidType)
                {
                    throw new InvalidOperationException($"برای عکس کتاب، فرمت '{contentType}' مجاز نیست. فرمت های مجاز: JPEG, PNG, GIF, WEBP");
                }
            }
            else if (isDocumentExtension)
            {
                isValidType = AllowedDocumentTypes.Contains(contentTypeUpper, StringComparer.OrdinalIgnoreCase) ||
                              contentTypeUpper.Contains("PDF", StringComparison.OrdinalIgnoreCase) ||
                              contentTypeUpper.Contains("WORD", StringComparison.OrdinalIgnoreCase) ||
                              contentTypeUpper.Contains("DOCUMENT", StringComparison.OrdinalIgnoreCase);
                if (!isValidType)
                {
                    throw new InvalidOperationException($"برای پیوست کتاب، فرمت '{contentType}' مجاز نیست. فرمت های مجاز: PDF, Word (DOC, DOCX)");
                }
            }
            else
            {
                throw new InvalidOperationException($"فرمت فایل '{fileExtension}' برای کتاب مجاز نیست. برای عکس: JPEG, PNG, GIF, WEBP | برای پیوست: PDF, DOC, DOCX");
            }
        }
        else
        {
            isValidType = AllowedImageTypes.Contains(contentTypeUpper, StringComparer.OrdinalIgnoreCase);
            if (!isValidType)
            {
                throw new InvalidOperationException($"این پسوند '{contentType}' مجاز نیست. پسوند های مجاز: {string.Join(", ", AllowedImageTypes)}");
            }
        }

        string extension = Path.GetExtension(fileName);
        string uniqueFileName = $"{Guid.NewGuid()}{extension}";

        
        
        
        string entityFolder = SanitizeEntityFolder(entityType);
        string targetFolder = string.IsNullOrEmpty(entityFolder)
            ? _storagePath
            : Path.Combine(_storagePath, entityFolder);

        if (!Directory.Exists(targetFolder))
        {
            Directory.CreateDirectory(targetFolder);
        }

        string filePath = Path.Combine(targetFolder, uniqueFileName);

        try
        {
            await using FileStream fileStreamWriter = new(filePath, FileMode.Create);
            await fileStream.CopyToAsync(fileStreamWriter, cancellationToken);

            _logger.LogInformation("فایل با موفقیت ذخیره شد: {FilePath}", filePath);
            return string.IsNullOrEmpty(entityFolder)
                ? uniqueFileName
                : $"{entityFolder}/{uniqueFileName}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در ذخیره فایل: {FilePath}. نام حقیقی فایل: {FileName}", filePath, fileName);
            throw new InvalidOperationException($"خطا در ذخیره فایل '{fileName}' در مسیر '{filePath}'. ارور اصلی سرور را بررسی نمائید.", ex);
        }
    }

    
    
    
    
    
    private static string SanitizeEntityFolder(string? entityType)
    {
        if (string.IsNullOrWhiteSpace(entityType))
        {
            return string.Empty;
        }

        char[] allowed = entityType
            .Where(c => char.IsLetterOrDigit(c) || c == '-' || c == '_')
            .ToArray();

        return allowed.Length == 0 ? string.Empty : new string(allowed);
    }

    public Task<bool> DeleteFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        try
        {
            string fullPath = Path.Combine(_storagePath, filePath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                _logger.LogInformation("فایل حذف شد: {FilePath}", fullPath);
                return Task.FromResult(true);
            }

            _logger.LogWarning("فایل مربوط برای حذف یافت نشد: {FilePath}", fullPath);
            return Task.FromResult(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در حذف فایل: {FilePath}", filePath);
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

            _logger.LogWarning("فایل یافت شد: {FilePath}", fullPath);
            return Task.FromResult<Stream?>(null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "نتوانسیتم فایل را بخوانیم: {FilePath}", filePath);
            return Task.FromResult<Stream?>(null);
        }
    }

    public string GetFileUrl(string filePath)
    {
        return $"/uploads/{filePath}";
    }
}


