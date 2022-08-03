using MiauAPI.Models.Requests;
using MiauAPI.Models.Responses;
using MiauAPI.Validators.Abstractions;
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
    private readonly IRequestValidator<CreatedProductReviewRequest> _validator;

    public ProductReviewService(MiauDbContext db, IRequestValidator<CreatedProductReviewRequest> validator)
    {
        _db = db;
        _validator = validator;
    }

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

        // Check if request contains valid data
        if (!_validator.IsRequestValid(request, out var errorMessages))
            return new BadRequestObjectResult(new ErrorResponse(errorMessages.ToArray()));

        // Create the database pet
        var dbProductReview = new ProductReviewEntity()
        {
            User = _db.Users.First(x => x.Id == request.UserId),
            Product = _db.Products.First(x => x.Id == request.ProductId),
            Description = request.Description,
            Score = request.Score
        };

        _db.ProductReviews.Add(dbProductReview);
        await _db.SaveChangesAsync();

        // TODO: handle authentication properly
        return new CreatedResult(location, new CreatedPetResponse(dbProductReview.Id));
    }
}
