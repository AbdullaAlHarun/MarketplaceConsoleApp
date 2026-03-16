using MarketplaceConsoleApp.Helpers;
using MarketplaceConsoleApp.Models;
using MarketplaceConsoleApp.Services;
using Spectre.Console;

namespace MarketplaceConsoleApp.UI;

/// <summary>
/// Handles review-related menu actions.
/// </summary>
public class ReviewMenu
{
    private readonly ReviewService _reviewService;

    /// <summary>
    /// Creates a new review menu with required services.
    /// </summary>
    public ReviewMenu(ReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    /// <summary>
    /// Allows the current user to leave a review for a completed purchase.
    /// </summary>
    public void LeaveReviewAsCurrentUser(User currentUser)
    {
        var myPurchases = currentUser.Purchases;

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
            var review = _reviewService.AddReview(rating, comment, currentUser, transaction);
            AnsiConsole.MarkupLine("[green]Review added successfully.[/]");
            AnsiConsole.MarkupLine(
                $"Seller: {EscapeMarkup(review.ReviewedUser.Username)} | Rating: {review.Rating}");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Could not add review:[/] {EscapeMarkup(ex.Message)}");
        }
    }

    /// <summary>
    /// Shows all reviews in the system.
    /// </summary>
    public void ShowReviews()
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

    private static string EscapeMarkup(string text)
    {
        return Markup.Escape(text);
    }
}