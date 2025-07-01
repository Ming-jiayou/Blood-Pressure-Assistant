using System.IO;
using System.Reflection;
using System.Windows.Threading;
using BPA.Services;
using BPA.ViewModels.Pages;
using BPA.ViewModels.Windows;
using BPA.Views.Pages;
using BPA.Views.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wpf.Ui;
using Microsoft.EntityFrameworkCore;
using BPA.Data;
using Wpf.Ui.Abstractions;
using Wpf.Ui.DependencyInjection;

namespace BPA
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        // The.NET Generic Host provides dependency injection, configuration, logging, and other services.
        // https://docs.microsoft.com/dotnet/core/extensions/generic-host
        // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
        // https://docs.microsoft.com/dotnet/core/extensions/configuration
        // https://docs.microsoft.com/dotnet/core/extensions/logging
        private static readonly IHost _host = Host
            .CreateDefaultBuilder()
            .ConfigureAppConfiguration(c => { c.SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)); })
            .ConfigureServices((context, services) =>
            {
                // 添加数据库服务
               
                var projectRootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..");

                // 确保路径是规范化的，去除冗余的 ..
                projectRootPath = Path.GetFullPath(projectRootPath);

                // 构建数据库文件的完整路径
                // 它会是 D:\Learning\MyProject\Blood-Pressure-Assistant\BPA\Data\bpa.db
                var dbPath = Path.Combine(projectRootPath, "Data", "bpa.db");
                services.AddDbContext<BPADbContext>(options =>
                    options.UseSqlite($"Data Source={dbPath}"));

                services.AddHostedService<ApplicationHostService>();

                // Page resolver service
                services.AddNavigationViewPageProvider();

                // Theme manipulation
                services.AddSingleton<IThemeService, ThemeService>();

                // TaskBar manipulation
                services.AddSingleton<ITaskBarService, TaskBarService>();

                // Service containing navigation, same as INavigationWindow... but without window
                services.AddSingleton<INavigationService, NavigationService>();

                // Main window with navigation
                services.AddSingleton<INavigationWindow, MainWindow>();
                services.AddSingleton<MainWindowViewModel>();

                services.AddSingleton<DashboardPage>();
                services.AddSingleton<DashboardViewModel>();
                services.AddSingleton<DataPage>();
                services.AddSingleton<DataViewModel>();
                services.AddSingleton<SettingsPage>();
                services.AddSingleton<SettingsViewModel>();

                services.AddSingleton<BPAHomePage>();
                services.AddSingleton<BPAHomeViewModel>();
            }).Build();

        /// <summary>
        /// Gets registered service.
        /// </summary>
        /// <typeparam name="T">Type of the service to get.</typeparam>
        /// <returns>Instance of the service or <see langword="null"/>.</returns>
        public static T GetService<T>()
            where T : class
        {
            return _host.Services.GetService(typeof(T)) as T;
        }

        /// <summary>
        /// Occurs when the application is loading.
        /// </summary>
        private void OnStartup(object sender, StartupEventArgs e)
        {
            _host.Start();
        }

        /// <summary>
        /// Occurs when the application is closing.
        /// </summary>
        private async void OnExit(object sender, ExitEventArgs e)
        {
            await _host.StopAsync();

            _host.Dispose();
        }

        /// <summary>
        /// Occurs when an exception is thrown by an application but not handled.
        /// </summary>
        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // For more info see https://docs.microsoft.com/en-us/dotnet/api/system.windows.application.dispatcherunhandledexception?view=windowsdesktop-6.0
        }
    }
}
