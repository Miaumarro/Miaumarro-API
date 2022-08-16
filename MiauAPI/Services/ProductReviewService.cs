using MiauAPI.Models.Requests;
using MiauAPI.Models.Responses;
using MiauDatabase;
using MiauDatabase.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace MiauAPI.Services;

/// <summary>
/// Handles requests pertaining to product review.
/// </summary>
public sealed class ProductReviewService
{
    private readonly MiauDbContext _db;

    public ProductReviewService(MiauDbContext db)
        => _db = db;

    /// <summary>
    /// Creates a new product review.
    /// </summary>
    /// <param name="request">The controller request.</param>
    /// <param name="location">The URL of the new resource or the content of the Location header.</param>
    /// <returns>The result of the operation.</returns>
    /// <exception cref="ArgumentException">Occurs when <paramref name="location"/> is <see langword="null"/> or empty.</exception>
    public async Task<ActionResult<OneOf<CreatedProductReviewResponse, ErrorResponse>>> CreateProductReviewAsync(CreatedProductReviewRequest request, string location)
    {
        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Location cannot be null or empty.", nameof(location));

        // Create the database product review
        var dbProductReview = new ProductReviewEntity()
        {
            User = await _db.Users.FirstOrDefaultAsync(x => x.Id == request.UserId),
            Product = await _db.Products.FirstAsync(x => x.Id == request.ProductId),
            Description = request.Description,
            Score = request.Score
        };

        _db.ProductReviews.Add(dbProductReview);
        await _db.SaveChangesAsync();

        return new CreatedResult(location, new CreatedProductReviewResponse(dbProductReview.Id));
    }
}
