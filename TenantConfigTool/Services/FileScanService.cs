using System.IO;
using TenantConfigTool.Models;

namespace TenantConfigTool.Services;

public class FileScanService : IFileScanService
{
    private static readonly string[] AllowedExtensions = { ".properties", ".yml", ".yaml", ".xml" };

    public List<FileMapping> ScanFiles(string baseProjectPath, string baseTenantCode)
    {
        var mappings = new List<FileMapping>();

        if (string.IsNullOrWhiteSpace(baseProjectPath) || !Directory.Exists(baseProjectPath))
        {
            return mappings;
        }

        if (string.IsNullOrWhiteSpace(baseTenantCode))
        {
            return mappings;
        }

        var searchPattern = $"*-{baseTenantCode}.*";
        var allFiles = Directory.GetFiles(baseProjectPath, searchPattern, SearchOption.AllDirectories);

        foreach (var filePath in allFiles)
        {
            var extension = Path.GetExtension(filePath);
            if (!AllowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }

            var fileName = Path.GetFileName(filePath);
            if (!fileName.Contains($"-{baseTenantCode}", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var relativePath = Path.GetRelativePath(baseProjectPath, filePath);
            mappings.Add(new FileMapping
            {
                SourcePath = filePath,
                RelativePath = relativePath
            });
        }

        return mappings;
    }
}
