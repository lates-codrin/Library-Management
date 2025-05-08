using Library_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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