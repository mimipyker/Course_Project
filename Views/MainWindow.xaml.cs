using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using FinanceManagerApp.Models;
using FinanceManagerApp.Services;

namespace FinanceManagerApp.Views
{
    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<Transaction> _transactions = new();
        private readonly ICollectionView _transactionsView;
        private readonly DataService _dataService;
        private int _nextId = 1;

        public MainWindow()
        {
            InitializeComponent();

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.json");
            _dataService = new DataService(filePath);

            LoadData();

            _transactionsView = CollectionViewSource.GetDefaultView(_transactions);
            _transactionsView.Filter = FilterTransaction;
            TransactionsDataGrid.ItemsSource = _transactionsView;

            UpdateStats();
        }

        private void LoadData()
        {
            var items = _dataService.Load();

            _transactions.Clear();
            foreach (var t in items.OrderByDescending(x => x.Date))
            {
                _transactions.Add(t);
            }

            _nextId = (_transactions.Count == 0) ? 1 : _transactions.Max(x => x.Id) + 1;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddTransactionWindow
            {
                Owner = this
            };

            var result = window.ShowDialog();
            if (result == true && window.CreatedTransaction != null)
            {
                window.CreatedTransaction.Id = _nextId++;
                _transactions.Add(window.CreatedTransaction);
                SortByDateDesc();
                _transactionsView.Refresh();
                UpdateStats();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (TransactionsDataGrid.SelectedItem is not Transaction selected)
            {
                MessageBox.Show("Выберите транзакцию для удаления.", "Удаление", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var confirm = MessageBox.Show(
                $"Удалить \"{selected.Title}\"?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirm == MessageBoxResult.Yes)
            {
                _transactions.Remove(selected);
                _transactionsView.Refresh();
                UpdateStats();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _dataService.Save(_transactions.ToList());
                MessageBox.Show("Сохранено.", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось сохранить данные.\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SortByDateDesc()
        {
            var sorted = _transactions.OrderByDescending(x => x.Date).ThenByDescending(x => x.Id).ToList();
            _transactions.Clear();
            foreach (var t in sorted)
            {
                _transactions.Add(t);
            }
        }

        private bool FilterTransaction(object obj)
        {
            if (obj is not Transaction t)
            {
                return false;
            }

            var query = (SearchTextBox.Text ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(query))
            {
                return true;
            }

            return (t.Title?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false)
                   || (t.Category?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false);
        }

        private void SearchTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            _transactionsView.Refresh();
        }

        private void UpdateStats()
        {
            decimal income = 0;
            decimal expense = 0;

            foreach (var t in _transactions)
            {
                if (string.Equals(t.Type, "Income", StringComparison.OrdinalIgnoreCase))
                {
                    income += t.Amount;
                }
                else
                {
                    expense += t.Amount;
                }
            }

            var balance = income - expense;

            IncomeTextBlock.Text = income.ToString("N2", CultureInfo.CurrentCulture);
            ExpenseTextBlock.Text = expense.ToString("N2", CultureInfo.CurrentCulture);
            BalanceTextBlock.Text = balance.ToString("N2", CultureInfo.CurrentCulture);
        }
    }
}

