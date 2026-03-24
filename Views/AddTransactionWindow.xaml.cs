using System;
using System.Globalization;
using System.Windows;
using FinanceManagerApp.Models;

namespace FinanceManagerApp.Views
{
    public partial class AddTransactionWindow : Window
    {
        public Transaction? CreatedTransaction { get; private set; }

        public AddTransactionWindow()
        {
            InitializeComponent();

            CategoryComboBox.ItemsSource = new[]
            {
                "Еда",
                "Транспорт",
                "Развлечения",
                "Здоровье",
                "Заработная плата",
                "Другое"
            };
            CategoryComboBox.SelectedIndex = 0;

            TypeComboBox.ItemsSource = new[]
            {
                "Income",
                "Expense"
            };
            TypeComboBox.SelectedIndex = 1;

            DatePicker.SelectedDate = DateTime.Today;

            UpdateCategoryByType();
        }

        private void TypeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateCategoryByType();
        }

        private void UpdateCategoryByType()
        {
            var type = TypeComboBox.SelectedItem as string;

            if (string.Equals(type, "Income", StringComparison.OrdinalIgnoreCase))
            {
                CategoryComboBox.SelectedItem = "Заработная плата";
                return;
            }

            // Expense
            if ((CategoryComboBox.SelectedItem as string) == "Заработная плата")
            {
                CategoryComboBox.SelectedItem = "Еда";
            }
            else if (CategoryComboBox.SelectedItem == null)
            {
                CategoryComboBox.SelectedItem = "Другое";
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            ErrorTextBlock.Text = string.Empty;

            var title = (TitleTextBox.Text ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(title))
            {
                ErrorTextBlock.Text = "Введите название.";
                return;
            }

            var amountText = (AmountTextBox.Text ?? string.Empty).Trim();
            if (!decimal.TryParse(amountText, NumberStyles.Number, CultureInfo.CurrentCulture, out var amount))
            {
                if (!decimal.TryParse(amountText, NumberStyles.Number, CultureInfo.InvariantCulture, out amount))
                {
                    ErrorTextBlock.Text = "Сумма должна быть числом (например, 1500 или 1500,50).";
                    return;
                }
            }

            if (amount < 0)
            {
                ErrorTextBlock.Text = "Сумма не может быть отрицательной.";
                return;
            }

            var category = CategoryComboBox.SelectedItem as string ?? "Другое";
            var type = TypeComboBox.SelectedItem as string ?? "Expense";
            var date = DatePicker.SelectedDate ?? DateTime.Today;

            CreatedTransaction = new Transaction
            {
                Title = title,
                Amount = amount,
                Category = category,
                Type = type,
                Date = date
            };

            DialogResult = true;
            Close();
        }
    }
}

