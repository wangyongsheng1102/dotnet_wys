using TenantConfigTool.Models;

namespace TenantConfigTool.Services;

public interface IFileScanService
{
    List<FileMapping> ScanFiles(string baseProjectPath, string baseTenantCode);
}
