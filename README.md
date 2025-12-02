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
## Main MENU with command 3(supporteed currencies)
<img width="608" height="464" alt="Снимок экрана 2025-12-02 в 23 14 38" src="https://github.com/user-attachments/assets/a6926e98-98fd-4a35-b807-8d24ef66c4b1" />


## Command 1 (converter)
<img width="551" height="313" alt="Снимок экрана 2025-12-02 в 23 17 18" src="https://github.com/user-attachments/assets/201ed303-13d1-42b5-8606-cdbc39ceabbc" />
### choose from currency you want to convert, if ypu choose anpther "ERROR: Unknown currency: USD. Please enter one of the codes from the table."


## if all condition are true, we get answer and add in history.
<img width="570" height="345" alt="Снимок экрана 2025-12-02 в 23 17 49" src="https://github.com/user-attachments/assets/b497ce6a-df6d-4983-8246-6a17a47721ee" />


## it is another example.
<img width="597" height="373" alt="Снимок экрана 2025-12-02 в 23 18 20" src="https://github.com/user-attachments/assets/306b1de3-dc0e-48b2-8cf0-65604ddb0018" />


## In main menu command 2(last records)
<img width="597" height="410" alt="Снимок экрана 2025-12-02 в 23 18 42" src="https://github.com/user-attachments/assets/f1b43e2d-749f-467b-84f5-1ff5ce0652a6" />

## In main menu command 4(admin panel)
<img width="405" height="332" alt="Снимок экрана 2025-12-02 в 23 19 31" src="https://github.com/user-attachments/assets/a5eba4c6-0c1e-4530-8403-d51ca184a25d" />
### we have 4 option


## Admin menu command 1(add new currency)
<img width="311" height="140" alt="Снимок экрана 2025-12-02 в 23 21 48" src="https://github.com/user-attachments/assets/5acd5992-1e66-4c7a-a444-ea1b058e761c" />

### We need write Code, Name, Rate to KGS, if all condition are true currency will be added.

## In Admin menu command 3(List currencies)
<img width="427" height="357" alt="Снимок экрана 2025-12-02 в 23 22 02" src="https://github.com/user-attachments/assets/dececf11-0b0b-4d2f-9043-2309eab07e03" />

## In Admin menu command 2(Delete currency)
<img width="399" height="356" alt="Снимок экрана 2025-12-02 в 23 22 21" src="https://github.com/user-attachments/assets/67583da5-f599-4f36-8662-e1cd00a175f4" />

### Write code and delete currency.
<img width="408" height="424" alt="Снимок экрана 2025-12-02 в 23 22 32" src="https://github.com/user-attachments/assets/8d5f1800-99d1-4fd2-bcb1-5f40cead4e5c" />

### We can see currency deleted.

## Last command in main menu (exit)
<img width="391" height="248" alt="Снимок экрана 2025-12-02 в 23 23 00" src="https://github.com/user-attachments/assets/342d8440-7b08-4284-9dae-0bccbbb2ce5b" />


## We have currencies.csv 
<img width="898" height="303" alt="Снимок экрана 2025-12-02 в 23 24 22" src="https://github.com/user-attachments/assets/dc681093-4494-40b4-8974-95e7714063bb" />

## Also we have histiry of coversions
<img width="846" height="516" alt="Снимок экрана 2025-12-02 в 23 24 31" src="https://github.com/user-attachments/assets/f914aa7e-4c49-4077-a714-deb683827ae9" />
