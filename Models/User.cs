namespace MarketplaceConsoleApp.Models;

/// <summary>
/// Represents a user in the marketplace.
/// </summary>
public class User
{
    /// <summary>
    /// Username of the user.
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// Password of the user.
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Listings created by the user.
    /// </summary>
    public List<Listing> Listings { get; set; }

    /// <summary>
    /// Items purchased by the user.
    /// </summary>
    public List<Transaction> Purchases { get; set; }

    /// <summary>
    /// Items sold by the user.
    /// </summary>
    public List<Transaction> Sales { get; set; }

    /// <summary>
    /// Reviews received from other users.
    /// </summary>
    public List<Review> ReviewsReceived { get; set; }

    /// <summary>
    /// Calculates the average rating based on received reviews.
    /// </summary>
    public double AverageRating
    {
        get
        {
            if (ReviewsReceived.Count == 0)
                return 0;

            return ReviewsReceived.Average(r => r.Rating);
        }
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    public User(string username, string password)
    {
        Username = username;
        Password = password;
        Listings = new List<Listing>();
        Purchases = new List<Transaction>();
        Sales = new List<Transaction>();
        ReviewsReceived = new List<Review>();
    }
}