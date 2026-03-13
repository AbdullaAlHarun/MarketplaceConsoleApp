using MarketplaceConsoleApp.Enums;
using MarketplaceConsoleApp.Models;
using MarketplaceConsoleApp.Services;

namespace MarketplaceConsoleApp.UI;

public class ConsoleMenu
{
    private readonly UserService _userService;
    private readonly ListingService _listingService;
    private readonly TransactionService _transactionService;
    private readonly ReviewService _reviewService;

    private User? _currentUser;

    public ConsoleMenu(
        UserService userService,
        ListingService listingService,
        TransactionService transactionService,
        ReviewService reviewService)
    {
        _userService = userService;
        _listingService = listingService;
        _transactionService = transactionService;
        _reviewService = reviewService;
    }

    public void Run()
    {
        SeedData();

        bool isRunning = true;

        while (isRunning)
        {
            Console.Clear();

            if (_currentUser == null)
            {
                isRunning = ShowStartMenu();
            }
            else
            {
                ShowUserMenu();
            }
        }
    }

    private bool ShowStartMenu()
    {
        Console.WriteLine("=== Second-Hand Market ===");
        Console.WriteLine("1. Register");
        Console.WriteLine("2. Log In");
        Console.WriteLine("3. Exit");
        Console.WriteLine();

        int choice = ReadChoice("Select an option: ", 1, 3);
        Console.Clear();

        switch (choice)
        {
            case 1:
                RegisterUser();
                Pause();
                return true;

            case 2:
                LoginUser();
                Pause();
                return true;

            case 3:
                Console.WriteLine("Exiting application...");
                return false;

            default:
                return true;
        }
    }

    private void ShowUserMenu()
    {
        Console.WriteLine($"=== Main Menu ({_currentUser!.Username}) ===");
        Console.WriteLine("1. Create Listing");
        Console.WriteLine("2. Browse Available Listings");
        Console.WriteLine("3. Search Listings");
        Console.WriteLine("4. My Profile");
        Console.WriteLine("5. My Listings");
        Console.WriteLine("6. My Purchases");
        Console.WriteLine("7. My Sales");
        Console.WriteLine("8. Leave Review");
        Console.WriteLine("9. View All Reviews");
        Console.WriteLine("10. Log Out");
        Console.WriteLine();

        int choice = ReadChoice("Select an option: ", 1, 10);
        Console.Clear();

        switch (choice)
        {
            case 1:
                CreateListingForCurrentUser();
                break;
            case 2:
                BrowseAvailableListings();
                break;
            case 3:
                SearchListings();
                break;
            case 4:
                ShowMyProfile();
                break;
            case 5:
                ShowMyListings();
                break;
            case 6:
                ShowMyPurchases();
                break;
            case 7:
                ShowMySales();
                break;
            case 8:
                LeaveReviewAsCurrentUser();
                break;
            case 9:
                ShowReviews();
                break;
            case 10:
                LogoutUser();
                return;
        }

        Pause();
    }

    private void RegisterUser()
    {
        Console.WriteLine("=== Register User ===");

        string username = ReadRequiredText("Enter username: ");
        string password = ReadRequiredText("Enter password: ");

        var existingUser = _userService.GetAllUsers()
            .FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

        if (existingUser != null)
        {
            Console.WriteLine("Username already exists.");
            return;
        }

        var user = _userService.RegisterUser(username, password);
        Console.WriteLine($"User '{user.Username}' registered successfully.");
    }

    private void LoginUser()
    {
        Console.WriteLine("=== Log In ===");

        string username = ReadRequiredText("Enter username: ");
        string password = ReadRequiredText("Enter password: ");

        var user = _userService.LoginUser(username, password);

        if (user == null)
        {
            Console.WriteLine("Invalid username or password.");
            return;
        }

        _currentUser = user;
        Console.WriteLine($"Welcome back, {_currentUser.Username}!");
    }

    private void LogoutUser()
    {
        Console.WriteLine($"{_currentUser!.Username} logged out.");
        _currentUser = null;
    }

