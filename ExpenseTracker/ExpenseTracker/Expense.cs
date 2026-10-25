namespace ExpenseTracker
{
    internal class Expense
    {
        public string Name { get; }
        public decimal Amount { get; }
        public Category Category { get; }
        public Expense(string name, decimal amount, Category category) 
        {
            Name = name;
            Amount = amount;
            Category = category;
        }
    }
}
