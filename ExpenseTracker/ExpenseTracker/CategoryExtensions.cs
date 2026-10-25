namespace ExpenseTracker
{
    internal static class CategoryExtensions
    {
        public static string GetCategoryRuTitle(Category c)
        {
            return c switch
            {
                Category.Food => "Еда",
                Category.Transport => "Транспорт",
                Category.Entertainment => "Развлечения",
                Category.Health => "Здоровье",
                Category.Other => "Другое",
                _ => throw new ArgumentOutOfRangeException(nameof(c), c, "Неизвестная категория")
            };
        }
    }
}
