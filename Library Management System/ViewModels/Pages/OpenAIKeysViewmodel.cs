using Library_Management_System.Services;

namespace Library_Management_System.ViewModels.Pages
{
    public partial class OpenAIKeysViewModel : ObservableObject
    {
        [ObservableProperty]
        private string openAIKey;

        public OpenAIKeysViewModel()
        {
            OpenAIKey = SettingsManager.LoadOpenAIKey();
        }

        [RelayCommand]
        private void Save()
        {
            SettingsManager.SaveOpenAIKey(OpenAIKey);
        }
        [RelayCommand]
        private void Delete()
        {
            SettingsManager.DeleteOpenAIKey();
        }
    }
}