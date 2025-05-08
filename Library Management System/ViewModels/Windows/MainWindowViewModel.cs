using System.Collections.ObjectModel;
using Wpf.Ui.Controls;

namespace Library_Management_System.ViewModels.Windows
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _applicationTitle = "Library_Management_System";

        [ObservableProperty]
        private ObservableCollection<object> _menuItems = new()
        {
            new NavigationViewItem()
            {
                Content = "Dashboard",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
                TargetPageType = typeof(Views.Pages.DashboardPage)
            },
            new NavigationViewItemSeparator(),
            new NavigationViewItem()
            {
                Content = "Browse Books",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Book24 },
                TargetPageType = typeof(Views.Pages.BrowseBooksPage)
            },
            new NavigationViewItem()
            {
                Content = "Manage Books",
                Icon = new SymbolIcon { Symbol = SymbolRegular.BookAdd24 },
                TargetPageType = typeof(Views.Pages.ManageBooksPage)
            },
            new NavigationViewItem()
            {
                Content = "Lend Books",
                Icon = new SymbolIcon { Symbol = SymbolRegular.BookContacts24 },
                TargetPageType = typeof(Views.Pages.LendBooksPage)
            },
            new NavigationViewItemSeparator(),
            new NavigationViewItem()
            {
                Content = "Integrations",
                Icon = new SymbolIcon { Symbol = SymbolRegular.PlugConnected24 },
                MenuItems =
                {
                    new NavigationViewItem()
                    {
                        Content = "Connect Bot",
                        Icon = new SymbolIcon { Symbol = SymbolRegular.Bot24 },
                        TargetPageType = typeof(Views.Pages.ConnectBotPage)
                    },
                    new NavigationViewItem()
                    {
                        Content = "Manage OpenAI Keys",
                        Icon = new SymbolIcon { Symbol = SymbolRegular.Key24 },
                        TargetPageType = typeof(Views.Pages.OpenAIKeysPage)
                    }
                }
            },
            new NavigationViewItem()
            {
                Content = "Data",
                Icon = new SymbolIcon { Symbol = SymbolRegular.DataHistogram24 },
                TargetPageType = typeof(Views.Pages.DataPage)
            }
        };

        [ObservableProperty]
        private ObservableCollection<object> _footerMenuItems = new()
        {
            new NavigationViewItem()
            {
                Content = "Settings",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
                TargetPageType = typeof(Views.Pages.SettingsPage)
            }
        };

        [ObservableProperty]
        private ObservableCollection<MenuItem> _trayMenuItems = new()
        {
            new MenuItem { Header = "Home", Tag = "tray_home" }
        };
    }
}