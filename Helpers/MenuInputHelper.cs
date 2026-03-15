namespace MarketplaceConsoleApp.Helpers;

public static class MenuInputHelper
{
    public static int ReadChoice(string prompt, int min, int max)
    {
        while (true)
        {
            Console.Write(prompt);

            if (int.TryParse(Console.ReadLine(), out int choice) &&
                choice >= min && choice <= max)
            {
                return choice;
            }

            Console.WriteLine($"Invalid input. Enter a number between {min} and {max}.");
        }
    }

    public static decimal ReadDecimal(string prompt, decimal minValue)
    {
        while (true)
        {
            Console.Write(prompt);

            if (decimal.TryParse(Console.ReadLine(), out decimal value) && value >= minValue)
            {
                return value;
            }

            Console.WriteLine($"Invalid input. Enter a number greater than or equal to {minValue}.");
        }
    }

    public static string ReadRequiredText(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(input))
            {
                return input.Trim();
            }

            Console.WriteLine("Input cannot be empty.");
        }
    }

    public static void Pause()
    {
        Console.WriteLine();
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    public static TEnum SelectEnumValue<TEnum>(string heading) where TEnum : struct, Enum
    {
        var values = Enum.GetValues<TEnum>().ToList();

        Console.WriteLine(heading);
        for (int i = 0; i < values.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {values[i]}");
        }

        Console.WriteLine();
        int choice = ReadChoice("Choose an option: ", 1, values.Count);
        return values[choice - 1];
    }

    public static T SelectFromList<T>(List<T> items, string heading, Func<T, string> displayText)
    {
        Console.WriteLine(heading);

        for (int i = 0; i < items.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {displayText(items[i])}");
        }

        Console.WriteLine();
        int choice = ReadChoice("Choose an option: ", 1, items.Count);
        return items[choice - 1];
    }
}