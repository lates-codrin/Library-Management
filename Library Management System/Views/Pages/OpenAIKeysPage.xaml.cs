using Library_Management_System.ViewModels.Pages;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Navigation;
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

            _libVLC = new LibVLC();
            _mediaPlayer = new MediaPlayer(_libVLC);
            VideoView.MediaPlayer = _mediaPlayer;

            var videoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "tutorial.mp4");
            if (File.Exists(videoPath))
            {
                _media = new Media(_libVLC, new Uri(videoPath));
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (_media != null)
            {
                if (!_mediaPlayer.IsPlaying)
                {
                    _mediaPlayer.Play(_media);
                }
            }
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_mediaPlayer.IsPlaying)
                _mediaPlayer.Pause();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            _mediaPlayer.Stop();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}