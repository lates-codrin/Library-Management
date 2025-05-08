using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Library_Management_System.BusinessLogic;
using Library_Management_System.Models;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using Wpf.Ui.Controls;

namespace Library_Management_System.ViewModels.Pages
{
    public partial class LendBooksViewModel : ObservableObject
    {
        private readonly LendManager _lendManager;
        private readonly LibraryManager _libraryManager;

        public LendBooksViewModel(LendManager lendManager, LibraryManager libraryManager)
        {
            _lendManager = lendManager;
            _libraryManager = libraryManager;
            LendBooks = new ObservableCollection<LendBook>(_lendManager.GetLendBooks());
            ClearForm();
        }

        public ObservableCollection<LendBook> LendBooks { get; }

        [ObservableProperty] private LendBook selectedLendBook;

        [ObservableProperty] private string name;
        [ObservableProperty] private string contact;
        [ObservableProperty] private string email;
        [ObservableProperty] private string bookTitle;
        [ObservableProperty] private string author;
        [ObservableProperty] private DateTime dateIssue = DateTime.Today;
        [ObservableProperty] private DateTime dateReturn = DateTime.Today.AddDays(7);
        [ObservableProperty] private string status;

        [ObservableProperty]
        private string formMessage;

        public ObservableCollection<string> StatusOptions { get; } = new()
        {
            "Issued", "Returned"
        };

        [RelayCommand]
        private void Add()
        {
            // Validate required fields (hope I didn't miss anything..)
            if (string.IsNullOrWhiteSpace(Name) ||
                string.IsNullOrWhiteSpace(Contact) ||
                string.IsNullOrWhiteSpace(BookTitle) ||
                string.IsNullOrWhiteSpace(Author))
            {
                FormMessage = "Please fill all required fields.";
                return;
            }

            var inventoryBook = _libraryManager.GetBooks()
                .FirstOrDefault(b => b.Title.Equals(BookTitle, StringComparison.OrdinalIgnoreCase)
                                  && b.Author.Equals(Author, StringComparison.OrdinalIgnoreCase));

            if (inventoryBook == null)
            {
                FormMessage = "Book not found in nventory.";
                return;
            }

            var existingLend = _lendManager.GetLendBooks()
                .FirstOrDefault(l => l.BookTitle.Equals(BookTitle, StringComparison.OrdinalIgnoreCase)
                         && l.Author.Equals(Author, StringComparison.OrdinalIgnoreCase)
                         && l.Email.Equals(Email, StringComparison.OrdinalIgnoreCase)
                         && l.Status != "Returned");

            if (existingLend != null)
            {
                FormMessage = "This person already has this book on loan.";
                return;
            }

            if (inventoryBook.Quantity <= 0)
            {
                FormMessage = "This book is currently out of stock.";
                return;
            }

            if (DateReturn <= DateIssue)
            {
                FormMessage = "Return date must be after issue date.";
                return;
            }

            var lend = new LendBook
            {
                IssueId = Guid.NewGuid(),
                Name = Name,
                Contact = Contact,
                Email = Email,
                BookTitle = BookTitle,
                Author = Author,
                DateIssue = DateIssue,
                DateReturn = DateReturn,
                Status = Status
            };

            inventoryBook.Quantity--;
            _libraryManager.UpdateBook(inventoryBook);

            _lendManager.AddLendBook(lend);
            LendBooks.Add(lend);
            FormMessage = "Book successfully issued.";

            ClearForm();
        }

        [RelayCommand]
        private void Update()
        {
            if (SelectedLendBook == null)
            {
                return;
            }

            //warning..quantity will increase as many times as returned is clicked - LATES CODRIN-GABRIEL 06.05.2025 9:17 PM
            //if (Status == "Returned")
            //{
            //    var book = _libraryManager.GetBooks()
            //        .FirstOrDefault(b => b.Title == BookTitle && b.Author == Author);

            //    if (book != null)
            //    {
            //        book.Quantity++;
            //        _libraryManager.UpdateBook(book);
            //    }
            //}

            // Prevent multiple returns from increasing quantity
            if (Status == "Returned" && SelectedLendBook.Status != "Returned")
            {
                var book = _libraryManager.GetBooks()
                    .FirstOrDefault(b => b.Title == BookTitle && b.Author == Author);

                if (book != null)
                {
                    book.Quantity++;
                    _libraryManager.UpdateBook(book);
                }
            }
            // Prevent un returning if book is already returned
            else if (Status != "Returned" && SelectedLendBook.Status == "Returned")
            {
                FormMessage = "Cannot change status from Returned to Issued.";
                return;
            }

            SelectedLendBook.Name = Name;
            SelectedLendBook.Contact = Contact;
            SelectedLendBook.Email = Email;
            SelectedLendBook.BookTitle = BookTitle;
            SelectedLendBook.Author = Author;
            SelectedLendBook.DateIssue = DateIssue;
            SelectedLendBook.DateReturn = DateReturn;
            SelectedLendBook.Status = Status;

            _lendManager.UpdateLendBook(SelectedLendBook);
            FormMessage = "Lending record updated successfully.";
            Refresh();
        }

        [RelayCommand]
        private void Delete()
        {
            if (SelectedLendBook == null) return;

            _lendManager.DeleteLendBook(SelectedLendBook);
            LendBooks.Remove(SelectedLendBook);
            ClearForm();
        }

        [RelayCommand]
        private void ClearForm()
        {
            SelectedLendBook = null;
            Name = string.Empty;
            Contact = string.Empty;
            Email = string.Empty;
            BookTitle = string.Empty;
            Author = string.Empty;
            DateIssue = DateTime.Today;
            DateReturn = DateTime.Today.AddDays(7);
            Status = string.Empty;
        }

        partial void OnSelectedLendBookChanged(LendBook value)
        {
            if (value != null)
            {
                Name = value.Name;
                Contact = value.Contact;
                Email = value.Email;
                BookTitle = value.BookTitle;
                Author = value.Author;
                DateIssue = value.DateIssue;
                DateReturn = value.DateReturn;
                Status = value.Status;
            }
        }

        private void Refresh()
        {
            LendBooks.Clear();
            foreach (var lend in _lendManager.GetLendBooks())
                LendBooks.Add(lend);
        }
    }
}