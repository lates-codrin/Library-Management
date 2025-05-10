using Library_Management_System.Models;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;

namespace Library_Management_System.ViewModels.Pages
{
    /// <summary>
    /// ViewModel for managing books in the library system. 
    /// Provides functionality to do CRUD operations on book records.
    /// Handles pagination and image import for books.
    /// </summary>
    public partial class ManageBooksViewModel : ObservableObject
    {
        private readonly LibraryManager _libraryManager;
        private readonly LendManager _lendManager;
        private int _pageNumber = 1;
        private const int PageSize = 12;
        private int _totalBooksCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManageBooksViewModel"/> class.
        /// </summary>
        /// <param name="libraryManager">The <see cref="LibraryManager"/> instance for managing book operations.</param>
        /// <param name="lendManager">The <see cref="LendManager"/> instance for managing lending operations.</param>
        public ManageBooksViewModel(LibraryManager libraryManager, LendManager lendManager)
        {
            _libraryManager = libraryManager;
            _lendManager = lendManager;
            Books = new ObservableCollection<Book>();

            _lendManager.BooksStatusChanged += () => ReloadBooks();
            _libraryManager.BooksStatusChanged += () => ReloadBooks();

            PreviousPageCommand = new RelayCommand(OnPreviousPage, () => CanGoPrevious);
            NextPageCommand = new RelayCommand(OnNextPage, () => CanGoNext);

            LoadBooksAsync();
            ClearForm();
        }

        /// <summary>
        /// Gets the collection of books currently managed by the system.
        /// </summary>
        public ObservableCollection<Book> Books { get; }

        [ObservableProperty] private Book selectedBook;
        [ObservableProperty] private string formTitle;
        [ObservableProperty] private string formAuthor;
        [ObservableProperty] private int formQuantity = 1;
        [ObservableProperty] private BookStatus formStatus = BookStatus.Available;
        [ObservableProperty] private DateTime formPublished = DateTime.Today;
        [ObservableProperty] private FileInfo? formImagePath;
        [ObservableProperty] private bool isLoading;

        /// <summary>
        /// Gets the available status options for the books.
        /// </summary>
        public ObservableCollection<BookStatus> StatusOptions { get; } = new()
        {
            BookStatus.Available,
            BookStatus.Issued,
            BookStatus.Reserved
        };

        /// <summary>
        /// Command to go to the previous page of books in the pagination system.
        /// </summary>
        public IRelayCommand PreviousPageCommand { get; }

        /// <summary>
        /// Command to go to the next page of books in the pagination system.
        /// </summary>
        public IRelayCommand NextPageCommand { get; }

        /// <summary>
        /// Determines if the user can go to the previous page.
        /// </summary>
        public bool CanGoPrevious => _pageNumber > 1;

        /// <summary>
        /// Determines if the user can go to the next page.
        /// </summary>
        public bool CanGoNext => _pageNumber * PageSize < _totalBooksCount;

        /// <summary>
        /// Handler for changes in the quantity of a book in the form.
        /// </summary>
        partial void OnFormQuantityChanged(int value)
        {
            if (value <= 0)
                FormStatus = BookStatus.Issued;
        }

        /// <summary>
        /// Command to add a new book to the library system.
        /// Validates and has anti duplicates protections.
        /// </summary>
        [RelayCommand]
        private void Add()
        {
            if (string.IsNullOrWhiteSpace(FormTitle) || string.IsNullOrWhiteSpace(FormAuthor) || FormQuantity < 0)
                return;

            bool alreadyExists = _libraryManager.GetBooks().Any(b =>
                b.Title.Equals(FormTitle, StringComparison.OrdinalIgnoreCase) &&
                b.Author.Equals(FormAuthor, StringComparison.OrdinalIgnoreCase));

            if (alreadyExists)
                return;

            var book = new Book
            {
                Id = Guid.NewGuid(),
                Title = FormTitle,
                Author = FormAuthor,
                Quantity = FormQuantity,
                Published = FormPublished,
                Status = FormStatus,
                ImagePath = FormImagePath
            };

            _libraryManager.AddBook(book);
            ReloadBooks();
            ClearForm();
        }

