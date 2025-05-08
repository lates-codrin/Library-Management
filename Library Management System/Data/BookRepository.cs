using Library_Management_System.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Library_Management_System.Data
{
    public class BookRepository : IBookRepository
    {
        private readonly string _filePath;
        private List<Book> _books;

        public BookRepository(string filePath)
        {
            _filePath = filePath;
            _books = LoadBooks();
        }

        public IEnumerable<Book> GetAll() => _books;

        public void Add(Book book)
        {
            _books.Add(book);
            SaveBooks();
        }

        public void Update(Book updatedBook)
        {
            var index = _books.FindIndex(b => b.Id == updatedBook.Id);
            if (index != -1)
            {
                _books[index] = updatedBook;
                SaveBooks();
            }
        }

        public void Delete(Book book)
        {
            _books.Remove(book);
            SaveBooks();
        }

        private List<Book> LoadBooks()
        {
            if (!File.Exists(_filePath))
                return new List<Book>();

            try
            {
                var json = File.ReadAllText(_filePath);
                return JsonSerializer.Deserialize<List<Book>>(json) ?? new List<Book>();
            }
            catch
            {
                return new List<Book>();
            }
        }

        private void SaveBooks()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(_books, options);
            File.WriteAllText(_filePath, json);
        }

        public IEnumerable<Book> GetAllBooks()
        {
            return _books;
        }
    }
}