using System.Threading.Tasks;
using TenantConfigTool.Models;

namespace TenantConfigTool.Services;

public interface IFileCopyService
{
    Task<(bool Success, string Message)> CopyFileAsync(
        FileMapping mapping,
        string targetProjectPath,
        string baseTenantCode,
        string baseTenantName,
        string newTenantCode,
        string newTenantName,
        bool overwriteExisting = false);
}
