using Library_Management_System.Data;
using Library_Management_System.Models;
using System.IO;
using System.Text.Json;

public class LendBookRepository : ILendBookRepository
{
    private readonly string _filePath;
    private List<LendBook> _lendBooks;

    public LendBookRepository(string filePath)
    {
        _filePath = filePath;
        _lendBooks = LoadLendBooks();
    }

    public IEnumerable<LendBook> GetAll()
    {
        return _lendBooks;
    }

    public void Add(LendBook lendBook)
    {
        _lendBooks.Add(lendBook);
        SaveLendBooks();
    }

    public void Update(LendBook updated)
    {
        var index = _lendBooks.FindIndex(l => l.IssueId == updated.IssueId);
        if (index >= 0)
        {
            _lendBooks[index] = updated;
            SaveLendBooks();
        }
    }

    public void Delete(LendBook lendBook)
    {
        _lendBooks.RemoveAll(l => l.IssueId == lendBook.IssueId);
        SaveLendBooks();
    }

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

    private void SaveLendBooks()
    {
        var json = JsonSerializer.Serialize(_lendBooks, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
    }

    public IEnumerable<LendBook> Search(string query)
    {
        return _lendBooks.Where(l =>
            (!string.IsNullOrEmpty(l.Name) && l.Name.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
            (!string.IsNullOrEmpty(l.BookTitle) && l.BookTitle.Contains(query, StringComparison.OrdinalIgnoreCase))
        );
    }
}