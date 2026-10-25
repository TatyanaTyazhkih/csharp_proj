namespace ExpenseTracker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var book = new ExpenseBook();

            while (true)
            {
                int choice = ConsoleUi.ReadIntFromConsole("Меню:\n" +
                "1. Добавить трату\n" +
                "2. Показать все траты\n" +
                "3. Статистика\n" +
                "4. Выход\n\n" +
                "Выберите номер действия: ");

                switch (choice)
                {
                    case 1:
                        ConsoleUi.AddAnExpense(book);
                        break;
                    case 2:
                        ConsoleUi.PrintExpenses(book);
                        break;
                    case 3:
                        ConsoleUi.ExpenseStatistics(book);
                        break;
                    case 4:
                        return;
                    default:
                        ConsoleUi.PrintError("Введите номер действия от 1 до 4.");
                        break;
                }
            }
        }
    }
}