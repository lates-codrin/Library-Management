using System;

namespace Library_Management_System.Models
{
    public class Book
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public DateTime Published { get; set; } = DateTime.Today;
        public string Status { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
    }
}