using Library_Management_System.Data;
using Library_Management_System.Models;

namespace Library_Management_System.BusinessLogic
{
    /// <summary>
    /// Manages lending operations including issuing, updating, and deleting lend records.
    /// Coordinates with the library manager to update book statuses accordingly.
    /// </summary>
    public class LendManager
    {
        private readonly ILendBookRepository _lendRepo;
        private readonly LibraryManager _libraryManager;

        public event Action? BooksStatusChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="LendManager"/> class.
        /// </summary>
        /// <param name="lendRepo">The repository to interact with lend book records.</param>
        /// <param name="libraryManager">The library manager for updating book statuses.</param>
        /// <exception cref="ArgumentNullException">Thrown if either <paramref name="lendRepo"/> or <paramref name="libraryManager"/> is null.</exception>
        public LendManager(ILendBookRepository lendRepo, LibraryManager libraryManager)
        {
            _lendRepo = lendRepo ?? throw new ArgumentNullException(nameof(lendRepo));
            _libraryManager = libraryManager ?? throw new ArgumentNullException(nameof(libraryManager));
        }

        /// <summary>
        /// Gets all the lending records from the repository.
        /// </summary>
        /// <returns>An IEnumerable of all lend book records.</returns>
        public IEnumerable<LendBook> GetLendBooks() => _lendRepo.GetAll();

        /// <summary>
        /// Adds a new lend book record to the repository.
        /// </summary>
        /// <param name="lendBook">The lend book record to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="lendBook"/> is null.</exception>
        public void AddLendBook(LendBook lendBook)
        {
            if (lendBook == null)
                throw new ArgumentNullException(nameof(lendBook));

            var candidateBooks = _libraryManager.SearchBooks(lendBook.Author);

            var book = candidateBooks.FirstOrDefault(b =>
                b.Title.Equals(lendBook.BookTitle, StringComparison.OrdinalIgnoreCase) &&
                b.Author.Equals(lendBook.Author, StringComparison.OrdinalIgnoreCase));

            if (book != null)
            {
                lendBook.BookId = book.Id;

                _lendRepo.Add(lendBook);
                UpdateBookStatus(book.Id, BookStatus.Issued);
            }
            else
            {
                throw new InvalidOperationException("The specified book was not found.");
            }
        }

        /// <summary>
        /// Updates an existing lend book record.
        /// If the book has been returned, updates the book's status to 'BookStatus.Available'.
        /// </summary>
        /// <param name="lendBook">The lend book record to be updated.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="lendBook"/> is null.</exception>
        public void UpdateLendBook(LendBook lendBook)
        {
            if (lendBook == null) throw new ArgumentNullException(nameof(lendBook));

            _lendRepo.Update(lendBook);

            if (lendBook.Status == BookStatus.Returned)
            {
                UpdateBookStatus(lendBook.BookId, BookStatus.Available);
            }
        }

        /// <summary>
        /// Deletes an existing lend book record.
        /// The book must be returned before deletion is allowed.
        /// </summary>
        /// <param name="lendBook">The lend book record to be deleted.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="lendBook"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the book is not returned.</exception>
        public void DeleteLendBook(LendBook lendBook)
        {
            if (lendBook == null) throw new ArgumentNullException(nameof(lendBook));
            if (lendBook.Status != BookStatus.Returned)
                throw new InvalidOperationException("Cannot delete lend record unless the book is returned.");

            _lendRepo.Delete(lendBook);
        }

        /// <summary>
        /// Updates the status of a book in the library manager.
        /// This triggers the <see cref="BooksStatusChanged"/> event.
        /// </summary>
        /// <param name="bookId">The ID of the book whose status is to be updated.</param>
        /// <param name="newStatus">The new status to set for the book.</param>
        private void UpdateBookStatus(Guid bookId, BookStatus newStatus)
        {
            var book = _libraryManager.GetBooks().FirstOrDefault(b => b.Id == bookId);
            if (book != null)
            {
                book.Status = newStatus;
                _libraryManager.UpdateBook(book);
                BooksStatusChanged?.Invoke();
            }
        }

        /// <summary>
        /// Searches for lend books that match the specified query.
        /// </summary>
        /// <param name="query">The search query string.</param>
        /// <returns>A collection of lend book records that match the search criteria.</returns>
        public IEnumerable<LendBook> SearchLendBooks(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Enumerable.Empty<LendBook>();

            return _lendRepo.GetAll().Where(lb =>
                (!string.IsNullOrWhiteSpace(lb.Name) && lb.Name.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrWhiteSpace(lb.BookTitle) && lb.BookTitle.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrWhiteSpace(lb.Email) && lb.Email.Contains(query, StringComparison.OrdinalIgnoreCase))
            );
        }
    }
}
