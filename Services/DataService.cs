using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using FinanceManagerApp.Models;

namespace FinanceManagerApp.Services
{
    public class DataService
    {
        private readonly string _filePath;

        public DataService(string filePath)
        {
            _filePath = filePath;
        }

        public List<Transaction> Load()
        {
            if (!File.Exists(_filePath))
            {
                return new List<Transaction>();
            }

            try
            {
                var json = File.ReadAllText(_filePath);
                var items = JsonSerializer.Deserialize<List<Transaction>>(json);
                return items ?? new List<Transaction>();
            }
            catch
            {
                return new List<Transaction>();
            }
        }

        public void Save(List<Transaction> transactions)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(transactions, options);
            File.WriteAllText(_filePath, json);
        }
    }
}

