using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Library_Management_System.BusinessLogic;
using Library_Management_System.Models;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Library_Management_System.ViewModels.Pages
{
    public partial class ManageBooksViewModel : ObservableObject
    {
        private readonly LibraryManager _libraryManager;
        private readonly LendManager _lendManager;

        public ManageBooksViewModel(LibraryManager libraryManager, LendManager lendManager)
        {
            _libraryManager = libraryManager;
            Books = new ObservableCollection<Book>(_libraryManager.GetBooks());

            ClearForm();
            _lendManager = lendManager;
            _lendManager.BooksStatusChanged += RefreshBooks;

        }

        public ObservableCollection<Book> Books { get; }

        [ObservableProperty] private Book selectedBook;

        [ObservableProperty] private string formTitle;
        [ObservableProperty] private string formAuthor;
        [ObservableProperty] private int formQuantity = 1;
        [ObservableProperty] private string formStatus;
        [ObservableProperty] private DateTime formPublished = DateTime.Today;
        [ObservableProperty] private string formImagePath;

        partial void OnFormQuantityChanged(int value)
        {
            if (value < 0)
            {
                FormStatus = "Reserved";
            }
        }

        public ObservableCollection<string> StatusOptions { get; } = new()
        {
            "Available", "Issued", "Reserved"
        };

        [RelayCommand]
        private void Add()
        {
            if (string.IsNullOrWhiteSpace(FormTitle) || string.IsNullOrWhiteSpace(FormAuthor))
            {
                FormStatus = "Title and Author are required.";
                return;
            }

            if (FormQuantity < 0)
            {
                FormStatus = "Quantity cannot be negative.";
                return;
            }

            bool alreadyExists = Books.Any(b =>
                b.Title.Equals(FormTitle, StringComparison.OrdinalIgnoreCase) &&
                b.Author.Equals(FormAuthor, StringComparison.OrdinalIgnoreCase)
            );

            if (alreadyExists)
            {
                FormStatus = "This book already exists!";
                return;
            }

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
            Books.Add(book);

            ClearForm();
        }

        [RelayCommand]
        private void Update()
        {
            if (SelectedBook == null)
            {
                FormStatus = "No book selected.";
                return;
            }

            if (string.IsNullOrWhiteSpace(FormTitle) || string.IsNullOrWhiteSpace(FormAuthor))
            {
                FormStatus = "Title and Author are required.";
                return;
            }

            if (FormQuantity < 0)
            {
                FormStatus = "Quantity cannot be negative.";
                return;
            }

            // Check if another book already has this title/author
            bool conflictExists = Books.Any(b =>
                b.Id != SelectedBook.Id &&
                b.Title.Equals(FormTitle, StringComparison.OrdinalIgnoreCase) &&
                b.Author.Equals(FormAuthor, StringComparison.OrdinalIgnoreCase)
            );

            if (conflictExists)
            {
                FormStatus = "Another book with this title/author already exists.";
                return;
            }

            // Prevent setting quantity higher than available + lent copies
            var currentlyLent = _lendManager.GetLendBooks()
                .Count(l => l.BookTitle.Equals(SelectedBook.Title, StringComparison.OrdinalIgnoreCase) &&
                           l.Author.Equals(SelectedBook.Author, StringComparison.OrdinalIgnoreCase) &&
                           l.Status != "Returned");

            if (FormQuantity < currentlyLent)
            {
                FormStatus = $"Cannot set quantity below {currentlyLent} (currently lent copies).";
                return;
            }

            SelectedBook.Title = FormTitle;
            SelectedBook.Author = FormAuthor;
            SelectedBook.Quantity = FormQuantity;
            SelectedBook.Published = FormPublished;
            SelectedBook.Status = FormStatus;
            SelectedBook.ImagePath = FormImagePath;

            _libraryManager.UpdateBook(SelectedBook);
            FormStatus = "Book updated successfully.";
            RefreshBooks();
        }

        [RelayCommand]
        private void Delete()
        {
            if (SelectedBook == null)
                return;

            _libraryManager.DeleteBook(SelectedBook);
            Books.Remove(SelectedBook);
            ClearForm();
        }

        [RelayCommand]
        private void ClearForm()
        {
            SelectedBook = null;
            FormTitle = string.Empty;
            FormAuthor = string.Empty;
            FormQuantity = 1;
            FormStatus = string.Empty;
            FormPublished = DateTime.Today;
            FormImagePath = string.Empty;
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
                FormImagePath = dialog.FileName;
            }
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

        [RelayCommand]
        private void Search(string query)
        {
            Books.Clear();

            var results = string.IsNullOrWhiteSpace(query)
                ? _libraryManager.GetBooks()
                : _libraryManager.SearchBooks(query);

            foreach (var book in results)
                Books.Add(book);
        }

        private void RefreshBooks()
        {
            Books.Clear();
            foreach (var book in _libraryManager.GetBooks())
                Books.Add(book);
        }
    }
}