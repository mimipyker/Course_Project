using System;

namespace FinanceManagerApp.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Category { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        /// <summary>
        /// "Income" или "Expense"
        /// </summary>
        public string Type { get; set; } = "Expense";
    }
}

