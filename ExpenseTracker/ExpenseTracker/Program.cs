using System;
using System.Collections.Generic;
using System.Globalization;

namespace ExpenseTracker
{
    enum Category { Food, Transport, Entertainment, Health, Other };
    internal class Program
    {
        private const decimal LargeExpenseLimit = 1000m;
        private const int LineWidth = 80;

        static void Main(string[] args)
        {
            var book = new ExpenseBook();

            while (true)
            {
                int choice = ReadIntFromConsole("Меню:\n" +
                "1. Добавить трату\n" +
                "2. Показать все траты\n" +
                "3. Статистика\n" +
                "4. Выход\n\n" +
                "Выберите номер действия: ");

                switch (choice)
                {
                    case 1:
                        AddAnExpense(book);
                        break;
                    case 2:
                        PrintExpenses(book);
                        break;
                    case 3:
                        ExpenseStatistics(book);
                        break;
                    case 4:
                        return;
                    default:
                        Console.WriteLine("Введите номер действия от 1 до 4.\n");
                        break;
                }
            }

        }

        private static void PrintError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Ошибка! {message}\n");
            Console.ResetColor();
        }

        private static bool TryReadInt(string? text, out int result)
        {
            return int.TryParse(text, out result);
        }

        private static bool TryReadAmount(string text, out decimal amount)
        {
            string normalized = text.Trim().Replace(',', '.');
            return decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out amount) && amount > 0;
        }

        private static bool TryReadCategory(string text, out Category category)
        {
            return Enum.TryParse(text.Trim(), true, out category)
                   && Enum.IsDefined(category);
        }

        private static int ReadIntFromConsole(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);

                if (TryReadInt(Console.ReadLine(), out int result))
                {
                    return result;
                }

                PrintError("Введите корректное целое число.");
            }
        }

        private static void AddAnExpense(ExpenseBook book)
        {
            Console.WriteLine("\nДоступные категории:");
            foreach (var category in Enum.GetValues<Category>())
            {
                Console.WriteLine($"{category} = {(int)category}");
            }
            Console.WriteLine();

            Console.Write("Введите трату [название;стоимость;категория]: ");
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

            if (!TryReadAmount(parts[1], out decimal amount))
            {
                PrintError("Сумма должна быть числом больше нуля.");
                return;
            }

            if (!TryReadCategory(parts[2], out Category selectedCategory))
            {
                PrintError("Такой категории нет.");
                return;
            }

            book.Add(new Expense(name, amount, selectedCategory));

            Console.WriteLine("Трата добавлена.\n");
            Console.WriteLine(new string('-', LineWidth));
        }

        private static string GetCategoryRuTitle (Category c)
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

        private static void PrintExpenses(ExpenseBook book)
        {
            if (book.Count == 0)
            {
                Console.WriteLine("\nТрат пока нет.\n");
                Console.WriteLine(new string('-', LineWidth));
                return;
            }

            Console.WriteLine("\n{0, -15}{1, -25}{2, -20}{3, -20}", "Номер", "Название", "Сумма", "Категория");

            int item = 0;
            foreach (var expense in book.Expenses)
            {
                item++;
                Console.WriteLine("{0, -15}{1, -25}{2, -20:N2}{3, -20}", item, expense.Name, expense.Amount, GetCategoryRuTitle(expense.Category));
            }

            Console.WriteLine(new string('-', LineWidth));
        }

        private static void ExpenseStatistics(ExpenseBook book)
        {
            if (book.Count == 0)
            {
                Console.WriteLine("\nТрат пока нет.\n");
                Console.WriteLine(new string('-', LineWidth));
                return;
            }

            decimal totalAmount = book.GetTotal();
            decimal averageAmount = book.GetAverage();
            Expense maxExpense = book.GetMax();
            Dictionary<Category, decimal> categorySum = book.GetTotalsByCategory();
            Dictionary<Category, decimal> percents = book.GetPercentsByCategory();
            int numberOfExpenses = book.CountAbove(LargeExpenseLimit);

            Console.WriteLine($"\nОбщая сумма: {totalAmount:N2}\n" +
                $"Средняя трата: {averageAmount:N2}\n" +
                $"Самая большая трата: {maxExpense.Name} --- {maxExpense.Amount:N2}\n");
            
            Console.WriteLine($"\n{"Категория",-20} | {"Сумма",-15}");

            foreach (var pair in categorySum)
            {
                Console.WriteLine($"{GetCategoryRuTitle(pair.Key), -20} | {pair.Value, -15:N2}");
            }

            Console.WriteLine($"\n{"Категория",-20} | {"Процент от суммы",-15}");

            foreach (var pair in percents)
            {
                Console.WriteLine($"{GetCategoryRuTitle(pair.Key),-20} | {pair.Value,-15:N3}");
            }

            Console.WriteLine($"\nКоличество трат больше {LargeExpenseLimit:N2}: {numberOfExpenses}\n");

            Console.WriteLine(new string('-', LineWidth));
        }
    }
}