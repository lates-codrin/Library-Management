using Library_Management_System.Properties;

using OpenAI.Chat;

/// <summary>
/// Service to provide book recommendations using OpenAI.
/// </summary>
namespace Library_Management_System.BusinessLogic
{
    public class RecommendationService
    {
        private readonly ChatClient? _api;

        /// <summary>
        /// Indicates whether the OpenAI API key is missing.
        /// </summary>
        public bool IsApiKeyMissing { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecommendationService"/> class.
        /// </summary>
        public RecommendationService()
        {
            string apiKey = Settings.Default.OpenAIKey ?? throw new ArgumentNullException("Key not found.");

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                IsApiKeyMissing = true;
                return;
            }

            _api = new(model: "gpt-3.5-turbo", apiKey: apiKey);
        }

        /// <summary>
        /// Gets book recommendations based on a given title, genre, and synopsis.
        /// </summary>
        /// <param name="title">The title of the book the user likes.</param>
        /// <param name="genre">The genre of the book.</param>
        /// <param name="synopsis">A short synopsis of the book.</param>
        /// <returns>A string with recommended books or null if unavailable.</returns>
        public async Task<string?> GetRecommendationsAsync(string title, string genre, string synopsis)
        {
            if (IsApiKeyMissing)
                return null;

            string prompt = $"I like the book titled \"{title}\". It falls under the genre {genre}. " +
                            $"Here is the synopsis: {synopsis}. Recommend 3 similar books.";

            ChatCompletion result = await _api!.CompleteChatAsync(prompt);

            return result.Content?.FirstOrDefault()?.Text?.Trim();
        }
    }
}