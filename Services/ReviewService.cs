using MarketplaceConsoleApp.Models;

namespace MarketplaceConsoleApp.Services;

public class ReviewService
{
    private List<Review> _reviews = new List<Review>();

    public Review AddReview(int rating, string comment, User reviewer, Transaction transaction)
    {
        if (rating < 1 || rating > 6)
        {
            throw new ArgumentException("Rating must be between 1 and 6.");
        }

        if (transaction.Buyer != reviewer)
        {
            throw new InvalidOperationException("Only the buyer can leave a review for this transaction.");
        }

        bool reviewAlreadyExists = _reviews.Any(r => r.Transaction == transaction);
        if (reviewAlreadyExists)
        {
            throw new InvalidOperationException("A review has already been added for this transaction.");
        }

        var reviewedUser = transaction.Seller;

        var review = new Review(rating, comment, reviewer, reviewedUser, transaction);

        _reviews.Add(review);
        reviewedUser.ReviewsReceived.Add(review);

        return review;
    }

    public List<Review> GetAllReviews()
    {
        return _reviews;
    }

    public List<Review> GetReviewsForUser(User user)
    {
        return _reviews
            .Where(r => r.ReviewedUser == user)
            .ToList();
    }
}