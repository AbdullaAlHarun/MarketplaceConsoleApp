using MarketplaceConsoleApp.Enums;

namespace MarketplaceConsoleApp.Models;

public class Listing
{
    public string Title { get; set; }

    public string Description { get; set; }

    public Category Category { get; set; }

    public ItemCondition Condition { get; set; }

    public decimal Price { get; set; }

    public ListingStatus Status { get; set; }

    public User Seller { get; set; }

    public User? Buyer { get; set; }

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