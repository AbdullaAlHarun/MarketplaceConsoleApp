namespace MarketplaceConsoleApp.Models;

public class Transaction
{
    public Listing Listing { get; set; }

    public User Buyer { get; set; }

    public User Seller { get; set; }

    public decimal Price { get; set; }

    public DateTime TransactionDate { get; set; }

    public Transaction(Listing listing, User buyer)
    {
        Listing = listing;
        Buyer = buyer;
        Seller = listing.Seller;
        Price = listing.Price;
        TransactionDate = DateTime.Now;
    }
}