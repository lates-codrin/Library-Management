using Library_Management_System.ViewModels.Pages;
using Wpf.Ui.Abstractions.Controls;

namespace Library_Management_System.Views.Pages
{
    public partial class DataPage : INavigableView<DataViewModel>
    {
        public DataViewModel ViewModel { get; }

        public DataPage(DataViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }
    }
}