    private void CreateListingForCurrentUser()
    {
        Console.WriteLine("=== Create Listing ===");

        string title = ReadRequiredText("Enter title: ");
        string description = ReadRequiredText("Enter description: ");
        Category category = SelectEnumValue<Category>("Select category:");
        ItemCondition condition = SelectEnumValue<ItemCondition>("Select condition:");
        decimal price = ReadDecimal("Enter price: ", 0);

        var listing = _listingService.CreateListing(
            title,
            description,
            category,
            condition,
            price,
            _currentUser!);

        Console.WriteLine("Listing created successfully.");
        Console.WriteLine($"{listing.Title} | {listing.Category} | {listing.Condition} | {listing.Price} NOK");
    }

    private void BrowseAvailableListings()
    {
        var availableListings = _listingService.GetAvailableListings()
            .Where(l => l.Seller != _currentUser)
            .ToList();

        if (!availableListings.Any())
        {
            Console.WriteLine("No available listings found.");
            return;
        }

        var selectedListing = SelectFromList(
            availableListings,
            "=== Available Listings ===",
            l => $"{l.Title} | {l.Category} | {l.Condition} | {l.Price} NOK | Seller: {l.Seller.Username}");

        Console.Clear();
        ShowListingDetails(selectedListing);

        Console.WriteLine();
        Console.WriteLine("1. Buy this item");
        Console.WriteLine("2. Go back");
        Console.WriteLine();

        int choice = ReadChoice("Select an option: ", 1, 2);

        if (choice == 1)
        {
            PurchaseListingAsCurrentUser(selectedListing);
        }
    }

    private void PurchaseListingAsCurrentUser(Listing listing)
    {
        try
        {
            var transaction = _transactionService.PurchaseListing(listing, _currentUser!);

            Console.WriteLine();
            Console.WriteLine("Purchase completed successfully.");
            Console.WriteLine($"Item: {transaction.Listing.Title}");
            Console.WriteLine($"Buyer: {transaction.Buyer.Username}");
            Console.WriteLine($"Seller: {transaction.Seller.Username}");
            Console.WriteLine($"Price: {transaction.Price} NOK");
            Console.WriteLine($"Date: {transaction.TransactionDate:g}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Purchase failed: {ex.Message}");
        }
    }

