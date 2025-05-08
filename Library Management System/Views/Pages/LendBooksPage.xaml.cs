using Library_Management_System.ViewModels.Pages;
using Wpf.Ui.Abstractions.Controls;

namespace Library_Management_System.Views.Pages
{
    public partial class LendBooksPage : INavigableView<LendBooksViewModel>
    {
        public LendBooksViewModel ViewModel { get; }

        public LendBooksPage(LendBooksViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }
    }
}