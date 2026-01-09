using TenantConfigTool.Models;

namespace TenantConfigTool.Services;

public interface IFileCopyService
{
    Task<(bool Success, string Message)> CopyFileAsync(
        FileMapping mapping,
        string targetProjectPath,
        string baseTenantCode,
        string newTenantCode,
        bool overwriteExisting = false);
}
