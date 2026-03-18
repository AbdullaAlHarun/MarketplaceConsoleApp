namespace MarketplaceConsoleApp.Models;

/// <summary>
/// Represents a review given after a completed transaction.
/// </summary>
public class Review
{
    /// <summary>
    /// Rating value (1–6).
    /// </summary>
    public int Rating { get; set; }

    /// <summary>
    /// Optional comment.
    /// </summary>
    public string Comment { get; set; }

    /// <summary>
    /// The user who wrote the review.
    /// </summary>
    public User Reviewer { get; set; }

    /// <summary>
    /// The user being reviewed.
    /// </summary>
    public User ReviewedUser { get; set; }

    /// <summary>
    /// The related transaction.
    /// </summary>
    public Transaction Transaction { get; set; }

    /// <summary>
    /// Creates a new review.
    /// </summary>
    public Review(int rating, string comment, User reviewer, User reviewedUser, Transaction transaction)
    {
        Rating = rating;
        Comment = comment;
        Reviewer = reviewer;
        ReviewedUser = reviewedUser;
        Transaction = transaction;
    }
}