        /// <summary>
        /// Command to update an existing book's details in the library system.
        /// Ensures anti duplication and no negative quantity.
        /// </summary>
        [RelayCommand]
        private void Update()
        {
            if (SelectedBook == null || FormQuantity < 0)
                return;

            bool conflictExists = _libraryManager.GetBooks().Any(b =>
                b.Id != SelectedBook.Id &&
                b.Title.Equals(FormTitle, StringComparison.OrdinalIgnoreCase) &&
                b.Author.Equals(FormAuthor, StringComparison.OrdinalIgnoreCase));

            if (conflictExists)
                return;

            var currentlyLent = _lendManager.GetLendBooks().Count(l =>
                l.BookTitle.Equals(SelectedBook.Title, StringComparison.OrdinalIgnoreCase) &&
                l.Author.Equals(SelectedBook.Author, StringComparison.OrdinalIgnoreCase) &&
                l.Status != BookStatus.Returned);

            if (FormQuantity < currentlyLent)
                return;

            SelectedBook.Title = FormTitle;
            SelectedBook.Author = FormAuthor;
            SelectedBook.Quantity = FormQuantity;
            SelectedBook.Published = FormPublished;
            SelectedBook.Status = FormStatus;
            SelectedBook.ImagePath = FormImagePath;

            _libraryManager.UpdateBook(SelectedBook);

            ReloadBooks();
        }

        /// <summary>
        /// Command to delete the selected book from the library system.
        /// </summary>
        [RelayCommand]
        private void Delete()
        {
            if (SelectedBook == null) return;

            _libraryManager.DeleteBook(SelectedBook);
            ReloadBooks();
            ClearForm();
        }

        /// <summary>
        /// Command to clear the form inputs and reset the selected book.
        /// </summary>
        [RelayCommand]
        private void ClearForm()
        {
            SelectedBook = null;
            FormTitle = string.Empty;
            FormAuthor = string.Empty;
            FormQuantity = 1;
            FormStatus = BookStatus.Available;
            FormPublished = DateTime.Today;
            FormImagePath = null;
        }

        /// <summary>
        /// Command to import an image for the book from the file system.
        /// </summary>
        [RelayCommand]
        private void ImportImage()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Image Files (*.png;*.jpg)|*.png;*.jpg"
            };

            if (dialog.ShowDialog() == true)
            {
                FormImagePath = new FileInfo(dialog.FileName);
            }
        }

        /// <summary>
        /// Command to search for books based on a search query.
        /// Resets the page number to 1 and reloads the books.
        /// </summary>
        /// <param name="query">The search query.</param>
        [RelayCommand]
        private void Search(string query)
        {
            _pageNumber = 1;
            LoadBooksAsync(query);
        }

        /// <summary>
        /// Handler for changes in the selected book.
        /// Populates the form fields with the details of the selected book.
        /// </summary>
        partial void OnSelectedBookChanged(Book value)
        {
            if (value != null)
            {
                FormTitle = value.Title;
                FormAuthor = value.Author;
                FormQuantity = value.Quantity;
                FormPublished = value.Published;
                FormStatus = value.Status;
                FormImagePath = value.ImagePath;
            }
        }

        /// <summary>
        /// Navigates to the previous page of books if possible.
        /// </summary>
        private void OnPreviousPage()
        {
            if (!CanGoPrevious) return;

            _pageNumber--;
            LoadBooksAsync();
            NotifyPaginationChanged();
        }

        /// <summary>
        /// Navigates to the next page of books if possible.
        /// </summary>
        private void OnNextPage()
        {
            if (!CanGoNext) return;

            _pageNumber++;
            LoadBooksAsync();
            NotifyPaginationChanged();
        }

        /// <summary>
        /// Notifies the pagination commands that the current state has changed,
        /// updating their ability to execute.
        /// </summary>
        private void NotifyPaginationChanged()
        {
            PreviousPageCommand.NotifyCanExecuteChanged();
            NextPageCommand.NotifyCanExecuteChanged();
            OnPropertyChanged(nameof(CanGoPrevious));
            OnPropertyChanged(nameof(CanGoNext));
        }

        /// <summary>
        /// Reloads the books based on the current search query and page.
        /// </summary>
        private void ReloadBooks() => LoadBooksAsync();

        /// <summary>
        /// Loads books from the library, with optional search query and pagination.
        /// </summary>
        /// <param name="query">Optional search query to filter books by title or author.</param>
        private async void LoadBooksAsync(string? query = null)
        {
            if (IsLoading) return;
            IsLoading = true;

            try
            {
                var books = string.IsNullOrWhiteSpace(query)
                    ? await _libraryManager.GetBooksBySearchAsync("", _pageNumber, PageSize)
                    : await _libraryManager.GetBooksBySearchAsync(query, _pageNumber, PageSize);

                _totalBooksCount = await _libraryManager.GetBooksCountBySearchAsync(query ?? "");

                Books.Clear();
                foreach (var book in books)
                    Books.Add(book);
            }
            finally
            {
                IsLoading = false;
                NotifyPaginationChanged();
            }
        }
    }
}
