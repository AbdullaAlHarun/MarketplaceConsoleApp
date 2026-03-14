using MarketplaceConsoleApp.Enums;
using MarketplaceConsoleApp.Models;

namespace MarketplaceConsoleApp.Services;

/// <summary>
/// Handles creation, retrieval, update, and removal of marketplace listings.
/// </summary>
public class ListingService
{
    private readonly List<Listing> _listings = new();

    /// <summary>
    /// Creates a new listing for the given seller.
    /// </summary>
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

    /// <summary>
    /// Returns all listings.
    /// </summary>
    public List<Listing> GetAllListings()
    {
        return _listings;
    }

    /// <summary>
    /// Returns only listings that are still available.
    /// </summary>
    public List<Listing> GetAvailableListings()
    {
        return _listings
            .Where(l => l.Status == ListingStatus.Available)
            .ToList();
    }

    /// <summary>
    /// Updates an existing listing. Only the seller can edit it.
    /// </summary>
    public void UpdateListing(
        Listing listing,
        User seller,
        string title,
        string description,
        Category category,
        ItemCondition condition,
        decimal price)
    {
        if (listing.Seller != seller)
        {
            throw new InvalidOperationException("Only the seller can edit this listing.");
        }

        if (listing.Status == ListingStatus.Sold)
        {
            throw new InvalidOperationException("Sold listings cannot be edited.");
        }

        listing.Title = title;
        listing.Description = description;
        listing.Category = category;
        listing.Condition = condition;
        listing.Price = price;
    }

    /// <summary>
    /// Removes a listing. Only the seller can remove it.
    /// </summary>
    public void RemoveListing(Listing listing, User seller)
    {
        if (listing.Seller != seller)
        {
            throw new InvalidOperationException("Only the seller can remove this listing.");
        }

        if (listing.Status == ListingStatus.Sold)
        {
            throw new InvalidOperationException("Sold listings cannot be removed.");
        }

        _listings.Remove(listing);
        seller.Listings.Remove(listing);
    }
}