using System.Globalization;

namespace ExpenseTracker
{
    internal static class InputParser
    {
        public static bool TryReadInt(string? text, out int result)
        {
            return int.TryParse(text, out result);
        }

        public static bool TryReadAmount(string text, out decimal amount)
        {
            string normalized = text.Trim().Replace(',', '.');
            return decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out amount) && amount > 0;
        }

        public static bool TryReadCategory(string text, out Category category)
        {
            return Enum.TryParse(text.Trim(), true, out category)
                   && Enum.IsDefined(category);
        }
    }
}
