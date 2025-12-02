# MBANK Currency Converter (Console App)

MBANK is a C# console application that demonstrates core programming concepts  
(arrays, strings, OOP, conditionals, loops, file handling) through a real-world task:  
a multi-currency converter with logging, persistent storage, and an admin panel.

The app simulates a banking system where KGS (Kyrgyz Som) is the base currency,  
and all other currencies are stored relative to KGS.

---

## 1. Key Features

- Console-based currency converter with a clear text UI  
- Base currency = KGS (Kyrgyz Som), stored in a separate file  
- Conversion history is logged to a file and persists between runs  
- Supports multiple currencies (USD, EUR, GBP, JPY, RUB, KZT, CNY, TRY, AZN, UZS, etc.)  
- Admin panel with password protection:  
  - Add new currency  
  - Remove existing currency (except KGS)  
- Persistent storage:  
  - Currencies stored in a CSV file  
  - Base currency stored in a separate file  
  - Conversion logs stored in a dedicated file  
- User-friendly navigation:  
  - Clear main menu  
  - Ability to return to the previous screen using `BACK`  
- Colored console output for improved readability  
  - Green – success and application banner  
  - Red – errors  
  - Cyan – menus and tables  
  - Yellow – log records and prompts  
  - DarkGray – extended hints/help

This project is suitable as a course assignment to demonstrate:
- functionality  
- code quality  
- use of core C# concepts  
- mathematical/logical reasoning  
- documentation  
- creativity  

---

## 2. Technologies Used

- Language: C#  
- Runtime: .NET (e.g., .NET 8.0 or similar)  
- Type: Console Application  
- Concepts demonstrated:
  - Object-Oriented Programming  
    - classes: `CurrencyRate`, `CurrencyConverter`, `ConversionLogger`, `Program`  
  - Arrays (initial default rates)  
  - Lists and collections  
  - String operations and parsing  
  - Conditional logic (if/else)  
  - Loops (for, foreach, while)  
  - File handling (File, StreamWriter, Directory)  
  - Mathematical conversion logic using a base currency model  

---

## 3. How the Currency System Works

### 3.1 Base Currency: KGS (Som)

- The base currency is KGS (Kyrgyz Som).  
- Each `CurrencyRate` internally stores:

```csharp
public decimal RateToSom;
