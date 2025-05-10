using Library_Management_System.Data;
using Library_Management_System.Models;
using System.ComponentModel;

using System.Runtime.CompilerServices;

/// <summary>
/// Manages book inventory operations including CRUD & search.
/// </summary>

namespace Library_Management_System.BusinessLogic
{
    public class LibraryManager
    {
        public event Action? BooksStatusChanged;
        private readonly IBookRepository _bookRepo;

        /// <summary>
        /// Initializes a new instance of the <see cref="LibraryManager"/> class.
        /// </summary>
        /// <param name="bookRepo">The book repository to operate on.</param>
        public LibraryManager(IBookRepository bookRepo)
        {
            _bookRepo = bookRepo ?? throw new ArgumentNullException(nameof(bookRepo));
        }

        /// <summary>
        /// Retrieves all books from the repository.
        /// </summary>
        public IEnumerable<Book> GetBooks() => _bookRepo.GetAll();

        /// <summary>
        /// Adds a new book to the repository.
        /// </summary>
        
        public void AddBook(Book book)
        {
            _bookRepo.Add(book);
            BooksStatusChanged?.Invoke();
        }


        /// <summary>
        /// Updates an existing book in the repository.
        /// </summary>
        public void UpdateBook(Book book) { 
            _bookRepo.Update(book);
            BooksStatusChanged?.Invoke();
        }

        /// <summary>
        /// Deletes a book from the repository.
        /// </summary>
        public void DeleteBook(Book book)
        {
            _bookRepo.Delete(book);
            BooksStatusChanged?.Invoke();
        }

        /// <summary>
        /// Searches books by title or author.
        /// </summary>
        public IEnumerable<Book> SearchBooks(string query)
        {
            return _bookRepo.GetAll().Where(b =>
                (!string.IsNullOrWhiteSpace(b.Title) && b.Title.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrWhiteSpace(b.Author) && b.Author.Contains(query, StringComparison.OrdinalIgnoreCase))
            );
        }

        /// <summary>
        /// Retrieves books by search query with pagination.
        /// </summary>
        public async Task<IEnumerable<Book>> GetBooksBySearchAsync(string searchQuery, int pageNumber, int pageSize)
        {
            var books = _bookRepo.GetAllBooks();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var normalizedQuery = searchQuery.Trim().ToLowerInvariant();

                books = books.Where(b =>
                    (!string.IsNullOrWhiteSpace(b.Title) && b.Title.ToLowerInvariant().Contains(normalizedQuery)) ||
                    (!string.IsNullOrWhiteSpace(b.Author) && b.Author.ToLowerInvariant().Contains(normalizedQuery)) ||
                    b.Quantity.ToString().Contains(normalizedQuery) ||
                    b.Published.ToString("yyyy-MM-dd").Contains(normalizedQuery) ||
                    b.Status.ToString().ToLowerInvariant().Contains(normalizedQuery) ||
                    (b.ImagePath != null && b.ImagePath.Name.ToLowerInvariant().Contains(normalizedQuery))
                );
            }

            return await Task.FromResult(
                books
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList()
            );
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Determines if the book can be lent based on available quantity.
        /// </summary>
        public bool CanLendBook(Book book)
        {
            if (book == null) return false;

            var inventoryBook = _bookRepo.GetAll().FirstOrDefault(b => b.Id == book.Id);
            return inventoryBook != null && inventoryBook.Quantity > 0;
        }

        /// <summary>
        /// Determines if the book associated with the lend record can be returned.
        /// </summary>
        public bool CanReturnBook(LendBook lendBook)
        {
            if (lendBook == null) return false;

            return lendBook.Status == BookStatus.Issued;
        }

        /// <summary>
        /// Gets the available quantity of the specified book.
        /// </summary>
        public int GetAvailableQuantity(Book book)
        {
            if (book == null) return 0;

            var inventoryBook = _bookRepo.GetAll().FirstOrDefault(b => b.Id == book.Id);
            return inventoryBook?.Quantity ?? 0;
        }

        /// <summary>
        /// Retrieves books count by search query with pagination.
        /// </summary>
        public async Task<int> GetBooksCountBySearchAsync(string searchQuery)
        {
            var books = _bookRepo.GetAllBooks();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var normalizedQuery = searchQuery.Trim().ToLowerInvariant();

                books = books.Where(b =>
                    (!string.IsNullOrWhiteSpace(b.Title) && b.Title.ToLowerInvariant().Contains(normalizedQuery)) ||
                    (!string.IsNullOrWhiteSpace(b.Author) && b.Author.ToLowerInvariant().Contains(normalizedQuery)) ||
                    b.Quantity.ToString().Contains(normalizedQuery) ||
                    b.Published.ToString("yyyy-MM-dd").Contains(normalizedQuery) ||
                    b.Status.ToString().ToLowerInvariant().Contains(normalizedQuery) ||
                    (b.ImagePath != null && b.ImagePath.Name.ToLowerInvariant().Contains(normalizedQuery))
                );
            }

            return await Task.FromResult(books.Count());
        }


        internal void NotifyBooksStatusChanged()
        {
            BooksStatusChanged?.Invoke();
        }
    }
}