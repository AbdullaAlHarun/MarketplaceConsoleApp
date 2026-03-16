using MarketplaceConsoleApp.Enums;
using MarketplaceConsoleApp.Helpers;
using MarketplaceConsoleApp.Models;
using MarketplaceConsoleApp.Services;
using Spectre.Console;

namespace MarketplaceConsoleApp.UI;

/// <summary>
/// Handles the main application flow and top-level menus.
/// </summary>
public class ConsoleMenu
{
    private readonly UserService _userService;
    private readonly ListingService _listingService;
    private readonly TransactionService _transactionService;
    private readonly ReviewService _reviewService;

    private readonly ListingMenu _listingMenu;
    private readonly ProfileMenu _profileMenu;
    private readonly ReviewMenu _reviewMenu;

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

        _listingMenu = new ListingMenu(_listingService, _transactionService);
        _profileMenu = new ProfileMenu(_reviewService);
        _reviewMenu = new ReviewMenu(_reviewService);
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
                _listingMenu.CreateListingForCurrentUser(_currentUser!);
                break;
            case "Browse Available Listings":
                _listingMenu.BrowseAvailableListings(_currentUser!);
                break;
            case "Filter Listings by Category":
                _listingMenu.FilterListingsByCategory(_currentUser!);
                break;
            case "Search Listings":
                _listingMenu.SearchListings(_currentUser!);
                break;
            case "My Profile":
                _profileMenu.ShowMyProfile(_currentUser!);
                break;
            case "Manage My Listings":
                _listingMenu.ManageMyListings(_currentUser!);
                break;
            case "My Purchases":
                _profileMenu.ShowMyPurchases(_currentUser!);
                break;
            case "My Sales":
                _profileMenu.ShowMySales(_currentUser!);
                break;
            case "Leave Review":
                _reviewMenu.LeaveReviewAsCurrentUser(_currentUser!);
                break;
            case "View All Reviews":
                _reviewMenu.ShowReviews();
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