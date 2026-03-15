using MarketplaceConsoleApp.Models;
using Spectre.Console;

namespace MarketplaceConsoleApp.UI;

/// <summary>
/// Helper methods for showing listing data in the console.
/// </summary>
public static class ListingViewHelper
{
    /// <summary>
    /// Displays one listing in a formatted panel.
    /// </summary>
    public static void ShowListingDetails(Listing listing)
    {
        var grid = new Grid();
        grid.AddColumn();
        grid.AddColumn();

        grid.AddRow("[yellow]Seller:[/]", EscapeMarkup(listing.Seller.Username));
        grid.AddRow("[yellow]Category:[/]", EscapeMarkup(listing.Category.ToString()));
        grid.AddRow("[yellow]Condition:[/]", EscapeMarkup(listing.Condition.ToString()));
        grid.AddRow("[yellow]Price:[/]", $"{listing.Price} NOK");
        grid.AddRow("[yellow]Status:[/]", EscapeMarkup(listing.Status.ToString()));
        grid.AddRow("[yellow]Description:[/]", EscapeMarkup(listing.Description));

        var panel = new Panel(grid)
        {
            Header = new PanelHeader(EscapeMarkup(listing.Title)),
            Border = BoxBorder.Rounded
        };

        AnsiConsole.Write(panel);
    }

    /// <summary>
    /// Displays a list of listings in a formatted table.
    /// </summary>
    public static void PrintListings(List<Listing> listings)
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .Expand();

        table.AddColumn("[bold]#[/]");
        table.AddColumn("[bold]Title[/]");
        table.AddColumn("[bold]Category[/]");
        table.AddColumn("[bold]Condition[/]");
        table.AddColumn("[bold]Price[/]");
        table.AddColumn("[bold]Status[/]");
        table.AddColumn("[bold]Seller[/]");

        for (int i = 0; i < listings.Count; i++)
        {
            var listing = listings[i];

            table.AddRow(
                (i + 1).ToString(),
                EscapeMarkup(listing.Title),
                EscapeMarkup(listing.Category.ToString()),
                EscapeMarkup(listing.Condition.ToString()),
                $"{listing.Price} NOK",
                EscapeMarkup(listing.Status.ToString()),
                EscapeMarkup(listing.Seller.Username));
        }

        AnsiConsole.Write(table);
    }

    private static string EscapeMarkup(string text)
    {
        return Markup.Escape(text);
    }
}