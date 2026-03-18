using MarketplaceConsoleApp.Enums;

namespace MarketplaceConsoleApp.Models;

/// <summary>
/// Represents an item listed for sale in the marketplace.
/// </summary>
public class Listing
{
    /// <summary>
    /// Title of the listing.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Description of the item.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Category of the item.
    /// </summary>
    public Category Category { get; set; }

    /// <summary>
    /// Condition of the item.
    /// </summary>
    public ItemCondition Condition { get; set; }

    /// <summary>
    /// Price in NOK.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Current status of the listing.
    /// </summary>
    public ListingStatus Status { get; set; }

    /// <summary>
    /// The user who created the listing.
    /// </summary>
    public User Seller { get; set; }

    /// <summary>
    /// The user who bought the item (if sold).
    /// </summary>
    public User? Buyer { get; set; }

    /// <summary>
    /// Creates a new listing.
    /// </summary>
    public Listing(
        string title,
        string description,
        Category category,
        ItemCondition condition,
        decimal price,
        User seller)
    {
        Title = title;
        Description = description;
        Category = category;
        Condition = condition;
        Price = price;
        Seller = seller;
        Buyer = null;
        Status = ListingStatus.Available;
    }
}