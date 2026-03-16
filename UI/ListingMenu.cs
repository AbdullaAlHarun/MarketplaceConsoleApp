using MarketplaceConsoleApp.Enums;
using MarketplaceConsoleApp.Helpers;
using MarketplaceConsoleApp.Models;
using MarketplaceConsoleApp.Services;
using Spectre.Console;

namespace MarketplaceConsoleApp.UI;

/// <summary>
/// Handles listing-related menu actions.
/// </summary>
public class ListingMenu
{
    private readonly ListingService _listingService;
    private readonly TransactionService _transactionService;

    /// <summary>
    /// Creates a new listing menu with required services.
    /// </summary>
    public ListingMenu(ListingService listingService, TransactionService transactionService)
    {
        _listingService = listingService;
        _transactionService = transactionService;
    }

    /// <summary>
    /// Creates a new listing for the current user.
    /// </summary>
    public void CreateListingForCurrentUser(User currentUser)
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
            currentUser);

        AnsiConsole.MarkupLine("[green]Listing created successfully.[/]");
        ListingViewHelper.ShowListingDetails(listing);
    }

    /// <summary>
    /// Shows available listings and allows the current user to buy one.
    /// </summary>
    public void BrowseAvailableListings(User currentUser)
    {
        var availableListings = _listingService.GetAvailableListings()
            .Where(l => l.Seller != currentUser)
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
            PurchaseListingAsCurrentUser(selectedListing, currentUser);
        }
    }

    /// <summary>
    /// Filters available listings by category.
    /// </summary>
    public void FilterListingsByCategory(User currentUser)
    {
        AnsiConsole.Write(new Rule("[yellow]Filter Listings by Category[/]"));

        Category selectedCategory = MenuInputHelper.SelectEnumValue<Category>("Select category:");

        var filteredListings = _listingService.GetAvailableListings()
            .Where(l => l.Seller != currentUser)
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

    /// <summary>
    /// Searches listings by keyword in title or description.
    /// </summary>
    public void SearchListings(User currentUser)
    {
        AnsiConsole.Write(new Rule("[yellow]Search Listings[/]"));
        string keyword = MenuInputHelper.ReadRequiredText("Enter keyword: ");

        var results = _listingService.GetAvailableListings()
            .Where(l => l.Seller != currentUser)
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

    /// <summary>
    /// Shows the current user's listings and allows edit or remove actions.
    /// </summary>
    public void ManageMyListings(User currentUser)
    {
        if (!currentUser.Listings.Any())
        {
            AnsiConsole.MarkupLine("[yellow]You have not created any listings.[/]");
            return;
        }

        AnsiConsole.Write(new Rule("[green]My Listings[/]"));
        ListingViewHelper.PrintListings(currentUser.Listings);
        AnsiConsole.WriteLine();

        var selectedListing = AnsiConsole.Prompt(
            new SelectionPrompt<Listing>()
                .Title("Select one of your listings:")
                .PageSize(10)
                .UseConverter(l =>
                    $"{l.Title} | {l.Category} | {l.Condition} | {l.Price} NOK | Status: {l.Status}")
                .AddChoices(currentUser.Listings));

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
                EditListing(selectedListing, currentUser);
                break;
            case "Remove Listing":
                RemoveListing(selectedListing, currentUser);
                break;
            case "Go Back":
                return;
        }
    }

    /// <summary>
    /// Updates an existing listing owned by the current user.
    /// </summary>
    public void EditListing(Listing listing, User currentUser)
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
                currentUser,
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

    /// <summary>
    /// Removes an existing listing owned by the current user.
    /// </summary>
    public void RemoveListing(Listing listing, User currentUser)
    {
        var confirm = AnsiConsole.Confirm("Are you sure you want to remove this listing?");

        if (!confirm)
        {
            AnsiConsole.MarkupLine("[yellow]Remove cancelled.[/]");
            return;
        }

        try
        {
            _listingService.RemoveListing(listing, currentUser);
            AnsiConsole.MarkupLine("[green]Listing removed successfully.[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Could not remove listing:[/] {EscapeMarkup(ex.Message)}");
        }
    }

    /// <summary>
    /// Purchases the selected listing for the current user.
    /// </summary>
    public void PurchaseListingAsCurrentUser(Listing listing, User currentUser)
    {
        try
        {
            var transaction = _transactionService.PurchaseListing(listing, currentUser);

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

    private static string EscapeMarkup(string text)
    {
        return Markup.Escape(text);
    }
}