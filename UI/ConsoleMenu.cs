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

        int choice = MenuInputHelper.ReadChoice("Select an option: ", 1, 3);
        Console.Clear();

        switch (choice)
        {
            case 1:
                RegisterUser();
                MenuInputHelper.Pause();
                return true;

            case 2:
                LoginUser();
                MenuInputHelper.Pause();
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
        Console.WriteLine("3. Filter Listings by Category");
        Console.WriteLine("4. Search Listings");
        Console.WriteLine("5. My Profile");
        Console.WriteLine("6. Manage My Listings");
        Console.WriteLine("7. My Purchases");
        Console.WriteLine("8. My Sales");
        Console.WriteLine("9. Leave Review");
        Console.WriteLine("10. View All Reviews");
        Console.WriteLine("11. Log Out");
        Console.WriteLine();

        int choice = MenuInputHelper.ReadChoice("Select an option: ", 1, 11);
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
                FilterListingsByCategory();
                break;
            case 4:
                SearchListings();
                break;
            case 5:
                ShowMyProfile();
                break;
            case 6:
                ManageMyListings();
                break;
            case 7:
                ShowMyPurchases();
                break;
            case 8:
                ShowMySales();
                break;
            case 9:
                LeaveReviewAsCurrentUser();
                break;
            case 10:
                ShowReviews();
                break;
            case 11:
                LogoutUser();
                return;
        }

        MenuInputHelper.Pause();
    }

    private void RegisterUser()
    {
        Console.WriteLine("=== Register User ===");

        string username = MenuInputHelper.ReadRequiredText("Enter username: ");
        string password = MenuInputHelper.ReadRequiredText("Enter password: ");

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

        string username = MenuInputHelper.ReadRequiredText("Enter username: ");
        string password = MenuInputHelper.ReadRequiredText("Enter password: ");

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

        string title = MenuInputHelper.ReadRequiredText("Enter title: ");
        string description = MenuInputHelper.ReadRequiredText("Enter description: ");
        Category category = MenuInputHelper.SelectEnumValue<Category>("Select category:");
        ItemCondition condition = MenuInputHelper.SelectEnumValue<ItemCondition>("Select condition:");
        decimal price = MenuInputHelper.ReadDecimal("Enter price: ", 0);

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
        // Hide the current user's own listings from the buy flow.
        var availableListings = _listingService.GetAvailableListings()
            .Where(l => l.Seller != _currentUser)
            .ToList();

        if (!availableListings.Any())
        {
            Console.WriteLine("No available listings found.");
            return;
        }

        var selectedListing = MenuInputHelper.SelectFromList(
            availableListings,
            "=== Available Listings ===",
            l => $"{l.Title} | {l.Category} | {l.Condition} | {l.Price} NOK | Seller: {l.Seller.Username}");

        Console.Clear();
        ListingViewHelper.ShowListingDetails(selectedListing);

        Console.WriteLine();
        Console.WriteLine("1. Buy this item");
        Console.WriteLine("2. Go back");
        Console.WriteLine();

        int choice = MenuInputHelper.ReadChoice("Select an option: ", 1, 2);

        if (choice == 1)
        {
            PurchaseListingAsCurrentUser(selectedListing);
        }
    }

    private void FilterListingsByCategory()
    {
        Console.WriteLine("=== Filter Listings by Category ===");

        Category selectedCategory = MenuInputHelper.SelectEnumValue<Category>("Select category:");

        var filteredListings = _listingService.GetAvailableListings()
            .Where(l => l.Seller != _currentUser)
            .Where(l => l.Category == selectedCategory)
            .ToList();

        if (!filteredListings.Any())
        {
            Console.WriteLine("No listings found in that category.");
            return;
        }

        Console.WriteLine($"=== {selectedCategory} Listings ===");
        ListingViewHelper.PrintListings(filteredListings);
    }

    private void SearchListings()
    {
        Console.WriteLine("=== Search Listings ===");
        string keyword = MenuInputHelper.ReadRequiredText("Enter keyword: ");

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
        ListingViewHelper.PrintListings(results);
    }

    private void ManageMyListings()
    {
        if (!_currentUser!.Listings.Any())
        {
            Console.WriteLine("You have not created any listings.");
            return;
        }

        var selectedListing = MenuInputHelper.SelectFromList(
            _currentUser.Listings,
            "=== My Listings ===",
            l => $"{l.Title} | {l.Category} | {l.Condition} | {l.Price} NOK | Status: {l.Status}");

        Console.Clear();
        ListingViewHelper.ShowListingDetails(selectedListing);

        Console.WriteLine();
        Console.WriteLine("1. Edit Listing");
        Console.WriteLine("2. Remove Listing");
        Console.WriteLine("3. Go Back");
        Console.WriteLine();

        int choice = MenuInputHelper.ReadChoice("Select an option: ", 1, 3);

        switch (choice)
        {
            case 1:
                EditListing(selectedListing);
                break;
            case 2:
                RemoveListing(selectedListing);
                break;
            case 3:
                return;
        }
    }

    private void EditListing(Listing listing)
    {
        Console.WriteLine("=== Edit Listing ===");

        string title = MenuInputHelper.ReadRequiredText("Enter new title: ");
        string description = MenuInputHelper.ReadRequiredText("Enter new description: ");
        Category category = MenuInputHelper.SelectEnumValue<Category>("Select new category:");
        ItemCondition condition = MenuInputHelper.SelectEnumValue<ItemCondition>("Select new condition:");
        decimal price = MenuInputHelper.ReadDecimal("Enter new price: ", 0);

        try
        {
            _listingService.UpdateListing(
                listing,
                _currentUser!,
                title,
                description,
                category,
                condition,
                price);

            Console.WriteLine("Listing updated successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not update listing: {ex.Message}");
        }
    }

    private void RemoveListing(Listing listing)
    {
        Console.Write("Are you sure you want to remove this listing? (y/n): ");
        string? input = Console.ReadLine();

        if (input == null || !input.Equals("y", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Remove cancelled.");
            return;
        }

        try
        {
            _listingService.RemoveListing(listing, _currentUser!);
            Console.WriteLine("Listing removed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not remove listing: {ex.Message}");
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

        var transaction = MenuInputHelper.SelectFromList(
            myPurchases,
            "Select a purchase to review:",
            t => $"{t.Listing.Title} | Seller: {t.Seller.Username} | {t.Price} NOK");

        int rating = MenuInputHelper.ReadChoice("Enter rating (1-6): ", 1, 6);
        Console.Write("Enter comment (optional): ");
        string comment = Console.ReadLine() ?? string.Empty;

        try
        {
            // The buyer is the only user allowed to review a transaction.
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

    private void SeedData()
    {
        var abdulla = _userService.RegisterUser("Abdulla", "1234");
        _userService.RegisterUser("Mehedi", "1234");

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