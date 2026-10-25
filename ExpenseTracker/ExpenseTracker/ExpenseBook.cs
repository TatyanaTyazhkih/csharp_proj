using System;
using System.Collections.Generic;

namespace ExpenseTracker
{
    internal class ExpenseBook
    {
        private const string EmptyMessage = "Нет ни одной траты.";
        private readonly List<Expense> expenses = new List<Expense>();
        public int Count => expenses.Count;
        public IReadOnlyList<Expense> Expenses => expenses;
        
        public void Add(Expense expense)
        {
            expenses.Add(expense);
        }

        public decimal GetTotal()
        {
            decimal total = 0;
            foreach (var expense in expenses)
            {
                total += expense.Amount;
            }
            return total;
        }

        public decimal GetAverage()
        {
            if (expenses.Count == 0) throw new InvalidOperationException(EmptyMessage);
            return GetTotal() / expenses.Count;
        }

        public Expense GetMax()
        {
            if (expenses.Count == 0) throw new InvalidOperationException(EmptyMessage);
            var max = expenses[0];

            foreach (var expense in expenses)
            {
                if (expense.Amount > max.Amount)
                {
                    max = expense;
                }
            }
            return max;
        }

        public Dictionary<Category, decimal> GetTotalsByCategory()
        {
            var totalsByCategory = new Dictionary<Category, decimal>();

            foreach (var category in Enum.GetValues<Category>())
            {
                totalsByCategory[category] = 0m;
            }

            foreach (var expense in expenses)
            {
                totalsByCategory[expense.Category] += expense.Amount;
            }
            return totalsByCategory;
        }

        public Dictionary<Category, decimal> GetPercentsByCategory()
        {
            decimal total = GetTotal();
            if (total == 0) throw new InvalidOperationException(EmptyMessage);

            var percents = new Dictionary<Category, decimal>();
            foreach(var pair in GetTotalsByCategory())
            {
                percents[pair.Key] = pair.Value / total * 100;
            }
            return percents;
        }

        public int CountAbove(decimal limit)
        {
            int count = 0;
            foreach (var expense in expenses)
            {
                count += (expense.Amount > limit) ? 1 : 0;
            }
            return count;
        }
    }
}
