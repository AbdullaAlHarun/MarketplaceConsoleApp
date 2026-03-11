using MarketplaceConsoleApp.Models;
using MarketplaceConsoleApp.Enums;

namespace MarketplaceConsoleApp.Services;

public class TransactionService
{
    private List<Transaction> _transactions = new List<Transaction>();

    public Transaction PurchaseListing(Listing listing, User buyer)
    {
        if (listing.Status == ListingStatus.Sold)
        {
            throw new InvalidOperationException("This listing has already been sold.");
        }

        if (listing.Seller == buyer)
        {
            throw new InvalidOperationException("You cannot buy your own listing.");
        }

        listing.Status = ListingStatus.Sold;
        listing.Buyer = buyer;

        var transaction = new Transaction(listing, buyer);

        _transactions.Add(transaction);
        buyer.Purchases.Add(transaction);
        listing.Seller.Sales.Add(transaction);

        return transaction;
    }

    public List<Transaction> GetAllTransactions()
    {
        return _transactions;
    }
}