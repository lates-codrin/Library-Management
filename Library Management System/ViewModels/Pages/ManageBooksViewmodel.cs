using Library_Management_System.Models;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;

namespace Library_Management_System.ViewModels.Pages
{
    public partial class ManageBooksViewModel : ObservableObject
    {
        private readonly LibraryManager _libraryManager;
        private readonly LendManager _lendManager;

        private int _pageNumber = 1;
        private const int PageSize = 12;
        private int _totalBooksCount;


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

        public ObservableCollection<Book> Books { get; }

        [ObservableProperty] private Book selectedBook;
        [ObservableProperty] private string formTitle;
        [ObservableProperty] private string formAuthor;
        [ObservableProperty] private int formQuantity = 1;
        [ObservableProperty] private BookStatus formStatus = BookStatus.Available;
        [ObservableProperty] private DateTime formPublished = DateTime.Today;
        [ObservableProperty] private FileInfo? formImagePath;
        [ObservableProperty] private bool isLoading;

        public ObservableCollection<BookStatus> StatusOptions { get; } = new()
        {
            BookStatus.Available,
            BookStatus.Issued,
            BookStatus.Reserved
        };

        public IRelayCommand PreviousPageCommand { get; }
        public IRelayCommand NextPageCommand { get; }

        public bool CanGoPrevious => _pageNumber > 1;
        public bool CanGoNext => _pageNumber * PageSize < _totalBooksCount;

        partial void OnFormQuantityChanged(int value)
        {
            if (value <= 0)
                FormStatus = BookStatus.Issued;
        }

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

        [RelayCommand]
        private void Delete()
        {
            if (SelectedBook == null) return;

            _libraryManager.DeleteBook(SelectedBook);
            ReloadBooks();
            ClearForm();
        }

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

        [RelayCommand]
        private void Search(string query)
        {
            _pageNumber = 1;
            LoadBooksAsync(query);
        }

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

        private void OnPreviousPage()
        {
            if (!CanGoPrevious) return;

            _pageNumber--;
            LoadBooksAsync();
            NotifyPaginationChanged();
        }

        private void OnNextPage()
        {
            if (!CanGoNext) return;

            _pageNumber++;
            LoadBooksAsync();
            NotifyPaginationChanged();
        }

        private void NotifyPaginationChanged()
        {
            PreviousPageCommand.NotifyCanExecuteChanged();
            NextPageCommand.NotifyCanExecuteChanged();
            OnPropertyChanged(nameof(CanGoPrevious));
            OnPropertyChanged(nameof(CanGoNext));
        }

        private void ReloadBooks() => LoadBooksAsync();

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
