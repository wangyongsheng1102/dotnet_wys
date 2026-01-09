using System.Text;
using System.Text.RegularExpressions;

namespace TenantConfigTool.Services;

public class TenantReplaceService : ITenantReplaceService
{
    public string ReplaceTenantCode(string content, string baseTenantCode, string newTenantCode)
    {
        if (string.IsNullOrEmpty(content) || string.IsNullOrEmpty(baseTenantCode) || string.IsNullOrEmpty(newTenantCode))
        {
            return content;
        }

        var result = content;

        // Replace exact matches with case preservation
        // aaa -> bbb
        result = result.Replace(baseTenantCode, newTenantCode, StringComparison.Ordinal);
        
        // AAA -> BBB
        result = result.Replace(baseTenantCode.ToUpperInvariant(), newTenantCode.ToUpperInvariant(), StringComparison.Ordinal);
        
        // Aaa -> Bbb (PascalCase)
        if (baseTenantCode.Length > 0 && newTenantCode.Length > 0)
        {
            var basePascal = char.ToUpperInvariant(baseTenantCode[0]) + baseTenantCode.Substring(1).ToLowerInvariant();
            var newPascal = char.ToUpperInvariant(newTenantCode[0]) + newTenantCode.Substring(1).ToLowerInvariant();
            result = result.Replace(basePascal, newPascal, StringComparison.Ordinal);
        }

        // Also handle case-insensitive replacements for any other variations
        var pattern = $@"\b{Regex.Escape(baseTenantCode)}\b";
        var regex = new Regex(pattern, RegexOptions.IgnoreCase);

        result = regex.Replace(result, match =>
        {
            var matchedValue = match.Value;
            
            // If already replaced by exact matches above, skip
            if (matchedValue == newTenantCode || 
                matchedValue == newTenantCode.ToUpperInvariant() ||
                (newTenantCode.Length > 0 && matchedValue == char.ToUpperInvariant(newTenantCode[0]) + newTenantCode.Substring(1).ToLowerInvariant()))
            {
                return matchedValue;
            }

            // Preserve case pattern
            if (matchedValue == baseTenantCode)
            {
                return newTenantCode;
            }
            else if (matchedValue == baseTenantCode.ToUpperInvariant())
            {
                return newTenantCode.ToUpperInvariant();
            }
            else if (matchedValue == baseTenantCode.ToLowerInvariant())
            {
                return newTenantCode.ToLowerInvariant();
            }
            else if (baseTenantCode.Length > 0 && newTenantCode.Length > 0 && 
                     char.IsUpper(matchedValue[0]) && 
                     matchedValue.Substring(1).Equals(baseTenantCode.Substring(1), StringComparison.OrdinalIgnoreCase))
            {
                // First letter uppercase (PascalCase)
                return char.ToUpperInvariant(newTenantCode[0]) + newTenantCode.Substring(1).ToLowerInvariant();
            }
            else
            {
                // Default: use lowercase
                return newTenantCode.ToLowerInvariant();
            }
        });

        return result;
    }
}
