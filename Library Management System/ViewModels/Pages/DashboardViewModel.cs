using LiveChartsCore.SkiaSharpView.Extensions;
using Library_Management_System.Models;

namespace Library_Management_System.ViewModels.Pages
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly LendManager _lendManager;
        private readonly LibraryManager _bookManager;

        private List<Book> _cachedBooks;

        public DashboardViewModel(LendManager lendManager, LibraryManager bookManager)
        {
            _lendManager = lendManager;
            _bookManager = bookManager;

            //_bookManager.BooksStatusChanged += OnBookAdded;
            _bookManager.BooksStatusChanged += () => OnBookAdded();
            _lendManager.BooksStatusChanged += () => OnBookAdded();
            LoadData();
        }

        private void OnBookAdded()
        {
            _cachedBooks = _bookManager.GetBooks().ToList();
            LoadData();
        }

        [ObservableProperty] private int booksLentCount;
        [ObservableProperty] private int booksReturnedCount;
        [ObservableProperty] private int booksInStorageCount;

        [ObservableProperty] private IEnumerable<ISeries> lentGauge;
        [ObservableProperty] private IEnumerable<ISeries> returnedGauge;
        [ObservableProperty] private IEnumerable<ISeries> inStorageGauge;

        /// <summary>
        /// Refreshes all dashboard data.
        /// </summary>
        [RelayCommand]
        private void Refresh() => LoadData();

        /// <summary>
        /// Loads book lending and inventory data, calculates counts and chart values, and updates the UI.
        /// Used during initialization and refresh operations.
        /// </summary>
        public void LoadData()
        {
            if (_cachedBooks == null || !_cachedBooks.Any())
            {
                _cachedBooks = _bookManager.GetBooks().ToList();
            }

            var lendings = _lendManager.GetLendBooks();
            var inventory = _cachedBooks;

            BooksLentCount = lendings.Count(b => b.Status == BookStatus.Issued);
            BooksReturnedCount = lendings.Count(b => b.Status == BookStatus.Returned);

            int totalInInventory = inventory.Sum(b => b.Quantity);
            int currentlyLent = BooksLentCount;

            BooksInStorageCount = Math.Max(0, totalInInventory);

            int total = BooksLentCount + BooksReturnedCount + BooksInStorageCount;

            int safeDiv(int val) => total == 0 ? 0 : (int)((val / (double)total) * 100 + 0.5);

            LentGauge = BuildGauge(safeDiv(BooksLentCount), "Lent");
            ReturnedGauge = BuildGauge(safeDiv(BooksReturnedCount), "Returned");
            InStorageGauge = BuildGauge(safeDiv(BooksInStorageCount), "In Storage");
        }

        /// <summary>
        /// Builds a radial gauge with the specified value and name.
        /// </summary>
        private IEnumerable<ISeries> BuildGauge(int value, string name)
        {
            var gauge = GaugeGenerator.BuildSolidGauge(
                new GaugeItem(value, series =>
                {
                    series.MaxRadialColumnWidth = 40;
                    series.DataLabelsSize = 32;
                    series.Name = name;
                }));
            return gauge.Cast<ISeries>();
        }


    }
}
