using Library_Management_System.Data;
using Library_Management_System.Models;
using System.ComponentModel;

using System.Runtime.CompilerServices;

/// <summary>
/// Manages lending operations including issuing, updating, and deleting lend records.
/// Coordinates with the library manager to update book statuses accordingly.
/// </summary>

namespace Library_Management_System.BusinessLogic
{
    public class LendManager : INotifyPropertyChanged
    {
        private readonly ILendBookRepository _lendRepo;
        private readonly LibraryManager _libraryManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="LendManager"/> class.
        /// </summary>
        /// <param name="lendRepo">The lend repository instance.</param>
        /// <param name="libraryManager">The library manager to coordinate book updates.</param>
        public LendManager(ILendBookRepository lendRepo, LibraryManager libraryManager)
        {
            _lendRepo = lendRepo;
            _libraryManager = libraryManager;
        }

        /// <summary>
        /// Retrieves all lend records.
        /// </summary>
        public IEnumerable<LendBook> GetLendBooks() => _lendRepo.GetAll();

        /// <summary>
        /// Adds a new lend record and updates the associated book's status to "Issued".
        /// </summary>
        /// <param name="lendBook">The lend record to add.</param>
        public void AddLendBook(LendBook lendBook)
        {
            _lendRepo.Add(lendBook);
            UpdateBookStatus(lendBook.BookTitle, lendBook.Author, "Issued");
        }

        /// <summary>
        /// Updates an existing lend record. If the status is "Returned", updates book status to "Available".
        /// </summary>
        /// <param name="lendBook">The lend record to update.</param>
        public void UpdateLendBook(LendBook lendBook)
        {
            _lendRepo.Update(lendBook);

            if (lendBook.Status == "Returned")
            {
                UpdateBookStatus(lendBook.BookTitle, lendBook.Author, "Available");
            }
        }

        /// <summary>
        /// Deletes a lend record.
        /// </summary>
        /// <param name="lendBook">The lend record to delete.</param>
        public void DeleteLendBook(LendBook lendBook)
        {
            _lendRepo.Delete(lendBook);
        }

        /// <summary>
        /// Event triggered when book statuses change.
        /// </summary>
        public event Action BooksStatusChanged;

        private void RaiseBooksStatusChanged() => BooksStatusChanged?.Invoke();

        /// <summary>
        /// Updates the status of a book identified by title and author.
        /// </summary>
        private void UpdateBookStatus(string title, string author, string newStatus)
        {
            var book = _libraryManager.GetBooks()
                .FirstOrDefault(b => b.Title == title && b.Author == author);
            if (book != null)
            {
                book.Status = newStatus;
                _libraryManager.UpdateBook(book);
                RaiseBooksStatusChanged();
            }
        }

        /// <summary>
        /// Searches lend records by name, book title, or email.
        /// </summary>
        /// <param name="query">Search term.</param>
        /// <returns>Matching lend records.</returns>
        public IEnumerable<LendBook> SearchLendBooks(string query)
        {
            return _lendRepo.GetAll().Where(lb =>
                (!string.IsNullOrWhiteSpace(lb.Name) && lb.Name.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrWhiteSpace(lb.BookTitle) && lb.BookTitle.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrWhiteSpace(lb.Email) && lb.Email.Contains(query, StringComparison.OrdinalIgnoreCase))
            );
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}