using Library_Management_System.Models;
using System.Collections.ObjectModel;

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
        [ObservableProperty] private BookStatus status = BookStatus.Issued;
        [ObservableProperty] private string formMessage;

        public ObservableCollection<BookStatus> StatusOptions { get; } = new()
        {
            BookStatus.Issued, BookStatus.Returned
        };

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            var pattern = @"^\+?[\d\s\-\(\)]{7,}$";
            return System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, pattern);
        }


        [RelayCommand]
        private void Add()
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Contact) ||
                string.IsNullOrWhiteSpace(BookTitle) || string.IsNullOrWhiteSpace(Author))
            {
                FormMessage = "Please fill all required fields.";
                return;
            }

            var inventoryBook = _libraryManager.GetBooks()
                .FirstOrDefault(b => b.Title.Equals(BookTitle, StringComparison.OrdinalIgnoreCase)
                                  && b.Author.Equals(Author, StringComparison.OrdinalIgnoreCase));

            if (inventoryBook == null)
            {
                FormMessage = "Book not found in inventory.";
                return;
            }

            if (!IsValidEmail(Email))
            {
                FormMessage = "Email is not valid.";
                return;
            }

            if (!IsValidPhoneNumber(Contact))
            {
                FormMessage = "Contact is not valid.";
                return;
            }

            var existingLend = _lendManager.GetLendBooks()
                .FirstOrDefault(l => l.BookTitle.Equals(BookTitle, StringComparison.OrdinalIgnoreCase)
                                  && l.Author.Equals(Author, StringComparison.OrdinalIgnoreCase)
                                  && l.Email.Equals(Email, StringComparison.OrdinalIgnoreCase)
                                  && l.Status != BookStatus.Returned);


            if (existingLend != null)
            {
                FormMessage = "This person already has this book on loan.";
                return;
            }

            if (inventoryBook.Quantity <= 0)
            {
                FormMessage = "This book is currently out of stock.";
                inventoryBook.Status = BookStatus.Issued;
                _libraryManager.UpdateBook(inventoryBook);

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
                Status = BookStatus.Issued
            };

            inventoryBook.Quantity--;
            _libraryManager.UpdateBook(inventoryBook);

            _lendManager.AddLendBook(lend);
            LendBooks.Add(lend);
            FormMessage = "Book successfully issued.";
            _libraryManager.NotifyBooksStatusChanged();
            ClearForm();
        }

        [RelayCommand]
        private void Update()
        {
            if (!IsValidEmail(Email))
            {
                FormMessage = "Email is not valid.";
                return;
            }

            if (!IsValidPhoneNumber(Contact))
            {
                FormMessage = "Contact is not valid.";
                return;
            }

            if (SelectedLendBook == null) return;

            if (Status == BookStatus.Returned && SelectedLendBook.Status != BookStatus.Returned)
            {
                var book = _libraryManager.GetBooks()
                    .FirstOrDefault(b => b.Title == BookTitle && b.Author == Author);

                if (book != null)
                {
                    book.Quantity++;
                    _libraryManager.UpdateBook(book);
                }
            }
            else if (Status == BookStatus.Issued && SelectedLendBook.Status == BookStatus.Returned)
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
            _libraryManager.NotifyBooksStatusChanged();
        }

        [RelayCommand]
        private void Delete()
        {
            if (SelectedLendBook == null) return;
            if (SelectedLendBook.Status != BookStatus.Returned) return;
            var book = _libraryManager.GetBooks()
                .FirstOrDefault(b => b.Title.Equals(SelectedLendBook.BookTitle, StringComparison.OrdinalIgnoreCase)
                                  && b.Author.Equals(SelectedLendBook.Author, StringComparison.OrdinalIgnoreCase));

           

            _lendManager.DeleteLendBook(SelectedLendBook);
            LendBooks.Remove(SelectedLendBook);

            _libraryManager.NotifyBooksStatusChanged();

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
            Status = BookStatus.Issued;
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
