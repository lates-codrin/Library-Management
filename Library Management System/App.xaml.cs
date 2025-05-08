using System.IO;
using System.Reflection;
using System.Windows.Threading;
using Library_Management_System.BusinessLogic;
using Library_Management_System.Data;
using Library_Management_System.Services;
using Library_Management_System.ViewModels.Pages;
using Library_Management_System.ViewModels.Windows;
using Library_Management_System.Views.Pages;
using Library_Management_System.Views.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wpf.Ui;
using Wpf.Ui.DependencyInjection;

namespace Library_Management_System
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
            .ConfigureAppConfiguration(c => { c.SetBasePath(Path.GetDirectoryName(AppContext.BaseDirectory)); })
            .ConfigureServices((context, services) =>
            {
                services.AddNavigationViewPageProvider();

                services.AddHostedService<ApplicationHostService>();

                services.AddSingleton<IThemeService, ThemeService>();

                services.AddSingleton<ITaskBarService, TaskBarService>();

                services.AddSingleton<INavigationService, NavigationService>();

                services.AddSingleton<IBookRepository>(new BookRepository("Database/books.json"));
                services.AddSingleton<ILendBookRepository>(new LendBookRepository("Database/lend_books.json"));
                services.AddSingleton<LibraryManager>();
                services.AddSingleton<LendManager>();
                services.AddSingleton<LendBooksViewModel>();

                services.AddSingleton<INavigationWindow, MainWindow>();
                services.AddSingleton<MainWindowViewModel>();

                services.AddSingleton<DashboardPage>();
                services.AddSingleton<DashboardViewModel>();
                services.AddSingleton<BrowseBooksPage>();
                services.AddSingleton<BrowseBooksViewModel>();
                services.AddSingleton<ManageBooksPage>();
                services.AddSingleton<ManageBooksViewModel>();
                services.AddSingleton<LendBooksPage>();
                services.AddSingleton<LendBooksViewModel>();
                services.AddSingleton<ConnectBotPage>();
                services.AddSingleton<ConnectBotViewModel>();
                services.AddSingleton<OpenAIKeysPage>();
                services.AddSingleton<OpenAIKeysViewModel>();
                services.AddSingleton<DataPage>();
                services.AddSingleton<DataViewModel>();
                services.AddSingleton<SettingsPage>();
                services.AddSingleton<SettingsViewModel>();
            }).Build();

        /// <summary>
        /// Gets services.
        /// </summary>
        public static IServiceProvider Services
        {
            get { return _host.Services; }
        }

        /// <summary>
        /// Occurs when the application is loading.
        /// </summary>
        private async void OnStartup(object sender, StartupEventArgs e)
        {
            await _host.StartAsync();
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