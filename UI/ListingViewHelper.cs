using MarketplaceConsoleApp.Models;

namespace MarketplaceConsoleApp.UI;

public static class ListingViewHelper
{
    public static void ShowListingDetails(Listing listing)
    {
        Console.WriteLine($"=== {listing.Title} ===");
        Console.WriteLine($"Seller: {listing.Seller.Username}");
        Console.WriteLine($"Category: {listing.Category}");
        Console.WriteLine($"Condition: {listing.Condition}");
        Console.WriteLine($"Price: {listing.Price} NOK");
        Console.WriteLine($"Status: {listing.Status}");
        Console.WriteLine($"Description: {listing.Description}");
    }

    public static void PrintListings(List<Listing> listings)
    {
        for (int i = 0; i < listings.Count; i++)
        {
            var listing = listings[i];
            Console.WriteLine(
                $"{i + 1}. {listing.Title} | {listing.Category} | {listing.Condition} | {listing.Price} NOK | Status: {listing.Status} | Seller: {listing.Seller.Username}");
        }
    }
}