using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TenantConfigTool.Models;
using TenantConfigTool.Services;

namespace TenantConfigTool.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IFileScanService _fileScanService;
    private readonly IFileCopyService _fileCopyService;
    private Window? _parentWindow;

    [ObservableProperty]
    private string _baseProjectPath = string.Empty;

    [ObservableProperty]
    private string _targetProjectPath = string.Empty;

    [ObservableProperty]
    private string _baseTenantCode = string.Empty;

    [ObservableProperty]
    private string _newTenantCode = string.Empty;

    [ObservableProperty]
    private ObservableCollection<FileMapping> _fileMappings = new();

    [ObservableProperty]
    private ObservableCollection<string> _logMessages = new();

    [ObservableProperty]
    private bool _isProcessing = false;

    [ObservableProperty]
    private bool _overwriteExisting = false;

    [ObservableProperty]
    private FileMapping? _selectedMapping;

    public string StatusText => IsProcessing ? "处理中..." : "就绪";

    public MainViewModel(IFileScanService fileScanService, IFileCopyService fileCopyService)
    {
        _fileScanService = fileScanService;
        _fileCopyService = fileCopyService;
    }

    public void SetParentWindow(Window window)
    {
        _parentWindow = window;
    }

    [RelayCommand]
    private async Task BrowseBaseProjectPathAsync()
    {
        if (_parentWindow == null) return;

        var folderPicker = new FolderPickerOpenOptions
        {
            Title = "选择基础项目路径"
        };

        var result = await _parentWindow.StorageProvider.OpenFolderPickerAsync(folderPicker);
        
        if (result.Count > 0)
        {
            var folder = result[0];
            BaseProjectPath = folder.Path.LocalPath;
            AddLog($"已选择基础项目路径: {BaseProjectPath}");
        }
    }

    [RelayCommand]
    private async Task BrowseTargetProjectPathAsync()
    {
        if (_parentWindow == null) return;

        var folderPicker = new FolderPickerOpenOptions
        {
            Title = "选择目标项目路径"
        };

        var result = await _parentWindow.StorageProvider.OpenFolderPickerAsync(folderPicker);
        
        if (result.Count > 0)
        {
            var folder = result[0];
            TargetProjectPath = folder.Path.LocalPath;
            AddLog($"已选择目标项目路径: {TargetProjectPath}");
        }
    }

    [RelayCommand]
    private void Preview()
    {
        if (!ValidateInputs())
        {
            return;
        }

        try
        {
            IsProcessing = true;
            OnPropertyChanged(nameof(StatusText));
            AddLog("开始扫描文件...");

            var mappings = _fileScanService.ScanFiles(BaseProjectPath, BaseTenantCode);

            FileMappings.Clear();
            foreach (var mapping in mappings)
            {
                // Build target path for preview
                var targetRelativePath = mapping.RelativePath.Replace(
                    $"-{BaseTenantCode}",
                    $"-{NewTenantCode}",
                    StringComparison.OrdinalIgnoreCase);
                mapping.TargetPath = Path.Combine(TargetProjectPath, targetRelativePath);

                FileMappings.Add(mapping);
            }

            AddLog($"找到 {mappings.Count} 个文件");
        }
        catch (Exception ex)
        {
            AddLog($"预览失败: {ex.Message}");
        }
        finally
        {
            IsProcessing = false;
            OnPropertyChanged(nameof(StatusText));
        }
    }

    [RelayCommand]
    private async Task GenerateAsync()
    {
        if (!ValidateInputs())
        {
            return;
        }

        if (FileMappings.Count == 0)
        {
            AddLog("请先执行预览");
            return;
        }

        try
        {
            IsProcessing = true;
            OnPropertyChanged(nameof(StatusText));
            AddLog("开始生成文件...");

            int successCount = 0;
            int failCount = 0;

            foreach (var mapping in FileMappings)
            {
                var result = await _fileCopyService.CopyFileAsync(
                    mapping,
                    TargetProjectPath,
                    BaseTenantCode,
                    NewTenantCode,
                    OverwriteExisting);

                if (result.Success)
                {
                    successCount++;
                    AddLog($"[成功] {result.Message}");
                }
                else
                {
                    failCount++;
                    AddLog($"[失败] {result.Message}");
                }
            }

            AddLog($"生成完成: 成功 {successCount} 个, 失败 {failCount} 个");
        }
        catch (Exception ex)
        {
            AddLog($"生成过程出错: {ex.Message}");
        }
        finally
        {
            IsProcessing = false;
            OnPropertyChanged(nameof(StatusText));
        }
    }

    private bool ValidateInputs()
    {
        if (string.IsNullOrWhiteSpace(BaseProjectPath))
        {
            AddLog("错误: 基础项目路径不能为空");
            return false;
        }

        if (!Directory.Exists(BaseProjectPath))
        {
            AddLog("错误: 基础项目路径不存在");
            return false;
        }

        if (string.IsNullOrWhiteSpace(TargetProjectPath))
        {
            AddLog("错误: 目标项目路径不能为空");
            return false;
        }

        if (string.IsNullOrWhiteSpace(BaseTenantCode))
        {
            AddLog("错误: 基础租户代码不能为空");
            return false;
        }

        if (string.IsNullOrWhiteSpace(NewTenantCode))
        {
            AddLog("错误: 新租户代码不能为空");
            return false;
        }

        if (BaseTenantCode.Equals(NewTenantCode, StringComparison.OrdinalIgnoreCase))
        {
            AddLog("错误: 基础租户代码和新租户代码不能相同");
            return false;
        }

        return true;
    }

    private void AddLog(string message)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss");
        LogMessages.Add($"[{timestamp}] {message}");
    }
}
