namespace MarketplaceConsoleApp.Models;

public class Review
{
    public int Rating { get; set; }

    public string Comment { get; set; }

    public User Reviewer { get; set; }

    public User ReviewedUser { get; set; }

    public Transaction Transaction { get; set; }

    public Review(int rating, string comment, User reviewer, User reviewedUser, Transaction transaction)
    {
        Rating = rating;
        Comment = comment;
        Reviewer = reviewer;
        ReviewedUser = reviewedUser;
        Transaction = transaction;
    }
}