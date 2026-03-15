using MarketplaceConsoleApp.Enums;
using MarketplaceConsoleApp.Models;
using MarketplaceConsoleApp.Services;
using MarketplaceConsoleApp.Helpers;
using Spectre.Console;

namespace MarketplaceConsoleApp.UI;

/// <summary>
/// Handles the main console interaction for the marketplace application.
/// </summary>
public class ConsoleMenu
{
    private readonly UserService _userService;
    private readonly ListingService _listingService;
    private readonly TransactionService _transactionService;
    private readonly ReviewService _reviewService;

    private User? _currentUser;

    /// <summary>
    /// Creates a new console menu with required services.
    /// </summary>
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

    /// <summary>
    /// Starts the application loop.
    /// </summary>
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
        AnsiConsole.Write(new Rule("[yellow]Second-Hand Market[/]"));
        AnsiConsole.MarkupLine("[grey]Buy and sell used items easily[/]");
        AnsiConsole.WriteLine();

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Choose an option:")
                .PageSize(10)
                .AddChoices("Register", "Log In", "Exit"));

        Console.Clear();

        switch (choice)
        {
            case "Register":
                RegisterUser();
                MenuInputHelper.Pause();
                return true;

            case "Log In":
                LoginUser();
                MenuInputHelper.Pause();
                return true;

            case "Exit":
                AnsiConsole.MarkupLine("[yellow]Exiting application...[/]");
                return false;

            default:
                return true;
        }
    }

    private void ShowUserMenu()
    {
        AnsiConsole.Write(new Rule($"[green]Main Menu ({EscapeMarkup(_currentUser!.Username)})[/]"));
        AnsiConsole.WriteLine();

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Choose an action:")
                .PageSize(12)
                .AddChoices(
                    "Create Listing",
                    "Browse Available Listings",
                    "Filter Listings by Category",
                    "Search Listings",
                    "My Profile",
                    "Manage My Listings",
                    "My Purchases",
                    "My Sales",
                    "Leave Review",
                    "View All Reviews",
                    "Log Out"));

        Console.Clear();

        switch (choice)
        {
            case "Create Listing":
                CreateListingForCurrentUser();
                break;
            case "Browse Available Listings":
                BrowseAvailableListings();
                break;
            case "Filter Listings by Category":
                FilterListingsByCategory();
                break;
            case "Search Listings":
                SearchListings();
                break;
            case "My Profile":
                ShowMyProfile();
                break;
            case "Manage My Listings":
                ManageMyListings();
                break;
            case "My Purchases":
                ShowMyPurchases();
                break;
            case "My Sales":
                ShowMySales();
                break;
            case "Leave Review":
                LeaveReviewAsCurrentUser();
                break;
            case "View All Reviews":
                ShowReviews();
                break;
            case "Log Out":
                LogoutUser();
                return;
        }

        MenuInputHelper.Pause();
    }

    private void RegisterUser()
    {
        AnsiConsole.Write(new Rule("[yellow]Register User[/]"));

        string username = MenuInputHelper.ReadRequiredText("Enter username: ");
        string password = MenuInputHelper.ReadRequiredText("Enter password: ");

        var existingUser = _userService.GetAllUsers()
            .FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

        if (existingUser != null)
        {
            AnsiConsole.MarkupLine("[red]Username already exists.[/]");
            return;
        }

        var user = _userService.RegisterUser(username, password);
        AnsiConsole.MarkupLine($"[green]User '{EscapeMarkup(user.Username)}' registered successfully.[/]");
    }

    private void LoginUser()
    {
        AnsiConsole.Write(new Rule("[yellow]Log In[/]"));

        string username = MenuInputHelper.ReadRequiredText("Enter username: ");
        string password = MenuInputHelper.ReadRequiredText("Enter password: ");

        var user = _userService.LoginUser(username, password);

        if (user == null)
        {
            AnsiConsole.MarkupLine("[red]Invalid username or password.[/]");
            return;
        }

        _currentUser = user;
        AnsiConsole.MarkupLine($"[green]Welcome back, {EscapeMarkup(_currentUser.Username)}![/]");
    }

    private void LogoutUser()
    {
        AnsiConsole.MarkupLine($"[yellow]{EscapeMarkup(_currentUser!.Username)} logged out.[/]");
        _currentUser = null;
    }

    private void CreateListingForCurrentUser()
    {
        AnsiConsole.Write(new Rule("[yellow]Create Listing[/]"));

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

        AnsiConsole.MarkupLine("[green]Listing created successfully.[/]");
        ListingViewHelper.ShowListingDetails(listing);
    }

    private void BrowseAvailableListings()
    {
        var availableListings = _listingService.GetAvailableListings()
            .Where(l => l.Seller != _currentUser)
            .ToList();

        if (!availableListings.Any())
        {
            AnsiConsole.MarkupLine("[yellow]No available listings found.[/]");
            return;
        }

        AnsiConsole.Write(new Rule("[green]Available Listings[/]"));
        ListingViewHelper.PrintListings(availableListings);
        AnsiConsole.WriteLine();

        var selectedListing = AnsiConsole.Prompt(
            new SelectionPrompt<Listing>()
                .Title("Select a listing:")
                .PageSize(10)
                .UseConverter(l =>
                    $"{l.Title} | {l.Category} | {l.Condition} | {l.Price} NOK | Seller: {l.Seller.Username}")
                .AddChoices(availableListings));

        Console.Clear();
        ListingViewHelper.ShowListingDetails(selectedListing);
        AnsiConsole.WriteLine();

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Choose an option:")
                .AddChoices("Buy this item", "Go back"));

        if (choice == "Buy this item")
        {
            PurchaseListingAsCurrentUser(selectedListing);
        }
    }

    private void FilterListingsByCategory()
    {
        AnsiConsole.Write(new Rule("[yellow]Filter Listings by Category[/]"));

        Category selectedCategory = MenuInputHelper.SelectEnumValue<Category>("Select category:");

        var filteredListings = _listingService.GetAvailableListings()
            .Where(l => l.Seller != _currentUser)
            .Where(l => l.Category == selectedCategory)
            .ToList();

        if (!filteredListings.Any())
        {
            AnsiConsole.MarkupLine("[yellow]No listings found in that category.[/]");
            return;
        }

        AnsiConsole.Write(new Rule($"[green]{EscapeMarkup(selectedCategory.ToString())} Listings[/]"));
        ListingViewHelper.PrintListings(filteredListings);
    }

    private void SearchListings()
    {
        AnsiConsole.Write(new Rule("[yellow]Search Listings[/]"));
        string keyword = MenuInputHelper.ReadRequiredText("Enter keyword: ");

        var results = _listingService.GetAvailableListings()
            .Where(l => l.Seller != _currentUser)
            .Where(l =>
                l.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                l.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (!results.Any())
        {
            AnsiConsole.MarkupLine("[yellow]No matching listings found.[/]");
            return;
        }

        AnsiConsole.Write(new Rule("[green]Search Results[/]"));
        ListingViewHelper.PrintListings(results);
    }

    private void ManageMyListings()
    {
        if (!_currentUser!.Listings.Any())
        {
            AnsiConsole.MarkupLine("[yellow]You have not created any listings.[/]");
            return;
        }

        AnsiConsole.Write(new Rule("[green]My Listings[/]"));
        ListingViewHelper.PrintListings(_currentUser.Listings);
        AnsiConsole.WriteLine();

        var selectedListing = AnsiConsole.Prompt(
            new SelectionPrompt<Listing>()
                .Title("Select one of your listings:")
                .PageSize(10)
                .UseConverter(l =>
                    $"{l.Title} | {l.Category} | {l.Condition} | {l.Price} NOK | Status: {l.Status}")
                .AddChoices(_currentUser.Listings));

        Console.Clear();
        ListingViewHelper.ShowListingDetails(selectedListing);
        AnsiConsole.WriteLine();

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Choose an option:")
                .AddChoices("Edit Listing", "Remove Listing", "Go Back"));

        switch (choice)
        {
            case "Edit Listing":
                EditListing(selectedListing);
                break;
            case "Remove Listing":
                RemoveListing(selectedListing);
                break;
            case "Go Back":
                return;
        }
    }

    private void EditListing(Listing listing)
    {
        AnsiConsole.Write(new Rule("[yellow]Edit Listing[/]"));

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

            AnsiConsole.MarkupLine("[green]Listing updated successfully.[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Could not update listing:[/] {EscapeMarkup(ex.Message)}");
        }
    }

    private void RemoveListing(Listing listing)
    {
        var confirm = AnsiConsole.Confirm("Are you sure you want to remove this listing?");

        if (!confirm)
        {
            AnsiConsole.MarkupLine("[yellow]Remove cancelled.[/]");
            return;
        }

        try
        {
            _listingService.RemoveListing(listing, _currentUser!);
            AnsiConsole.MarkupLine("[green]Listing removed successfully.[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Could not remove listing:[/] {EscapeMarkup(ex.Message)}");
        }
    }

    private void PurchaseListingAsCurrentUser(Listing listing)
    {
        try
        {
            var transaction = _transactionService.PurchaseListing(listing, _currentUser!);

            AnsiConsole.MarkupLine("[green]Purchase completed successfully.[/]");
            AnsiConsole.WriteLine();

            var panel = new Panel(
                $"[yellow]Item:[/] {EscapeMarkup(transaction.Listing.Title)}\n" +
                $"[yellow]Buyer:[/] {EscapeMarkup(transaction.Buyer.Username)}\n" +
                $"[yellow]Seller:[/] {EscapeMarkup(transaction.Seller.Username)}\n" +
                $"[yellow]Price:[/] {transaction.Price} NOK\n" +
                $"[yellow]Date:[/] {transaction.TransactionDate:g}")
            {
                Header = new PanelHeader("Purchase Summary"),
                Border = BoxBorder.Rounded
            };

            AnsiConsole.Write(panel);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Purchase failed:[/] {EscapeMarkup(ex.Message)}");
        }
    }

    private void ShowMyProfile()
    {
        var reviews = _reviewService.GetReviewsForUser(_currentUser!);

        var profilePanel = new Panel(
            $"[yellow]Username:[/] {EscapeMarkup(_currentUser.Username)}\n" +
            $"[yellow]Listings:[/] {_currentUser.Listings.Count}\n" +
            $"[yellow]Purchases:[/] {_currentUser.Purchases.Count}\n" +
            $"[yellow]Sales:[/] {_currentUser.Sales.Count}\n" +
            $"[yellow]Average Rating:[/] {_currentUser.AverageRating:F1}")
        {
            Header = new PanelHeader("My Profile"),
            Border = BoxBorder.Rounded
        };

        AnsiConsole.Write(profilePanel);

        if (!reviews.Any())
        {
            AnsiConsole.MarkupLine("[yellow]No reviews received yet.[/]");
            return;
        }

        var table = new Table().Border(TableBorder.Rounded);
        table.AddColumn("[bold]#[/]");
        table.AddColumn("[bold]Rating[/]");
        table.AddColumn("[bold]Reviewer[/]");
        table.AddColumn("[bold]Comment[/]");

        for (int i = 0; i < reviews.Count; i++)
        {
            var review = reviews[i];
            table.AddRow(
                (i + 1).ToString(),
                review.Rating.ToString(),
                EscapeMarkup(review.Reviewer.Username),
                EscapeMarkup(string.IsNullOrWhiteSpace(review.Comment) ? "-" : review.Comment));
        }

        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Rule("[green]Reviews Received[/]"));
        AnsiConsole.Write(table);
    }

    private void ShowMyPurchases()
    {
        if (!_currentUser!.Purchases.Any())
        {
            AnsiConsole.MarkupLine("[yellow]You have not purchased any items.[/]");
            return;
        }

        var table = new Table().Border(TableBorder.Rounded);
        table.AddColumn("[bold]#[/]");
        table.AddColumn("[bold]Item[/]");
        table.AddColumn("[bold]Seller[/]");
        table.AddColumn("[bold]Price[/]");
        table.AddColumn("[bold]Date[/]");

        for (int i = 0; i < _currentUser.Purchases.Count; i++)
        {
            var purchase = _currentUser.Purchases[i];
            table.AddRow(
                (i + 1).ToString(),
                EscapeMarkup(purchase.Listing.Title),
                EscapeMarkup(purchase.Seller.Username),
                $"{purchase.Price} NOK",
                purchase.TransactionDate.ToString("g"));
        }

        AnsiConsole.Write(new Rule("[green]My Purchases[/]"));
        AnsiConsole.Write(table);
    }

    private void ShowMySales()
    {
        if (!_currentUser!.Sales.Any())
        {
            AnsiConsole.MarkupLine("[yellow]You have not sold any items.[/]");
            return;
        }

        var table = new Table().Border(TableBorder.Rounded);
        table.AddColumn("[bold]#[/]");
        table.AddColumn("[bold]Item[/]");
        table.AddColumn("[bold]Buyer[/]");
        table.AddColumn("[bold]Price[/]");
        table.AddColumn("[bold]Date[/]");

        for (int i = 0; i < _currentUser.Sales.Count; i++)
        {
            var sale = _currentUser.Sales[i];
            table.AddRow(
                (i + 1).ToString(),
                EscapeMarkup(sale.Listing.Title),
                EscapeMarkup(sale.Buyer.Username),
                $"{sale.Price} NOK",
                sale.TransactionDate.ToString("g"));
        }

        AnsiConsole.Write(new Rule("[green]My Sales[/]"));
        AnsiConsole.Write(table);
    }

    private void LeaveReviewAsCurrentUser()
    {
        var myPurchases = _currentUser!.Purchases;

        if (!myPurchases.Any())
        {
            AnsiConsole.MarkupLine("[yellow]You have no purchases to review.[/]");
            return;
        }

        AnsiConsole.Write(new Rule("[green]Select Purchase to Review[/]"));

        var transaction = AnsiConsole.Prompt(
            new SelectionPrompt<Transaction>()
                .Title("Choose a purchase:")
                .PageSize(10)
                .UseConverter(t =>
                    $"{t.Listing.Title} | Seller: {t.Seller.Username} | {t.Price} NOK | {t.TransactionDate:g}")
                .AddChoices(myPurchases));

        int rating = MenuInputHelper.ReadChoice("Enter rating (1-6): ", 1, 6);
        string comment = AnsiConsole.Ask<string>("Enter comment (optional):");

        try
        {
            var review = _reviewService.AddReview(rating, comment, _currentUser, transaction);
            AnsiConsole.MarkupLine("[green]Review added successfully.[/]");
            AnsiConsole.MarkupLine(
                $"Seller: {EscapeMarkup(review.ReviewedUser.Username)} | Rating: {review.Rating}");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Could not add review:[/] {EscapeMarkup(ex.Message)}");
        }
    }

    private void ShowReviews()
    {
        var reviews = _reviewService.GetAllReviews();

        if (!reviews.Any())
        {
            AnsiConsole.MarkupLine("[yellow]No reviews found.[/]");
            return;
        }

        var table = new Table().Border(TableBorder.Rounded);
        table.AddColumn("[bold]#[/]");
        table.AddColumn("[bold]Rating[/]");
        table.AddColumn("[bold]Reviewer[/]");
        table.AddColumn("[bold]Reviewed User[/]");
        table.AddColumn("[bold]Item[/]");
        table.AddColumn("[bold]Comment[/]");

        for (int i = 0; i < reviews.Count; i++)
        {
            var review = reviews[i];
            table.AddRow(
                (i + 1).ToString(),
                review.Rating.ToString(),
                EscapeMarkup(review.Reviewer.Username),
                EscapeMarkup(review.ReviewedUser.Username),
                EscapeMarkup(review.Transaction.Listing.Title),
                EscapeMarkup(string.IsNullOrWhiteSpace(review.Comment) ? "-" : review.Comment));
        }

        AnsiConsole.Write(new Rule("[green]Reviews[/]"));
        AnsiConsole.Write(table);
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

    private static string EscapeMarkup(string text)
    {
        return Markup.Escape(text);
    }
}
