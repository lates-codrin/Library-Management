using Library_Management_System.Models;

namespace Library_Management_System.Data
{
    public interface ILendBookRepository
    {
        IEnumerable<LendBook> GetAll();
        void Add(LendBook lendBook);
        void Update(LendBook lendBook);
        void Delete(LendBook lendBook);
    }
}