using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace MBankCurrencyConverter
{
    public class CurrencyRate
    {
        public string Code { get; }
        public string Name { get; }

        public decimal RateToSom { get; }

        public CurrencyRate(string code, string name, decimal rateToSom)
        {
            Code = code;
            Name = name;
            RateToSom = rateToSom;
        }

        public override string ToString()
        {
            return $"{Code,-6} │ {Name,-22} │ 1 {Code} = {RateToSom} KGS";
        }

        public string ToCsv()
        {
            return $"{Code};{Name};{RateToSom}";
        }

        public static bool TryParseCsv(string line, out CurrencyRate rate)
        {
            rate = null;
            if (string.IsNullOrWhiteSpace(line)) return false;

            var parts = line.Split(';');
            if (parts.Length != 3) return false;

            var code = parts[0].Trim();
            var name = parts[1].Trim();
            if (!decimal.TryParse(parts[2].Trim(), out var r)) return false;

            rate = new CurrencyRate(code, name, r);
            return true;
        }
    }

    public class CurrencyConverter
    {
        private readonly List<CurrencyRate> _rates;

        public CurrencyConverter(List<CurrencyRate> rates)
        {
            _rates = rates;
        }

        public IReadOnlyList<CurrencyRate> Rates => _rates;

        public bool TryConvert(
            string fromCode,
            string toCode,
            decimal amount,
            out decimal result,
            out string errorMessage,
            out CurrencyRate fromRate,
            out CurrencyRate toRate)
        {
            result = 0;
            errorMessage = string.Empty;
            fromRate = null;
            toRate = null;

            if (amount < 0)
            {
                errorMessage = "Amount must be non-negative.";
                return false;
            }

            fromRate = FindRateByCode(fromCode);
            toRate = FindRateByCode(toCode);

            if (fromRate == null)
            {
                errorMessage = $"Unknown currency code: {fromCode}";
                return false;
            }

            if (toRate == null)
            {
                errorMessage = $"Unknown currency code: {toCode}";
                return false;
            }


            decimal amountInSom = amount * fromRate.RateToSom;
            result = amountInSom / toRate.RateToSom;
            return true;
        }

        public CurrencyRate FindRateByCode(string code)
        {
            return _rates.FirstOrDefault(r =>
                string.Equals(r.Code, code, StringComparison.OrdinalIgnoreCase));
        }

        public bool IsSupportedCode(string code)
        {
            return FindRateByCode(code) != null;
        }

        public void PrintTable()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("CODE   │ NAME                   │ RATE (to KGS)");
            Console.WriteLine("────────────────────────────────────────────────");
            Console.ResetColor();

            foreach (var r in _rates)
                Console.WriteLine(r);
        }

        public bool AddCurrency(CurrencyRate rate, out string error)
        {
            error = string.Empty;
            if (IsSupportedCode(rate.Code))
            {
                error = $"Currency {rate.Code} already exists.";
                return false;
            }

            if (rate.RateToSom <= 0)
            {
                error = "Rate to KGS must be positive.";
                return false;
            }

            _rates.Add(rate);
            return true;
        }

        public bool RemoveCurrency(string code, out string error)
        {
            error = string.Empty;

            var rate = FindRateByCode(code);
            if (rate == null)
            {
                error = $"Currency {code} not found.";
                return false;
            }

            _rates.Remove(rate);
            return true;
        }
    }

    public class ConversionLogger
    {
        public string LogFilePath { get; }

        public ConversionLogger(string logFilePath)
        {
            LogFilePath = logFilePath;
        }

        public void LogConversion(
            DateTime time,
            decimal amount,
            CurrencyRate fromRate,
            CurrencyRate toRate,
            decimal result)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(LogFilePath)!);

            string line =
                $"{time:yyyy-MM-dd HH:mm:ss} | " +
                $"{amount} {fromRate.Code} -> {result:F2} {toRate.Code} | " +
                $"Rates to KGS: {fromRate.RateToSom} / {toRate.RateToSom}";

            using (var writer = new StreamWriter(LogFilePath, append: true, Encoding.UTF8))
            {
                writer.WriteLine(line);
            }
        }

        public void PrintLastLines(int maxLines)
        {
            if (!File.Exists(LogFilePath))
            {
                Console.WriteLine("Log file does not exist yet.");
                return;
            }

            string[] allLines = File.ReadAllLines(LogFilePath, Encoding.UTF8);
            if (allLines.Length == 0)
            {
                Console.WriteLine("Log file is empty.");
                return;
            }

            int start = Math.Max(0, allLines.Length - maxLines);

            for (int i = start; i < allLines.Length; i++)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(allLines[i]);
            }
            Console.ResetColor();
        }
    }

    internal class Program
    {
        private const string AdminPassword = "MBANK2025";
        private const string BaseCurrencyCode = "KGS";
        private const string BaseCurrencyName = "Kyrgyz Som";

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string dataDir = Path.Combine(baseDir, "data");
            string logsDir = Path.Combine(baseDir, "logs");

            Directory.CreateDirectory(dataDir);
            Directory.CreateDirectory(logsDir);

            string baseCurrencyFilePath = Path.Combine(dataDir, "base_currency.txt");
            string currencyFilePath = Path.Combine(dataDir, "currencies.csv");
            string logFilePath = Path.Combine(logsDir, "mbank_conversions.log");

            var baseCurrency = InitBaseCurrency(baseCurrencyFilePath);

            if (!File.Exists(currencyFilePath))
            {
                var defaultRates = new[]
                {
                    new CurrencyRate("USD", "US Dollar",         89m),
                    new CurrencyRate("EUR", "Euro",              96m),
                    new CurrencyRate("GBP", "British Pound",     112m),
                    new CurrencyRate("JPY", "Japanese Yen",      0.60m),
                    new CurrencyRate("RUB", "Russian Ruble",     1.0m),
                    new CurrencyRate("KZT", "Kazakh Tenge",      0.20m),
                    new CurrencyRate("CNY", "Chinese Yuan",      12m),
                    new CurrencyRate("TRY", "Turkish Lira",      3.5m),
                    new CurrencyRate("AZN", "Azerbaijani Manat", 52m),
                    new CurrencyRate("UZS", "Uzbekistani Som",   0.0075m)
                };

                File.WriteAllLines(currencyFilePath,
                    defaultRates.Select(r => r.ToCsv()),
                    Encoding.UTF8);
            }

            var ratesList = LoadCurrencies(currencyFilePath);
            ratesList.Insert(0, baseCurrency);

            var converter = new CurrencyConverter(ratesList);
            var logger = new ConversionLogger(logFilePath);

            RunMenu(converter, logger, currencyFilePath);
        }

        private static CurrencyRate InitBaseCurrency(string basePath)
        {
            var baseCurrency = new CurrencyRate(BaseCurrencyCode, BaseCurrencyName, 1.0m);
            Directory.CreateDirectory(Path.GetDirectoryName(basePath)!);
            File.WriteAllText(basePath, baseCurrency.ToCsv(), Encoding.UTF8);
            return baseCurrency;
        }

        private static List<CurrencyRate> LoadCurrencies(string path)
        {
            var list = new List<CurrencyRate>();
            foreach (var line in File.ReadAllLines(path, Encoding.UTF8))
            {
                if (CurrencyRate.TryParseCsv(line, out var rate))
                    list.Add(rate);
            }
            return list;
        }

        private static void SaveCurrencies(string path, IEnumerable<CurrencyRate> rates)
        {
            var lines = rates
                .Where(r => !string.Equals(r.Code, BaseCurrencyCode, StringComparison.OrdinalIgnoreCase))
                .Select(r => r.ToCsv());

            File.WriteAllLines(path, lines, Encoding.UTF8);
        }

        private static void RunMenu(CurrencyConverter converter, ConversionLogger logger, string currencyFilePath)
        {
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("███╗   ███╗██████╗  █████╗ ███╗   ██╗██╗  ██╗");
                Console.WriteLine("████╗ ████║██╔══██╗██╔══██╗████╗  ██║██║ ██╔╝");
                Console.WriteLine("██╔████╔██║██████╔╝███████║██╔██╗ ██║█████╔╝ ");
                Console.WriteLine("██║╚██╔╝██║██╔══██╗██╔══██║██║╚██╗██║██╔═██╗ ");
                Console.WriteLine("██║ ╚═╝ ██║██████╔╝██║  ██║██║ ╚████║██║  ██╗");
                Console.WriteLine("╚═╝     ╚═╝╚═════╝ ╚═╝  ╚═╝╚═╝  ╚═══╝╚═╝  ╚═╝");
                Console.ResetColor();

                Console.WriteLine("\n===== MBANK Currency Converter (Base: KGS) =====");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("1. Convert currency");
                Console.WriteLine("2. Show last log records");
                Console.WriteLine("3. Show supported currencies");
                Console.WriteLine("4. Manage currencies (admin)");
                Console.WriteLine("5. Exit");
                Console.ResetColor();
                Console.Write("\nChoose option (1-5): ");

                string choice = Console.ReadLine() ?? "";

                if (choice == "1")
                    HandleConversion(converter, logger);
                else if (choice == "2")
                    HandleShowLog(logger);
                else if (choice == "3")
                    ShowCurrencies(converter);
                else if (choice == "4")
                    HandleAdmin(converter, currencyFilePath);
                else if (choice == "5")
                {
                    Console.Clear();
                    Console.WriteLine("Thank you for using MBANK!");
                    break;
                }
                else
                {
                    PrintError("Invalid menu option. Please enter 1–5.");
                }
            }
        }

        private static void ShowCurrencies(CurrencyConverter converter)
        {
            while (true)
            {
                Console.Clear();
                converter.PrintTable();
                Console.WriteLine("\nType BACK to return to main menu or press ENTER to refresh list:");
                string input = (Console.ReadLine() ?? "").Trim().ToUpperInvariant();

                if (input == "BACK")
                    return;
            }
        }

        private static void HandleConversion(CurrencyConverter converter, ConversionLogger logger)
        {
            while (true)
            {
                Console.Clear();
                converter.PrintTable();
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("Type BACK at any prompt to return to main menu.");
                Console.ResetColor();
                Console.WriteLine();

                // From
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("From currency: ");
                string fromCode = (Console.ReadLine() ?? "").Trim().ToUpperInvariant();
                Console.ResetColor();

                if (fromCode == "BACK")
                    return;

                if (!converter.IsSupportedCode(fromCode))
                {
                    PrintError($"Unknown currency: {fromCode}. Please enter one of the codes from the table.");
                    continue;
                }

                // To
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("To currency:   ");
                string toCode = (Console.ReadLine() ?? "").Trim().ToUpperInvariant();
                Console.ResetColor();

                if (toCode == "BACK")
                    return;

                if (!converter.IsSupportedCode(toCode))
                {
                    PrintError($"Unknown currency: {toCode}. Please enter one of the codes from the table.");
                    continue;
                }

                // Amount
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Amount:        ");
                string amountStr = Console.ReadLine() ?? "";
                Console.ResetColor();

                if (amountStr.Trim().ToUpperInvariant() == "BACK")
                    return;

                if (!decimal.TryParse(amountStr, out decimal amount))
                {
                    PrintError("Amount must be a valid number.");
                    continue;
                }

                bool ok = converter.TryConvert(
                    fromCode, toCode, amount,
                    out decimal result, out string error,
                    out CurrencyRate fromRate, out CurrencyRate toRate);

                Console.Clear();

                if (!ok)
                {
                    PrintError(error);
                    continue;
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n✔ {amount} {fromRate.Code} = {result:F2} {toRate.Code}");
                Console.ResetColor();

                logger.LogConversion(DateTime.Now, amount, fromRate, toRate, result);

                Console.WriteLine("\nPress ENTER to perform another conversion or type BACK to return to main menu:");
                string cont = (Console.ReadLine() ?? "").Trim().ToUpperInvariant();
                if (cont == "BACK")
                    return;
            }
        }

        private static void HandleShowLog(ConversionLogger logger)
        {
            while (true)
            {
                Console.Clear();
                Console.Write("How many last records? (or type BACK): ");
                string nStr = Console.ReadLine() ?? "";

                if (nStr.Trim().ToUpperInvariant() == "BACK")
                    return;

                if (!int.TryParse(nStr, out int n) || n <= 0)
                {
                    PrintError("Please enter a positive integer or BACK.");
                    continue;
                }

                Console.Clear();
                logger.PrintLastLines(n);
                Console.WriteLine("\nPress ENTER to refresh log or type BACK to return to main menu:");
                string cont = (Console.ReadLine() ?? "").Trim().ToUpperInvariant();
                if (cont == "BACK")
                    return;
            }
        }

        private static void HandleAdmin(CurrencyConverter converter, string currencyFilePath)
        {
            Console.Clear();
            Console.Write("Enter admin password (or BACK): ");
            string pwd = Console.ReadLine() ?? "";

            if (pwd.Trim().ToUpperInvariant() == "BACK")
                return;

            if (pwd != AdminPassword)
            {
                PrintError("Wrong password.");
                return;
            }

            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("===== MBANK Admin: Manage Currencies =====");
                Console.ResetColor();
                Console.WriteLine("1. Add currency");
                Console.WriteLine("2. Remove currency");
                Console.WriteLine("3. List currencies");
                Console.WriteLine("4. Back to main menu");
                Console.Write("\nChoose option (1-4): ");

                string choice = Console.ReadLine() ?? "";

                if (choice == "1")
                {
                    AdminAddCurrency(converter, currencyFilePath);
                }
                else if (choice == "2")
                {
                    AdminRemoveCurrency(converter, currencyFilePath);
                }
                else if (choice == "3")
                {
                    Console.Clear();
                    converter.PrintTable();
                    Console.WriteLine("\nPress ENTER to continue...");
                    Console.ReadLine();
                }
                else if (choice == "4")
                {
                    return;
                }
                else
                {
                    PrintError("Invalid option. Please enter 1–4.");
                }
            }
        }

        private static void AdminAddCurrency(CurrencyConverter converter, string currencyFilePath)
        {
            Console.Clear();
            Console.WriteLine("=== Add new currency ===");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("Type BACK at any prompt to cancel.");
            Console.ResetColor();

            Console.Write("\nCode (e.g. CHF): ");
            string code = (Console.ReadLine() ?? "").Trim().ToUpperInvariant();
            if (code == "BACK") return;

            if (string.IsNullOrWhiteSpace(code) || code.Length < 2 || code.Length > 6)
            {
                PrintError("Invalid code length. Use 2–6 letters.");
                return;
            }

            if (!code.All(char.IsLetter))
            {
                PrintError("Code must contain only letters.");
                return;
            }

            if (string.Equals(code, BaseCurrencyCode, StringComparison.OrdinalIgnoreCase))
            {
                PrintError($"{BaseCurrencyCode} is base currency and already exists.");
                return;
            }

            if (converter.IsSupportedCode(code))
            {
                PrintError($"Currency {code} already exists.");
                return;
            }

            Console.Write("Name (e.g. Swiss Franc): ");
            string name = (Console.ReadLine() ?? "").Trim();
            if (name.ToUpperInvariant() == "BACK") return;

            if (string.IsNullOrWhiteSpace(name))
            {
                PrintError("Name cannot be empty.");
                return;
            }

            Console.Write("Rate to KGS (1 CODE = ? KGS): ");
            string rateStr = Console.ReadLine() ?? "";
            if (rateStr.Trim().ToUpperInvariant() == "BACK") return;

            if (!decimal.TryParse(rateStr, out decimal rate) || rate <= 0)
            {
                PrintError("Rate must be a positive number.");
                return;
            }

            var newRate = new CurrencyRate(code, name, rate);
            if (!converter.AddCurrency(newRate, out string error))
            {
                PrintError(error);
                return;
            }

            SaveCurrencies(currencyFilePath, converter.Rates);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n✔ Currency {code} added successfully.");
            Console.ResetColor();
            Console.WriteLine("\nPress ENTER...");
            Console.ReadLine();
        }

        private static void AdminRemoveCurrency(CurrencyConverter converter, string currencyFilePath)
        {
            Console.Clear();
            Console.WriteLine("=== Remove currency ===");
            converter.PrintTable();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("\nType BACK to cancel.");
            Console.ResetColor();

            Console.Write("\nCode to remove: ");
            string code = (Console.ReadLine() ?? "").Trim().ToUpperInvariant();
            if (code == "BACK") return;

            if (string.Equals(code, BaseCurrencyCode, StringComparison.OrdinalIgnoreCase))
            {
                PrintError($"{BaseCurrencyCode} is base currency and cannot be removed.");
                return;
            }

            if (!converter.RemoveCurrency(code, out string error))
            {
                PrintError(error);
                return;
            }

            SaveCurrencies(currencyFilePath, converter.Rates);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n✔ Currency {code} removed successfully.");
            Console.ResetColor();
            Console.WriteLine("\nPress ENTER...");
            Console.ReadLine();
        }

        private static void PrintError(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ERROR: " + msg);
            Console.ResetColor();
            Console.WriteLine("\nPress ENTER to continue...");
            Console.ReadLine();
        }
    }
}