    private void SearchListings()
    {
        Console.WriteLine("=== Search Listings ===");
        string keyword = ReadRequiredText("Enter keyword: ");

        var results = _listingService.GetAvailableListings()
            .Where(l => l.Seller != _currentUser)
            .Where(l =>
                l.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                l.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (!results.Any())
        {
            Console.WriteLine("No matching listings found.");
            return;
        }

        Console.WriteLine("=== Search Results ===");
        PrintListings(results);
    }

    private void ShowMyProfile()
    {
        Console.WriteLine("=== My Profile ===");
        Console.WriteLine($"Username: {_currentUser!.Username}");
        Console.WriteLine($"Listings: {_currentUser.Listings.Count}");
        Console.WriteLine($"Purchases: {_currentUser.Purchases.Count}");
        Console.WriteLine($"Sales: {_currentUser.Sales.Count}");
        Console.WriteLine($"Average Rating: {_currentUser.AverageRating:F1}");

        var reviews = _reviewService.GetReviewsForUser(_currentUser);

        if (!reviews.Any())
        {
            Console.WriteLine("No reviews received yet.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine("Reviews received:");
        for (int i = 0; i < reviews.Count; i++)
        {
            var review = reviews[i];
            Console.WriteLine($"{i + 1}. Rating: {review.Rating} | Reviewer: {review.Reviewer.Username}");

            if (!string.IsNullOrWhiteSpace(review.Comment))
            {
                Console.WriteLine($"   Comment: {review.Comment}");
            }
        }
    }

    private void ShowMyListings()
    {
        if (!_currentUser!.Listings.Any())
        {
            Console.WriteLine("You have not created any listings.");
            return;
        }

        Console.WriteLine("=== My Listings ===");
        PrintListings(_currentUser.Listings);
    }

    private void ShowMyPurchases()
    {
        if (!_currentUser!.Purchases.Any())
        {
            Console.WriteLine("You have not purchased any items.");
            return;
        }

        Console.WriteLine("=== My Purchases ===");
        for (int i = 0; i < _currentUser.Purchases.Count; i++)
        {
            var purchase = _currentUser.Purchases[i];
            Console.WriteLine(
                $"{i + 1}. {purchase.Listing.Title} | Seller: {purchase.Seller.Username} | Price: {purchase.Price} NOK | Date: {purchase.TransactionDate:g}");
        }
    }

    private void ShowMySales()
    {
        if (!_currentUser!.Sales.Any())
        {
            Console.WriteLine("You have not sold any items.");
            return;
        }

        Console.WriteLine("=== My Sales ===");
        for (int i = 0; i < _currentUser.Sales.Count; i++)
        {
            var sale = _currentUser.Sales[i];
            Console.WriteLine(
                $"{i + 1}. {sale.Listing.Title} | Buyer: {sale.Buyer.Username} | Price: {sale.Price} NOK | Date: {sale.TransactionDate:g}");
        }
    }

    private void LeaveReviewAsCurrentUser()
    {
        var myPurchases = _currentUser!.Purchases;

        if (!myPurchases.Any())
        {
            Console.WriteLine("You have no purchases to review.");
            return;
        }

        var transaction = SelectFromList(
            myPurchases,
            "Select a purchase to review:",
            t => $"{t.Listing.Title} | Seller: {t.Seller.Username} | {t.Price} NOK");

        int rating = ReadChoice("Enter rating (1-6): ", 1, 6);
        Console.Write("Enter comment (optional): ");
        string comment = Console.ReadLine() ?? string.Empty;

        try
        {
            // Only the buyer can leave a review for this transaction.
            var review = _reviewService.AddReview(rating, comment, _currentUser, transaction);
            Console.WriteLine("Review added successfully.");
            Console.WriteLine($"Seller: {review.ReviewedUser.Username} | Rating: {review.Rating}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not add review: {ex.Message}");
        }
    }

    private void ShowReviews()
    {
        var reviews = _reviewService.GetAllReviews();

        if (!reviews.Any())
        {
            Console.WriteLine("No reviews found.");
            return;
        }

        Console.WriteLine("=== Reviews ===");
        for (int i = 0; i < reviews.Count; i++)
        {
            var review = reviews[i];
            Console.WriteLine(
                $"{i + 1}. Rating: {review.Rating} | Reviewer: {review.Reviewer.Username} | Reviewed User: {review.ReviewedUser.Username} | Item: {review.Transaction.Listing.Title}");

            if (!string.IsNullOrWhiteSpace(review.Comment))
            {
                Console.WriteLine($"   Comment: {review.Comment}");
            }
        }
    }

    private void ShowListingDetails(Listing listing)
    {
        Console.WriteLine($"=== {listing.Title} ===");
        Console.WriteLine($"Seller: {listing.Seller.Username}");
        Console.WriteLine($"Category: {listing.Category}");
        Console.WriteLine($"Condition: {listing.Condition}");
        Console.WriteLine($"Price: {listing.Price} NOK");
        Console.WriteLine($"Status: {listing.Status}");
        Console.WriteLine($"Description: {listing.Description}");
    }

    private void PrintListings(List<Listing> listings)
    {
        for (int i = 0; i < listings.Count; i++)
        {
            var listing = listings[i];
            Console.WriteLine(
                $"{i + 1}. {listing.Title} | {listing.Category} | {listing.Condition} | {listing.Price} NOK | Status: {listing.Status} | Seller: {listing.Seller.Username}");
        }
    }

    private T SelectFromList<T>(List<T> items, string heading, Func<T, string> displayText)
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

    private TEnum SelectEnumValue<TEnum>(string heading) where TEnum : struct, Enum
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

    private int ReadChoice(string prompt, int min, int max)
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

    private decimal ReadDecimal(string prompt, decimal minValue)
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

    private string ReadRequiredText(string prompt)
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

    private void Pause()
    {
        Console.WriteLine();
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    private void SeedData()
    {
        var abdulla = _userService.RegisterUser("Abdulla", "1234");
        var mehedi = _userService.RegisterUser("Mehedi", "1234");

        _listingService.CreateListing(
            "iPhone 12",
            "Used phone in good condition",
            Category.Electronics,
            ItemCondition.Good,
            4500,
            abdulla);

        _listingService.CreateListing(
            "Nike Shoes",
            "Running shoes",
            Category.SportsOutdoors,
            ItemCondition.Fair,
            800,
            abdulla);
    }
}