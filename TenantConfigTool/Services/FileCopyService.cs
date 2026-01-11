using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TenantConfigTool.Models;

namespace TenantConfigTool.Services;

public class FileCopyService : IFileCopyService
{
    private readonly ITenantReplaceService _tenantReplaceService;

    public FileCopyService(ITenantReplaceService tenantReplaceService)
    {
        _tenantReplaceService = tenantReplaceService;
    }

    public async Task<(bool Success, string Message)> CopyFileAsync(
        FileMapping mapping,
        string targetProjectPath,
        string baseTenantCode,
        string newTenantCode,
        bool overwriteExisting = false)
    {
        try
        {
            // Build target path by replacing tenant code in relative path
            var targetRelativePath = mapping.RelativePath.Replace(
                $"-{baseTenantCode}",
                $"-{newTenantCode}",
                StringComparison.OrdinalIgnoreCase);

            var targetFilePath = Path.Combine(targetProjectPath, targetRelativePath);
            var targetDirectory = Path.GetDirectoryName(targetFilePath);

            if (string.IsNullOrEmpty(targetDirectory))
            {
                return (false, $"无法确定目标目录: {targetFilePath}");
            }

            // Create directory if not exists
            Directory.CreateDirectory(targetDirectory);

            // Check if file exists
            if (File.Exists(targetFilePath) && !overwriteExisting)
            {
                return (false, $"文件已存在，跳过: {targetFilePath}");
            }

            // Read source file
            string content;
            try
            {
                content = await File.ReadAllTextAsync(mapping.SourcePath, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                return (false, $"读取源文件失败: {ex.Message}");
            }

            // Replace tenant codes
            content = _tenantReplaceService.ReplaceTenantCode(content, baseTenantCode, newTenantCode);

            // Write target file
            try
            {
                await File.WriteAllTextAsync(targetFilePath, content, Encoding.UTF8);
                return (true, $"成功生成: {targetRelativePath}");
            }
            catch (Exception ex)
            {
                return (false, $"写入目标文件失败: {ex.Message}");
            }
        }
        catch (Exception ex)
        {
            return (false, $"处理文件时出错: {ex.Message}");
        }
    }
}
