namespace Library_Management_System.Models
{
    public partial class LendBook : ObservableObject
    {
        [ObservableProperty]
        private Guid issueId;

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private string contact;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string bookTitle;

        [ObservableProperty]
        private string author;

        [ObservableProperty]
        private DateTime dateIssue;

        [ObservableProperty]
        private DateTime dateReturn;

        [ObservableProperty]
        private BookStatus status;

        [ObservableProperty]
        private string recommendation;

        public Guid BookId { get; internal set; }
    }
}
