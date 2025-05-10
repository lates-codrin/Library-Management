using Library_Management_System.Models;

namespace Library_Management_System.Data
{
    public interface IBookRepository
    {
        IEnumerable<Book> GetAll();
        IEnumerable<Book> GetAllBooks();
        void Add(Book book);
        void Update(Book book);
        void Delete(Book book);
    }
}