using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Appearance;

namespace Library_Management_System.ViewModels.Pages
{
    public partial class SettingsViewModel : ObservableObject, INavigationAware
    {
        [ObservableProperty]
        private string _appVersion = string.Empty;

        [ObservableProperty]
        private ApplicationTheme _currentTheme = ApplicationTheme.Unknown;

        private static readonly Lazy<Task> _initializationTask = new(() => Task.Run(InitializeViewModel));

        private static SettingsViewModel? _currentInstance;

        public Task OnNavigatedToAsync()
        {
            _currentInstance = this;
            return _initializationTask.Value;
        }

        public Task OnNavigatedFromAsync() => Task.CompletedTask;

        private static void InitializeViewModel()
        {
            if (_currentInstance is null) return;

            _currentInstance.CurrentTheme = ApplicationThemeManager.GetAppTheme();
            _currentInstance.AppVersion = $"UiDesktopApp1 - {_currentInstance.GetAssemblyVersion()}";
        }

        private string GetAssemblyVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString()
                ?? string.Empty;
        }

        [RelayCommand]
        private void OnChangeTheme(string parameter)
        {
            ApplicationTheme selectedTheme = parameter switch
            {
                "theme_light" => ApplicationTheme.Light,
                _ => ApplicationTheme.Dark,
            };

            if (CurrentTheme == selectedTheme)
                return;

            ApplicationThemeManager.Apply(selectedTheme);
            CurrentTheme = selectedTheme;
        }
    }
}
