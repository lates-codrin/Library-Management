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
        [ObservableProperty]
        private bool isBusy;

        [RelayCommand]
        private async Task GetRecommendationAsync()
        {
            if (isBusy) return;
            isBusy = true;

            try
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
                    RecommendationResult = await _recommendationService.GetRecommendationsAsync(BookTitle, Genre, Synopsis)
                                              ?? "No recommendations found.";
                    FormMessage = "Done.";
                }
            }
            catch (Exception ex)
            {
                FormMessage = $"Oops! Failed to retrieve recommendations. {ex.Message}";
            }
            finally
            {
                isBusy = false;
            }
        }
    }
}