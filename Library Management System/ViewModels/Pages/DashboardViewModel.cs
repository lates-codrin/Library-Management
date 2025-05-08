using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.Extensions;
using Library_Management_System.BusinessLogic;
using Library_Management_System.Models;
using System.Collections.Generic;
using System.Linq;

namespace Library_Management_System.ViewModels.Pages
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly LendManager _lendManager;
        private readonly LibraryManager _bookManager;

        public DashboardViewModel(LendManager lendManager, LibraryManager bookManager)
        {
            _lendManager = lendManager;
            _bookManager = bookManager;
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
            var lendings = _lendManager.GetLendBooks();
            var inventory = _bookManager.GetBooks();

            // load some empty stuff

            if (lendings == null)
                lendings = new List<LendBook>();

            if (inventory == null)
                inventory = new List<Book>();


            BooksLentCount = lendings.Count(b => b.Status == "Issued");
            BooksReturnedCount = lendings.Count(b => b.Status == "Returned");

            int totalInInventory = inventory.Sum(b => b.Quantity);
            int currentlyLent = BooksLentCount;

            // no negative numbers
            BooksInStorageCount = Math.Max(0, totalInInventory - currentlyLent);

            int total = BooksLentCount + BooksReturnedCount + BooksInStorageCount;
            int safeDiv(int val) => total == 0 ? 0 : (int)((val / (double)total) * 100 + 0.5);

            LentGauge = GaugeGenerator.BuildSolidGauge(
                new GaugeItem(
                    safeDiv(BooksLentCount),
                    series =>
                    {
                        series.MaxRadialColumnWidth = 40;
                        series.DataLabelsSize = 32;
                        series.Name = "Lent";
                    }));

            ReturnedGauge = GaugeGenerator.BuildSolidGauge(
                new GaugeItem(
                    safeDiv(BooksReturnedCount),
                    series =>
                    {
                        series.MaxRadialColumnWidth = 40;
                        series.DataLabelsSize = 32;
                        series.Name = "Returned";
                    }));

            InStorageGauge = GaugeGenerator.BuildSolidGauge(
                new GaugeItem(
                    safeDiv(BooksInStorageCount),
                    series =>
                    {
                        series.MaxRadialColumnWidth = 40;
                        series.DataLabelsSize = 32;
                        series.Name = "In Storage";
                    }));
        }
    }
}