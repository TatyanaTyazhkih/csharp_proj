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
                        AddAnExpense(expenses);
                        break;
                    case 2:
                        PrintExpenses(expenses);
                        break;
                    case 3:
                        ExpenseStatistics(expenses);
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
        private static void AddAnExpense(List<Expense> expenses)
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

            expenses.Add(new Expense(name, amount, selectedCategory));

            Console.WriteLine("Трата добавлена.\n");
            Console.WriteLine(new string('-', 65));
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
            if (expenses.Count == 0)
            {
                Console.WriteLine("\nТрат пока нет.\n");
                Console.WriteLine(new string('-', 65));
                return;
            }

            Console.WriteLine("\n{0, -15}{1, -25}{2, -20}{3, -20}", "Номер", "Название", "Сумма", "Категория");

            int item = 0;
            foreach (var expense in expenses)
            {
                item++;
                Console.WriteLine("{0, -15}{1, -25}{2, -20:N2}{3, -20}", item, expense.Name, expense.Amount, GetCategoryRuTitle(expense.Category));
            }

            Console.WriteLine(new string('-', 65));
        }

        private static void ExpenseStatistics(List<Expense> expenses)
        {
            if (expenses.Count == 0)
            {
                Console.WriteLine("\nТрат пока нет.\n");
                Console.WriteLine(new string('-', 65));
                return;
            }

            decimal totalAmount = 0;
            foreach (var expense in expenses)
            {
                totalAmount += expense.Amount;
            }

            decimal averageAmount = totalAmount / expenses.Count;

            int cast = (int)averageAmount;
            decimal rounded = Math.Round(averageAmount);
            decimal roundedAway = Math.Round(averageAmount, MidpointRounding.AwayFromZero);

            Expense maxExpense = expenses[0];
            foreach (var expense in expenses)
            {
                if (expense.Amount > maxExpense.Amount)
                {
                    maxExpense = expense;
                }
            }

            var categorySum = new Dictionary<Category, decimal>();
            foreach (var category in Enum.GetValues<Category>())
            {
                categorySum[category] = 0m;
            }

            foreach (var expense in expenses)
            {
                categorySum[expense.Category] += expense.Amount;
            }

            Console.WriteLine($"\nОбщая сумма: {totalAmount:N2}\n" +
                $"Средняя трата: {averageAmount:N2} (без дробной части: {cast}, банковское округление: {rounded}, обычное округление: {roundedAway})\n" +
                $"Самая большая трата: {maxExpense.Name} --- {maxExpense.Amount:N2}\n");
            
            Console.WriteLine($"\n{"Категория",-20} | {"Сумма",-15}");

            foreach (var pair in categorySum)
            {
                Console.WriteLine($"{GetCategoryRuTitle(pair.Key), -20} | {pair.Value, -15:N2}");
            }

            Console.WriteLine($"\n{"Категория",-20} | {"Процент от суммы",-15}");

            foreach (var pair in categorySum)
            {
                decimal percent = pair.Value / totalAmount * 100;
                Console.WriteLine($"{GetCategoryRuTitle(pair.Key),-20} | {percent,-15:N2}");
            }

            int numberOfExpenses = 0;
            foreach (var expense in expenses)
            {
                numberOfExpenses += (expense.Amount > 1000) ? 1 : 0;
            }

            Console.WriteLine($"\nКоличество трат больше 1000: {numberOfExpenses}\n");

            Console.WriteLine(new string('-', 65));
        }
    }
}