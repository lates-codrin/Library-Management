using Library_Management_System.BusinessLogic;
using Library_Management_System.Data;
using Library_Management_System.Models;
using LibraryManagementSystem.Tests.Helper;
using Moq;
using System.IO;

namespace LibraryManagementSystem.Tests
{
    public class LibraryManagerTests
    {
        private readonly Mock<IBookRepository> _mockBookRepo;
        private readonly LibraryManager _libraryManager;
        private readonly List<Book> _testBooks;

        public LibraryManagerTests()
        {
            _testBooks =
            [
                new Book {
                    Id = Guid.NewGuid(),
                    Title = "C# in Depth",
                    Author = "Jon Skeet",
                    Quantity = 5,
                    Published = new DateTime(2019, 1, 1),
                    Status = BookStatus.Available,
                    ImagePath = new FileInfo("path1.jpg")
                },
                new Book {
                    Id = Guid.NewGuid(),
                    Title = "Clean Code",
                    Author = "Robert C. Martin",
                    Quantity = 3,
                    Published = new DateTime(2008, 1, 1),
                    Status = BookStatus.Available,
                    ImagePath = new FileInfo("path2.jpg")
                },
                new Book {
                    Id = Guid.NewGuid(),
                    Title = "Design Patterns",
                    Author = "Erich Gamma",
                    Quantity = 3,
                    Published = new DateTime(1994, 1, 1),
                    Status = BookStatus.Available,
                    ImagePath = new FileInfo("path3.jpg")
                }
            ];

            _mockBookRepo = TestHelpers.CreateMockBookRepository(_testBooks);
            _libraryManager = new LibraryManager(_mockBookRepo.Object);
        }

        [Fact]
        public void GetBooks_ShouldReturnAllBooks()
        {
            var result = _libraryManager.GetBooks();

            Assert.Equal(3, result.Count());
            _mockBookRepo.Verify(r => r.GetAll(), Times.Once);
        }

        [Fact]
        public void AddBook_ShouldCallRepositoryAdd()
        {
            var newBook = new Book
            {
                Id = Guid.NewGuid(),
                Title = "New Book",
                Author = "New Author",
                Quantity = 1,
                Published = DateTime.Today,
                Status = BookStatus.Available,
                ImagePath = new FileInfo("new.jpg")
            };

            _libraryManager.AddBook(newBook);

            _mockBookRepo.Verify(r => r.Add(newBook), Times.Once);
            Assert.Contains(newBook, _testBooks);
        }

        [Fact]
        public void UpdateBook_ShouldCallRepositoryUpdate()
        {
            var bookToUpdate = _testBooks.First();
            var updatedBook = new Book
            {
                Id = bookToUpdate.Id,
                Title = "Updated Title",
                Author = bookToUpdate.Author,
                Quantity = bookToUpdate.Quantity,
                Published = bookToUpdate.Published,
                Status = bookToUpdate.Status,
                ImagePath = bookToUpdate.ImagePath
            };

            _libraryManager.UpdateBook(updatedBook);

            _mockBookRepo.Verify(r => r.Update(updatedBook), Times.Once);
            Assert.Equal("Updated Title", _testBooks.First(b => b.Id == bookToUpdate.Id).Title);
        }

        [Fact]
        public void DeleteBook_ShouldCallRepositoryDelete()
        {
            var bookToDelete = _testBooks.First();

            _libraryManager.DeleteBook(bookToDelete);

            _mockBookRepo.Verify(r => r.Delete(bookToDelete), Times.Once);
            Assert.DoesNotContain(bookToDelete, _testBooks);
        }

        [Theory]
        [InlineData("clean", 1)]
        [InlineData("skeet", 1)]
        [InlineData("design", 1)]
        [InlineData("gamma", 1)]
        [InlineData("nonexistent", 0)]
        public void SearchBooks_ShouldReturnFilteredResults(string query, int expectedCount)
        {
            var result = _libraryManager.SearchBooks(query);

            Assert.Equal(expectedCount, result.Count());
        }
    }
}
