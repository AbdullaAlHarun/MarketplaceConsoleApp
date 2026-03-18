# Marketplace Console App

A console-based second-hand marketplace written in C# using .NET.  
Users can register, create listings, buy items, and leave reviews through a menu-driven interface.

---


## Features

- User registration and login
- Create, edit, and remove listings
- Browse available listings
- Filter listings by category
- Search listings by keyword
- Purchase items (cannot buy your own listing)
- View purchase and sales history
- Leave reviews after transactions
- View user ratings and feedback

---

## Technologies

- C# (.NET 8)
- Console Application
- Spectre.Console
- LINQ
- Object-Oriented Programming (OOP)

---

## Project Structure

- Models – domain classes (User, Listing, Transaction, Review)
- Services – business logic
- Enums – categories, condition, status
- UI – console menus and display logic
- Helpers – input and utility methods

---

## How to Run

1. Clone the repository  
   git clone
   
   ``` https://github.com/AbdullaAlHarun/MarketplaceConsoleApp.git```

2. Navigate to the project folder  

   ``` cd MarketplaceConsoleApp```


3. Run the application  
 
   ``` dotnet run ```

---

## Design Choices

- Separated into Models, Services, UI, and Helpers
- LINQ used for filtering, searching, and calculations
- Enums used for fixed values (category, condition, status)
- Services handle logic, UI handles interaction
- Console UI improved using Spectre.Console

---

## Data Storage

All data is stored in memory during runtime.  
When the application restarts, data is reset and sample data is loaded.

---

## AI Usage

AI (ChatGPT) was used for guidance in:
- structuring the project
- improving code organization
- writing documentation

All code was reviewed and adjusted manually.

---

## Notes

- This project was developed as part of an academic assignment
- Optional database persistence (SQLite) was not implemented

---

## Author

Abdulla Al Harun