using System;
using System.Collections.Generic;

namespace ExpenseTracker
{
    internal static class ConsoleUi
    {
        private const decimal LargeExpenseLimit = 1000m;
        private const int LineWidth = 80;

        public static void PrintInfo(string message)
        {
            Console.WriteLine($"\n{message}\n");
        }

        public static void PrintError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Ошибка! {message}\n");
            Console.ResetColor();
        }

        public static void PrintSeparator()
        {
            Console.WriteLine(new string('-', LineWidth));
        }

        public static int ReadIntFromConsole(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);

                if (InputParser.TryReadInt(Console.ReadLine(), out int result))
                {
                    return result;
                }

                PrintError("Введите корректное целое число.");
            }
        }

        public static void PrintExpenses(ExpenseBook book)
        {
            if (book.Count == 0)
            {
                PrintInfo("Трат пока нет.");
                PrintSeparator();
                return;
            }

            Console.WriteLine("\n{0, -15}{1, -25}{2, -20}{3, -20}", "Номер", "Название", "Сумма", "Категория");

            int item = 0;
            foreach (var expense in book.Expenses)
            {
                item++;
                Console.WriteLine("{0, -15}{1, -25}{2, -20:N2}{3, -20}", item, expense.Name, expense.Amount, CategoryExtensions.GetCategoryRuTitle(expense.Category));
            }

            PrintSeparator();
        }

        public static void AddAnExpense(ExpenseBook book)
        {
            Console.WriteLine("\nДоступные категории: ");
            foreach (var category in Enum.GetValues<Category>())
            {
                Console.WriteLine($"{category} = {(int)category}");
            }

            Console.Write("\nВведите трату [название;стоимость;категория]: ");
            string? input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                PrintError("Пустой ввод.");
                return;
            }

            string[] parts = input.Split(';');
            if (parts.Length != 3)
            {
                PrintError("Нужно ввести ровно три части через ';'.");
                return;
            }

            string name = parts[0].Trim();
            if (name == "")
            {
                PrintError("Название не может быть пустым.");
                return;
            }

            if (!InputParser.TryReadAmount(parts[1], out decimal amount))
            {
                PrintError("Сумма должна быть числом больше нуля.");
                return;
            }

            if (!InputParser.TryReadCategory(parts[2], out Category selectedCategory))
            {
                PrintError("Такой категории нет.");
                return;
            }

            book.Add(new Expense(name, amount, selectedCategory));

            PrintInfo("Трата добавлена.");
            PrintSeparator();
        }

        public static void ExpenseStatistics(ExpenseBook book)
        {
            if (book.Count == 0)
            {
                PrintInfo("Трат пока нет.");
                PrintSeparator();
                return;
            }

            decimal totalAmount = book.GetTotal();
            decimal averageAmount = book.GetAverage();
            Expense maxExpense = book.GetMax();
            Dictionary<Category, decimal> categorySum = book.GetTotalsByCategory();
            Dictionary<Category, decimal> percents = book.GetPercentsByCategory();
            int numberOfExpenses = book.CountAbove(LargeExpenseLimit);

            PrintInfo($"Общая сумма: {totalAmount:N2}\n" +
                $"Средняя трата: {averageAmount:N2}\n" +
                $"Самая большая трата: {maxExpense.Name} --- {maxExpense.Amount:N2}");

            Console.WriteLine($"{"Категория",-20} | {"Сумма",-15}");

            foreach (var pair in categorySum)
            {
                Console.WriteLine($"{CategoryExtensions.GetCategoryRuTitle(pair.Key),-20} | {pair.Value,-15:N2}");
            }

            Console.WriteLine($"\n{"Категория",-20} | {"Процент от суммы",-15}");

            foreach (var pair in percents)
            {
                Console.WriteLine($"{CategoryExtensions.GetCategoryRuTitle(pair.Key),-20} | {pair.Value,-15:N3}");
            }

            PrintInfo($"Количество трат больше {LargeExpenseLimit:N2}: {numberOfExpenses}");

            PrintSeparator();
        }
    }
}
