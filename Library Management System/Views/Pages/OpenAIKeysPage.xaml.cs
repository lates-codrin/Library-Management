using Library_Management_System.ViewModels.Pages;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;
using Wpf.Ui.Abstractions.Controls;
using LibVLCSharp.Shared;

namespace Library_Management_System.Views.Pages
{
    public partial class OpenAIKeysPage : INavigableView<OpenAIKeysViewModel>
    {
        public OpenAIKeysViewModel ViewModel { get; }

        private LibVLC _libVLC;
        private MediaPlayer _mediaPlayer;
        private Media _media;

        public OpenAIKeysPage(OpenAIKeysViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            Core.Initialize();
            InitializeComponent();

        }

        
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }

        private void CopyDefaultApiKey_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(DefaultApiKeyTextBox.Text);
        }
    }
}
