using MarketplaceConsoleApp.Services;
using MarketplaceConsoleApp.UI;

namespace MarketplaceConsoleApp;

internal class Program
{
    static void Main(string[] args)
    {
        var userService = new UserService();
        var listingService = new ListingService();
        var transactionService = new TransactionService();
        var reviewService = new ReviewService();

        var menu = new ConsoleMenu(
            userService,
            listingService,
            transactionService,
            reviewService);

        menu.Run();
    }
}