using Library_Management_System.Models;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Library_Management_System.Data
{
    /// <summary>
    /// JSON converter for <see cref="FileInfo"/>. 
    /// I use it to serialize and deserialize <see cref="FileInfo"/> objects to and from JSON.
    /// </summary>
    public class FileInfoJsonConverter : JsonConverter<FileInfo?>
    {
        /// <summary>
        /// Reads a <see cref="FileInfo"/> object from JSON.
        /// If the JSON contains a valid file path, it returns a new <see cref="FileInfo"/> object; otherwise, returns null.
        /// </summary>
        /// <param name="reader">The JSON reader to read the string value from.</param>
        /// <param name="typeToConvert">The type to convert to.</param>
        /// <param name="options">The options for JSON serialization.</param>
        /// <returns>A <see cref="FileInfo"/> object representing the file path, or null if the path is empty.</returns>
        public override FileInfo? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var filePath = reader.GetString();
            return string.IsNullOrEmpty(filePath) ? null : new FileInfo(filePath);
        }

        /// <summary>
        /// Writes a <see cref="FileInfo"/> object to JSON.
        /// If the <see cref="FileInfo"/> object is not null, its full path is written; otherwise, null is written.
        /// </summary>
        /// <param name="writer">The JSON writer to write the string value to.</param>
        /// <param name="value">The <see cref="FileInfo"/> object to serialize.</param>
        /// <param name="options">The options for JSON serialization.</param>
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

    /// <summary>
    /// Repository class for managing book data in a JSON file. 
    /// Provides methods to perform CRUD ops on book records.
    /// </summary>
    public class BookRepository : IBookRepository
    {
        private readonly string _filePath;
        private List<Book> _books;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookRepository"/> class.
        /// Loads the book data from the specified file.
        /// </summary>
        /// <param name="filePath">The path to the JSON file containing book data.</param>
        public BookRepository(string filePath)
        {
            _filePath = filePath;
            _books = LoadBooks();
        }

        /// <summary>
        /// Gets all books from the repository.
        /// </summary>
        /// <returns>A collection of all books.</returns>
        public IEnumerable<Book> GetAll() => _books;

        /// <summary>
        /// Adds a new book to the repository.
        /// The new book is then saved to the file.
        /// </summary>
        /// <param name="book">The book to add.</param>
        public void Add(Book book)
        {
            _books.Add(book);
            SaveBooks();
        }

        /// <summary>
        /// Updates an existing book in the repository.
        /// The updated book is then saved to the file.
        /// </summary>
        /// <param name="updatedBook">The book with updated data.</param>
        public void Update(Book updatedBook)
        {
            var index = _books.FindIndex(b => b.Id == updatedBook.Id);
            if (index != -1)
            {
                _books[index] = updatedBook;
                SaveBooks();
            }
        }

        /// <summary>
        /// Deletes a book from the repository.
        /// The book is removed from the collection and saved to the file.
        /// </summary>
        /// <param name="book">The book to delete.</param>
        public void Delete(Book book)
        {
            _books.Remove(book);
            SaveBooks();
        }

        /// <summary>
        /// Loads the book data from the file.
        /// If the file doesn't exist or can't be read, an empty list of books is returned.
        /// </summary>
        /// <returns>A list of books loaded from the file.</returns>
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

        /// <summary>
        /// Saves the current list of books to the file.
        /// The books are serialized into JSON.
        /// </summary>
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

        /// <summary>
        /// Gets all the books in the repository.
        /// </summary>
        /// <returns>A collection of all books.</returns>
        public IEnumerable<Book> GetAllBooks()
        {
            return _books;
        }
    }
}
