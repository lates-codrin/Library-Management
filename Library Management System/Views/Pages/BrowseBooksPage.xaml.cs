using Library_Management_System.ViewModels.Pages;
using System.Windows.Controls;
using Wpf.Ui.Abstractions.Controls;

namespace Library_Management_System.Views.Pages
{
    public partial class BrowseBooksPage : INavigableView<BrowseBooksViewModel>
    {
        public BrowseBooksViewModel ViewModel { get; }

        public BrowseBooksPage(BrowseBooksViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }
        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalOffset + e.ViewportHeight >= e.ExtentHeight)
            {
                if (DataContext is BrowseBooksViewModel vm && vm.LoadMoreBooksCommand.CanExecute(null))
                {
                    vm.LoadMoreBooksCommand.Execute(null);
                }
            }
        }
    }
}