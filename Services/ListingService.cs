using MarketplaceConsoleApp.Models;
using MarketplaceConsoleApp.Enums;

namespace MarketplaceConsoleApp.Services;

public class ListingService
{
    private List<Listing> _listings = new List<Listing>();

    public Listing CreateListing(
        string title,
        string description,
        Category category,
        ItemCondition condition,
        decimal price,
        User seller)
    {
        var listing = new Listing(title, description, category, condition, price, seller);

        _listings.Add(listing);
        seller.Listings.Add(listing);

        return listing;
    }

    public List<Listing> GetAllListings()
    {
        return _listings;
    }

    public List<Listing> GetAvailableListings()
    {
        return _listings
            .Where(l => l.Status == ListingStatus.Available)
            .ToList();
    }
}