using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Library_Management_System.BusinessLogic;
using Library_Management_System.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Library_Management_System.ViewModels.Pages
{
    public partial class BrowseBooksViewModel : ObservableObject
    {
        private readonly LibraryManager _libraryManager;
        private readonly LendManager _lendManager;
        private int _pageNumber = 1;
        private const int PageSize = 100;

        public BrowseBooksViewModel(LibraryManager libraryManager, LendManager lendManager)
        {
            _libraryManager = libraryManager;
            _lendManager = lendManager;

            Books = new ObservableCollection<Book>();
            SearchQuery = string.Empty;

            _lendManager.BooksStatusChanged += () => ReloadBooks();

            LoadBooksAsync();
        }
        /// <summary>
        /// Reloads the list of books by clearing the current collection and fetching the latest data from the library manager.
        /// Typically used when book statuses are updated.
        /// </summary>
        [RelayCommand]
        public void ReloadBooks()
        {
            Books.Clear();
            var updatedBooks = _libraryManager.GetBooks();
            foreach (var book in updatedBooks)
            {
                Books.Add(book);
            }
        }
        /// <summary>
        /// Adds a new book to the library using the library manager and updates the local book collection.
        /// </summary>
        public void AddBook(Book newBook)
        {
            _libraryManager.AddBook(newBook);
            Books.Add(newBook);
        }

        public ObservableCollection<Book> Books { get; }

        [ObservableProperty]
        private string searchQuery;

        /// <summary>
        /// Initiates a search using the current search query, resets pagination, clears the book list, and loads matching results.
        /// </summary>
        /// 
        [RelayCommand]
        private async Task Search()
        {
            _pageNumber = 1;
            Books.Clear();
            await LoadBooksAsync();
        }

        private async Task LoadBooksAsync()
        {
            var books = await _libraryManager.GetBooksBySearchAsync(SearchQuery, _pageNumber, PageSize);
            foreach (var book in books)
            {
                Books.Add(book);
            }

            _pageNumber++;
        }

        /// <summary>
        /// Loads additional books for the next page based on the current search query and appends them to the collection.
        /// </summary>
        /// 
        [RelayCommand]
        private async Task LoadMoreBooks()
        {
            await LoadBooksAsync();
        }
    }
}
