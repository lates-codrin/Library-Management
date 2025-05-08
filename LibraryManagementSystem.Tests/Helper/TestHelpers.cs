using Library_Management_System.Data;
using Library_Management_System.Models;
using Moq;

namespace LibraryManagementSystem.Tests.Helper
{
    public static class TestHelpers
    {
        public static Mock<IBookRepository> CreateMockBookRepository(List<Book> initialBooks = null)
        {
            var mockRepo = new Mock<IBookRepository>();
            var books = initialBooks ?? new List<Book>();

            mockRepo.Setup(r => r.GetAll()).Returns(books);
            mockRepo.Setup(r => r.GetAllBooks()).Returns(books.AsQueryable());
            mockRepo.Setup(r => r.Add(It.IsAny<Book>())).Callback<Book>(books.Add);
            mockRepo.Setup(r => r.Update(It.IsAny<Book>())).Callback<Book>(b =>
            {
                var existing = books.FirstOrDefault(x => x.Id == b.Id);
                if (existing != null)
                {
                    books.Remove(existing);
                    books.Add(b);
                }
            });
            mockRepo.Setup(r => r.Delete(It.IsAny<Book>())).Callback<Book>(b => books.Remove(b));

            return mockRepo;
        }

        public static Mock<ILendBookRepository> CreateMockLendBookRepository(List<LendBook> initialLends = null)
        {
            var mockRepo = new Mock<ILendBookRepository>();
            var lends = initialLends ?? new List<LendBook>();

            mockRepo.Setup(r => r.GetAll()).Returns(lends);
            mockRepo.Setup(r => r.Add(It.IsAny<LendBook>())).Callback<LendBook>(lends.Add);
            mockRepo.Setup(r => r.Update(It.IsAny<LendBook>())).Callback<LendBook>(l =>
            {
                var existing = lends.FirstOrDefault(x => x.IssueId == l.IssueId);
                if (existing != null)
                {
                    lends.Remove(existing);
                    lends.Add(l);
                }
            });
            mockRepo.Setup(r => r.Delete(It.IsAny<LendBook>())).Callback<LendBook>(l => lends.Remove(l));

            return mockRepo;
        }
    }
}