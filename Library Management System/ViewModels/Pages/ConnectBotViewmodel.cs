using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Library_Management_System.BusinessLogic;
using System.Threading.Tasks;

namespace Library_Management_System.ViewModels.Pages
{
    public partial class ConnectBotViewModel : ObservableObject
    {
        private readonly RecommendationService _recommendationService;

        public ConnectBotViewModel()
        {
            _recommendationService = new RecommendationService();
        }

        [ObservableProperty]
        private string bookTitle;

        [ObservableProperty]
        private string genre;

        [ObservableProperty]
        private string synopsis;

        [ObservableProperty]
        private string recommendationResult;

        [ObservableProperty]
        private string formMessage;

        /// <summary>
        /// Sends book title, genre, and synopsis to the recommendation service to fetch AI suggestions.
        /// Validates inputs and updates the user side of things with status/errors.
        /// </summary>
        /// 
        [RelayCommand]
        private async Task GetRecommendationAsync()
        {
            if (string.IsNullOrWhiteSpace(BookTitle) || string.IsNullOrWhiteSpace(Synopsis))
            {
                FormMessage = "Please enter both title and synopsis!";
                return;
            }

            FormMessage = "Getting recommendations...";
            if (_recommendationService.IsApiKeyMissing)
            {
                FormMessage = "Oops! You have not set an API key in Integrations > Manage OpenAI Keys.";
            }
            else
            {
                try
                {
                    RecommendationResult = await _recommendationService.GetRecommendationsAsync(BookTitle, Genre, Synopsis);
                    FormMessage = "Done.";
                }
                catch (Exception exx)
                {
                    FormMessage = $"Oops!. Failed retrieving with the following message: {exx}";
                }
            }
        }
    }
}