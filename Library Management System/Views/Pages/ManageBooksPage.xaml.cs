using Library_Management_System.Models;
using Library_Management_System.ViewModels.Pages;
using Wpf.Ui.Abstractions.Controls;

namespace Library_Management_System.Views.Pages
{
    public partial class ManageBooksPage : INavigableView<ManageBooksViewModel>
    {
        public ManageBooksViewModel ViewModel { get; }

        public ManageBooksPage(ManageBooksViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }
    }
}