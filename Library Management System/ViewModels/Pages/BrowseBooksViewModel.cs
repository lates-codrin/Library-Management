using Library_Management_System.Models;
using System.Collections.ObjectModel;

namespace Library_Management_System.ViewModels.Pages
{
    public partial class BrowseBooksViewModel : ObservableObject
    {
        private readonly LibraryManager _libraryManager;
        private readonly LendManager _lendManager;
        private int _pageNumber = 1;
        private const int PageSize = 12;
        private int _totalBooksCount;

        public ObservableCollection<Book> Books { get; }

        private string _searchQuery = string.Empty;
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

        public bool CanGoPrevious => _pageNumber > 1;
        public bool CanGoNext => _pageNumber * PageSize < _totalBooksCount;

        public IRelayCommand PreviousPageCommand { get; }
        public IRelayCommand NextPageCommand { get; }

        [ObservableProperty]
        private bool isLoading;


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
                    if(!Books.Any(b => b.Id == book.Id))
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
