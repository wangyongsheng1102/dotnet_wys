using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using TenantConfigTool.Services;
using TenantConfigTool.ViewModels;
using TenantConfigTool.Views;

namespace TenantConfigTool;

public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Setup DI
        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow(_serviceProvider.GetRequiredService<MainViewModel>());
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Services
        services.AddSingleton<IFileScanService, FileScanService>();
        services.AddSingleton<ITenantReplaceService, TenantReplaceService>();
        services.AddSingleton<IFileCopyService, FileCopyService>();

        // ViewModels
        services.AddTransient<MainViewModel>();
    }
}
