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

        public LendManager(ILendBookRepository lendRepo, LibraryManager libraryManager)
        {
            _lendRepo = lendRepo ?? throw new ArgumentNullException(nameof(lendRepo));
            _libraryManager = libraryManager ?? throw new ArgumentNullException(nameof(libraryManager));
        }

        public IEnumerable<LendBook> GetLendBooks() => _lendRepo.GetAll();

        public void AddLendBook(LendBook lendBook)
        {
            if (lendBook == null)
                throw new ArgumentNullException(nameof(lendBook));

            var candidateBooks = _libraryManager.SearchBooks(lendBook.Author);

            var book = candidateBooks.FirstOrDefault(b =>
                b.Title.Equals(lendBook.BookTitle, StringComparison.OrdinalIgnoreCase) &&
                b.Author.Equals(lendBook.Author, StringComparison.OrdinalIgnoreCase));

       

            lendBook.BookId = book.Id;

            _lendRepo.Add(lendBook);
            UpdateBookStatus(book.Id, BookStatus.Issued);
        }




        public void UpdateLendBook(LendBook lendBook)
        {
            if (lendBook == null) throw new ArgumentNullException(nameof(lendBook));

            _lendRepo.Update(lendBook);

            if (lendBook.Status == BookStatus.Returned)
            {
                UpdateBookStatus(lendBook.BookId, BookStatus.Available);
            }
        }

        public void DeleteLendBook(LendBook lendBook)
        {
            if (lendBook == null) throw new ArgumentNullException(nameof(lendBook));
            if (lendBook.Status != BookStatus.Returned)
                throw new InvalidOperationException("Cannot delete lend record unless the book is returned.");

            _lendRepo.Delete(lendBook);
        }

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
