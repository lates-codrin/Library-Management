using Library_Management_System.ViewModels.Pages;
using Wpf.Ui.Abstractions.Controls;

namespace Library_Management_System.Views.Pages
{
    public partial class ConnectBotPage : INavigableView<ConnectBotViewModel>
    {
        public ConnectBotViewModel ViewModel { get; }

        public ConnectBotPage(ConnectBotViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }
    }
}