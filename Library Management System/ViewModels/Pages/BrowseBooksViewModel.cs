using Library_Management_System.Models;
using System.Collections.ObjectModel;

namespace Library_Management_System.ViewModels.Pages
{
    /// <summary>
    /// The ViewModel for browsing and searching books in the library.
    /// Handles book pagination, search functionality, and the display of books.
    /// </summary>
    public partial class BrowseBooksViewModel : ObservableObject
    {
        private readonly LibraryManager _libraryManager;
        private readonly LendManager _lendManager;
        private int _pageNumber = 1;
        private const int PageSize = 12;
        private int _totalBooksCount;

        /// <summary>
        /// Gets the collection of books to be displayed in the view.
        /// </summary>
        public ObservableCollection<Book> Books { get; }

        private string _searchQuery = string.Empty;

        /// <summary>
        /// Gets or sets the search query used to filter books.
        /// Reloads the books whenever the search query changes.
        /// </summary>
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (SetProperty(ref _searchQuery, value))
                {
                    ReloadBooks();
                }
            }
        }

        /// <summary>
        /// Indicates if the user can navigate to the previous page.
        /// </summary>
        public bool CanGoPrevious => _pageNumber > 1;

        /// <summary>
        /// Indicates iff the user can navigate to the next page.
        /// </summary>
        public bool CanGoNext => _pageNumber * PageSize < _totalBooksCount;

        /// <summary>
        /// Command for navigating to the previous page of books.
        /// </summary>
        public IRelayCommand PreviousPageCommand { get; }

        /// <summary>
        /// Command for navigating to the next page of books.
        /// </summary>
        public IRelayCommand NextPageCommand { get; }

        [ObservableProperty]
        private bool isLoading;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowseBooksViewModel"/> class.
        /// </summary>
        /// <param name="libraryManager">The instance of <see cref="LibraryManager"/> for managing books.</param>
        /// <param name="lendManager">The instance of <see cref="LendManager"/> for managing lending.</param>
        public BrowseBooksViewModel(LibraryManager libraryManager, LendManager lendManager)
        {
            _libraryManager = libraryManager;
            _lendManager = lendManager;

            Books = new ObservableCollection<Book>();

            PreviousPageCommand = new RelayCommand(OnPreviousPage, () => CanGoPrevious);
            NextPageCommand = new RelayCommand(OnNextPage, () => CanGoNext);

            _lendManager.BooksStatusChanged += () => ReloadBooks();
            _libraryManager.BooksStatusChanged += () => ReloadBooks();

            LoadBooksAsync();
        }

        /// <summary>
        /// Reloads the books based on the current search query and resets the pagination to the first page.
        /// </summary>
        [RelayCommand]
        public void ReloadBooks()
        {
            _pageNumber = 1;
            Books.Clear();
            LoadBooksAsync();

            PreviousPageCommand.NotifyCanExecuteChanged();
            NextPageCommand.NotifyCanExecuteChanged();
            OnPropertyChanged(nameof(CanGoPrevious));
            OnPropertyChanged(nameof(CanGoNext));
        }

        /// <summary>
        /// Loads books using the library manager.
        /// </summary>
        private async Task LoadBooksAsync()
        {
            if (IsLoading) return;
            IsLoading = true;

            try
            {
                var books = await _libraryManager.GetBooksBySearchAsync(SearchQuery, _pageNumber, PageSize);
                _totalBooksCount = await _libraryManager.GetBooksCountBySearchAsync(SearchQuery);

                Books.Clear();
                foreach (var book in books)
                {
                    if (!Books.Any(b => b.Id == book.Id))
                        Books.Add(book);
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void OnPreviousPage()
        {
            if (!CanGoPrevious) return;

            _pageNumber--;
            Books.Clear();
            LoadBooksAsync();

            PreviousPageCommand.NotifyCanExecuteChanged();
            NextPageCommand.NotifyCanExecuteChanged();
            OnPropertyChanged(nameof(CanGoPrevious));
            OnPropertyChanged(nameof(CanGoNext));
        }

        private void OnNextPage()
        {
            if (!CanGoNext) return;

            _pageNumber++;
            Books.Clear();
            LoadBooksAsync();

            PreviousPageCommand.NotifyCanExecuteChanged();
            NextPageCommand.NotifyCanExecuteChanged();
            OnPropertyChanged(nameof(CanGoPrevious));
            OnPropertyChanged(nameof(CanGoNext));
        }
    }
}
