namespace TenantConfigTool.Services;

public interface ITenantReplaceService
{
    string ReplaceTenantCode(string content, string baseTenantCode, string newTenantCode);
}
