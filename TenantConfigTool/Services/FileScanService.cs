using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TenantConfigTool.Models;

namespace TenantConfigTool.Services;

public class FileScanService : IFileScanService
{
    private static readonly string[] AllowedExtensions = [".properties", ".yml", ".yaml", ".xml"];

    public List<FileMapping> ScanFiles(string baseProjectPath, string baseTenantCode)
    {
        var mappings = new List<FileMapping>();

        if (string.IsNullOrWhiteSpace(baseProjectPath) || !Directory.Exists(baseProjectPath) || string.IsNullOrWhiteSpace(baseTenantCode))
        {
            return mappings;
        }

        var searchPattern = $"*-{baseTenantCode}.*";
        var allFiles = Directory.GetFiles(baseProjectPath, searchPattern, SearchOption.AllDirectories);

        mappings.AddRange(from filePath in allFiles let extension = Path.GetExtension(filePath) where AllowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase) let fileName = Path.GetFileName(filePath) where fileName.Contains($"-{baseTenantCode}", StringComparison.OrdinalIgnoreCase) let relativePath = Path.GetRelativePath(baseProjectPath, filePath) select new FileMapping { SourcePath = filePath, RelativePath = relativePath });

        return mappings;
    }
}
