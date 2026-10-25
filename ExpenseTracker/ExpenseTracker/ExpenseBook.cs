using System;
using System.Collections.Generic;

namespace ExpenseTracker
{
    internal class ExpenseBook
    {
        private readonly List<Expense> expenses = new List<Expense>();
        public int Count => expenses.Count;
        public IReadOnlyList<Expense> Expenses => expenses;
        
        public void Add(Expense expense)
        {
            expenses.Add(expense);
        }
    }
}
