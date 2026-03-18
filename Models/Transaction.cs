namespace MarketplaceConsoleApp.Models;

/// <summary>
/// Represents a completed purchase between a buyer and a seller.
/// </summary>
public class Transaction
{
    /// <summary>
    /// The listing that was purchased.
    /// </summary>
    public Listing Listing { get; set; }

    /// <summary>
    /// The buyer of the item.
    /// </summary>
    public User Buyer { get; set; }

    /// <summary>
    /// The seller of the item.
    /// </summary>
    public User Seller { get; set; }

    /// <summary>
    /// Final price of the transaction.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Date and time of the purchase.
    /// </summary>
    public DateTime TransactionDate { get; set; }

    /// <summary>
    /// Creates a new transaction.
    /// </summary>
    public Transaction(Listing listing, User buyer)
    {
        Listing = listing;
        Buyer = buyer;
        Seller = listing.Seller;
        Price = listing.Price;
        TransactionDate = DateTime.Now;
    }
}