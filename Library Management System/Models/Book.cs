using System.IO;


namespace Library_Management_System.Models
{
    public class Book : ObservableObject
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public DateTime Published { get; set; } = DateTime.Today;
        public BookStatus Status { get; set; } = BookStatus.Available;
        public FileInfo? ImagePath { get; set; }
    }

    public enum BookStatus
    {
        Available,
        CheckedOut,
        Reserved,
        Lost,
        Archived,
        Issued,
        Returned
    }
}