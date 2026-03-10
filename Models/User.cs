namespace MarketplaceConsoleApp.Models;

public class User
{
    public string Username { get; set; }

    public string Password { get; set; }

    public List<Listing> Listings { get; set; }

    public List<Transaction> Purchases { get; set; }

    public List<Transaction> Sales { get; set; }

    public List<Review> ReviewsReceived { get; set; }

    public double AverageRating
    {
        get
        {
            if (ReviewsReceived.Count == 0)
                return 0;

            return ReviewsReceived.Average(r => r.Rating);
        }
    }

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