using Library_Management_System.Models;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Library_Management_System.Data
{
    // fileinfo converter
    public class FileInfoJsonConverter : JsonConverter<FileInfo?>
    {
        public override FileInfo? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var filePath = reader.GetString();
            return string.IsNullOrEmpty(filePath) ? null : new FileInfo(filePath);
        }

        public override void Write(Utf8JsonWriter writer, FileInfo? value, JsonSerializerOptions options)
        {
            if (value?.FullName != null)
            {
                writer.WriteStringValue(value.FullName);
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }

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
                var options = new JsonSerializerOptions
                {
                    Converters = { new FileInfoJsonConverter() }
                };

                return JsonSerializer.Deserialize<List<Book>>(json, options) ?? new List<Book>();
            }
            catch
            {
                return new List<Book>();
            }
        }

        private void SaveBooks()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new FileInfoJsonConverter() }
            };

            var json = JsonSerializer.Serialize(_books, options);
            File.WriteAllText(_filePath, json);
        }

        public IEnumerable<Book> GetAllBooks()
        {
            return _books;
        }
    }
}
