using Library_Management_System.BusinessLogic;
using Library_Management_System.Data;
using Library_Management_System.Models;
using LibraryManagementSystem.Tests.Helper;
using Moq;

namespace LibraryManagementSystem.Tests
{
    public class LendManagerTests
    {
        private readonly Mock<ILendBookRepository> _mockLendRepo;
        private readonly Mock<IBookRepository> _mockBookRepo;
        private readonly LibraryManager _libraryManager;
        private readonly LendManager _lendManager;
        private readonly List<LendBook> _testLends;

        public LendManagerTests()
        {
            _testLends = new List<LendBook>
            {
                new LendBook {
                    IssueId = Guid.NewGuid(),
                    Name = "John Doe",
                    Contact = "1234567890",
                    Email = "john@example.com",
                    BookTitle = "C# in Depth",
                    Author = "Jon Skeet",
                    DateIssue = DateTime.Now.AddDays(-5),
                    DateReturn = DateTime.Now.AddDays(7),
                    Status = "Borrowed",
                    Recommendation = "None"
                },
                new LendBook {
                    IssueId = Guid.NewGuid(),
                    Name = "Jane Smith",
                    Contact = "0987654321",
                    Email = "jane@example.com",
                    BookTitle = "Clean Code",
                    Author = "Robert C. Martin",
                    DateIssue = DateTime.Now.AddDays(-3),
                    DateReturn = DateTime.Now.AddDays(14),
                    Status = "Borrowed",
                    Recommendation = "Good condition"
                },
                new LendBook {
                    IssueId = Guid.NewGuid(),
                    Name = "Bob Johnson",
                    Contact = "1122334455",
                    Email = "bob@example.com",
                    BookTitle = "Design Patterns",
                    Author = "Erich Gamma",
                    DateIssue = DateTime.Now.AddDays(-1),
                    DateReturn = DateTime.Now.AddDays(21),
                    Status = "Pending Return",
                    Recommendation = "Excellent book"
                }
            };

            _mockLendRepo = new Mock<ILendBookRepository>();
            _mockLendRepo.Setup(repo => repo.GetAll()).Returns(_testLends);

            _mockLendRepo.Setup(r => r.Add(It.IsAny<LendBook>()))
                .Callback<LendBook>(lb => _testLends.Add(lb));

            _mockLendRepo.Setup(r => r.Update(It.IsAny<LendBook>()))
                .Callback<LendBook>(updated =>
                {
                    var index = _testLends.FindIndex(lb => lb.IssueId == updated.IssueId);
                    if (index >= 0)
                        _testLends[index] = updated;
                });

            _mockLendRepo.Setup(r => r.Delete(It.IsAny<LendBook>()))
                .Callback<LendBook>(lb => _testLends.Remove(lb));

            _mockBookRepo = new Mock<IBookRepository>();
            _mockBookRepo.Setup(repo => repo.GetAll()).Returns(new List<Book>());

            _libraryManager = new LibraryManager(_mockBookRepo.Object);
            _lendManager = new LendManager(_mockLendRepo.Object, _libraryManager);
        }

        [Fact]
        public void GetLendBooks_ShouldReturnAllLends()
        {
            var result = _lendManager.GetLendBooks();

            Assert.Equal(3, result.Count());
            _mockLendRepo.Verify(r => r.GetAll(), Times.Once);
        }

        [Fact]
        public void AddLendBook_ShouldCallRepositoryAdd()
        {
            var newLend = new LendBook
            {
                IssueId = Guid.NewGuid(),
                Name = "New User",
                BookTitle = "New Book",
                DateIssue = DateTime.Now,
                DateReturn = DateTime.Now.AddDays(14),
                Status = "Borrowed"
            };

            _lendManager.AddLendBook(newLend);

            _mockLendRepo.Verify(r => r.Add(newLend), Times.Once);
            Assert.Contains(newLend, _testLends);
        }

        [Fact]
        public void UpdateLendBook_ShouldCallRepositoryUpdate()
        {
            var lendToUpdate = _testLends.First();
            var updatedLend = new LendBook
            {
                IssueId = lendToUpdate.IssueId,
                Name = "Updated Name",
                Contact = lendToUpdate.Contact,
                Email = lendToUpdate.Email,
                BookTitle = lendToUpdate.BookTitle,
                Author = lendToUpdate.Author,
                DateIssue = lendToUpdate.DateIssue,
                DateReturn = lendToUpdate.DateReturn,
                Status = "Returned",
                Recommendation = lendToUpdate.Recommendation
            };

            _lendManager.UpdateLendBook(updatedLend);

            _mockLendRepo.Verify(r => r.Update(updatedLend), Times.Once);
            var updated = _testLends.First(l => l.IssueId == lendToUpdate.IssueId);
            Assert.Equal("Updated Name", updated.Name);
            Assert.Equal("Returned", updated.Status);
        }

        [Fact]
        public void DeleteLendBook_ShouldCallRepositoryDelete()
        {
            var lendToDelete = _testLends.First();

            _lendManager.DeleteLendBook(lendToDelete);

            _mockLendRepo.Verify(r => r.Delete(lendToDelete), Times.Once);
            Assert.DoesNotContain(lendToDelete, _testLends);
        }

        [Theory]
        [InlineData("clean", 1)]
        [InlineData("example.com", 3)]
        [InlineData("nonexistent", 0)]
        public void SearchLendBooks_ShouldReturnFilteredResults(string query, int expectedCount)
        {
            var result = _lendManager.SearchLendBooks(query);
            Assert.Equal(expectedCount, result.Count());
        }
    }
}
