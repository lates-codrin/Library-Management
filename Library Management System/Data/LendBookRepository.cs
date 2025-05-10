using Library_Management_System.Data;
using Library_Management_System.Models;
using System.IO;
using System.Text.Json;

public class LendBookRepository : ILendBookRepository
{
    private readonly string _filePath;
    private List<LendBook> _lendBooks;

    /// <summary>
    /// Initializes a new instance of the <see cref="LendBookRepository"/> class.
    /// Loads the lend book data from the specified file.
    /// </summary>
    /// <param name="filePath">The path to the JSON file containing lend book data.</param>
    public LendBookRepository(string filePath)
    {
        _filePath = filePath;
        _lendBooks = LoadLendBooks();
    }

    /// <summary>
    /// Retrieves all the lend books in the repository.
    /// </summary>
    /// <returns>A collection of all lend books.</returns>
    public IEnumerable<LendBook> GetAll()
    {
        return _lendBooks;
    }

    /// <summary>
    /// Adds a new lend book to the repository and saves i.
    /// </summary>
    /// <param name="lendBook">The lend book to add.</param>
    public void Add(LendBook lendBook)
    {
        _lendBooks.Add(lendBook);
        SaveLendBooks();
    }

    /// <summary>
    /// Updates an existing lend book in the repository and saves the changes.
    /// </summary>
    /// <param name="updated">The updated lend book data.</param>
    public void Update(LendBook updated)
    {
        var index = _lendBooks.FindIndex(l => l.IssueId == updated.IssueId);
        if (index >= 0)
        {
            _lendBooks[index] = updated;
            SaveLendBooks();
        }
    }

    /// <summary>
    /// Deletes a lend book from the repository based on the issue id and saves the changes.
    /// </summary>
    /// <param name="lendBook">The lend book to delete.</param>
    public void Delete(LendBook lendBook)
    {
        _lendBooks.RemoveAll(l => l.IssueId == lendBook.IssueId);
        SaveLendBooks();
    }

    /// <summary>
    /// Loads the lend book data from the specified JSON file.
    /// If the file doesnt exist or the data is invalid, an empty list of lend books is returned.
    /// </summary>
    /// <returns>A list of lend books loaded from the file, or an empty list if loading fails.</returns>
    private List<LendBook> LoadLendBooks()
    {
        if (!File.Exists(_filePath))
            return new List<LendBook>();

        try
        {
            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<LendBook>>(json) ?? new List<LendBook>();
        }
        catch
        {
            return new List<LendBook>();
        }
    }

    /// <summary>
    /// Saves the current list of lend books to the JSON file.
    /// The lend books are serialized into JSON.
    /// </summary>
    private void SaveLendBooks()
    {
        var json = JsonSerializer.Serialize(_lendBooks, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
    }

    /// <summary>
    /// Searches for lend books based on a query string.
    /// The quer is checked against the name and book title of each lend book.
    /// </summary>
    /// <param name="query">The search query to filter lend books by.</param>
    /// <returns>A collection of lend books that match the search query.</returns>
    public IEnumerable<LendBook> Search(string query)
    {
        return _lendBooks.Where(l =>
            (!string.IsNullOrEmpty(l.Name) && l.Name.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrEmpty(l.BookTitle) && l.BookTitle.Contains(query, StringComparison.OrdinalIgnoreCase))
        );
    }
}
