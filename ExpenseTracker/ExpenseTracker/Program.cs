using System;
using System.Collections.Generic;
using System.Globalization;

namespace ExpenseTracker
{
    enum Category { Food, Transport, Entertainment, Health, Other };
    internal class Program
    {
        static void Main(string[] args)
        {
            var expenses = new List<Expense>();
            //Category[] allCategories = Enum.GetValues<Category>();

            while (true)
            {
                int choice = ReadIntFromConsole("Меню:\n" +
                "1. Добавить трату\n" +
                "2. Показать все траты\n" +
                "3. Статистика\n" +
                "4. Лаборатория типов\n" +
                "5. Выход\n\n" +
                "Выберите номер действия: ");

                switch (choice)
                {
                    case 1:
                        AddAnExpense(expenses);
                        break;
                    case 2:
                        PrintExpenses(expenses);
                        break;
                    case 3:
                        //вызов ф-ии;
                        break;
                    case 4:
                        //вызов ф-ии;
                        break;
                    case 5:
                        return;
                    default:
                        Console.WriteLine("Введите номер действия от 1 до 5.\n");
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

        private static int ReadIntFromConsole(string promt)
        {
            while (true)
            {
                Console.Write(promt);

                if (TryReadInt(Console.ReadLine(), out int result))
                {
                    return result;
                }

                PrintError("Введите корректное целое число.");
            }
        }
        private static void AddAnExpense(List<Expense> expenses)
        {
            Console.WriteLine("\nДоступные категории:");
            foreach (Category category in Enum.GetValues<Category>())
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

            expenses.Add(new Expense(name, amount, selectedCategory));

            Console.WriteLine("Трата добавлена.\n");
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
        private static void PrintExpenses(List<Expense> expenses)
        {
            int item = 0;
            foreach (Expense expense in expenses)
            {
                item++;
                Console.WriteLine("{0, -10}{1, -25}{2, -10}{3, -20}", "Номер", "Название", "Сумма", "Категория");
                Console.WriteLine("{0, -10}{1, -25}{2, -10}{3, -20}", $"{item}", $"{expense.Name}", $"{expense.Amount}", $"{GetCategoryRuTitle(expense.Category)}");
            }
        }
    }
}