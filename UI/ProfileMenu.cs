using MarketplaceConsoleApp.Models;
using MarketplaceConsoleApp.Services;
using Spectre.Console;

namespace MarketplaceConsoleApp.UI;

/// <summary>
/// Handles profile and history-related menu actions.
/// </summary>
public class ProfileMenu
{
    private readonly ReviewService _reviewService;

    /// <summary>
    /// Creates a new profile menu with required services.
    /// </summary>
    public ProfileMenu(ReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    /// <summary>
    /// Shows the current user's profile and received reviews.
    /// </summary>
    public void ShowMyProfile(User currentUser)
    {
        var reviews = _reviewService.GetReviewsForUser(currentUser);

        var profilePanel = new Panel(
            $"[yellow]Username:[/] {EscapeMarkup(currentUser.Username)}\n" +
            $"[yellow]Listings:[/] {currentUser.Listings.Count}\n" +
            $"[yellow]Purchases:[/] {currentUser.Purchases.Count}\n" +
            $"[yellow]Sales:[/] {currentUser.Sales.Count}\n" +
            $"[yellow]Average Rating:[/] {currentUser.AverageRating:F1}")
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

    /// <summary>
    /// Shows the current user's purchase history.
    /// </summary>
    public void ShowMyPurchases(User currentUser)
    {
        if (!currentUser.Purchases.Any())
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

        for (int i = 0; i < currentUser.Purchases.Count; i++)
        {
            var purchase = currentUser.Purchases[i];
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

    /// <summary>
    /// Shows the current user's sales history.
    /// </summary>
    public void ShowMySales(User currentUser)
    {
        if (!currentUser.Sales.Any())
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

        for (int i = 0; i < currentUser.Sales.Count; i++)
        {
            var sale = currentUser.Sales[i];
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

    private static string EscapeMarkup(string text)
    {
        return Markup.Escape(text);
    }
}