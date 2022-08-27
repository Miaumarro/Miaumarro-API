using MiauAPI.Models.QueryParameters;
using MiauAPI.Models.Requests;
using MiauAPI.Models.Responses;
using MiauAPI.Pagination;
using MiauAPI.Validators.Abstractions;
using MiauAPI.Extensions;
using MiauDatabase;
using MiauDatabase.Entities;
using Microsoft.AspNetCore.Mvc;
using OneOf;
using MiauAPI.Validators;
using LinqToDB;
using MiauDatabase.Enums;
using MiauAPI.Enums;
using MiauAPI.Models.QueryObjects;
using Microsoft.EntityFrameworkCore;
using LinqToDB.EntityFrameworkCore;
using OneOf.Types;

namespace MiauAPI.Services;

/// <summary>
/// Handles requests pertaining to reviews.
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
    /// Creates a new review.
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

        var reviewAlreadyExists = await _db.ProductReviews
            .Include(x => x.Product)
            .Include(x => x.User)
            .AnyAsyncEF(x => x.User != null && x.User.Id == request.UserId && x.Product.Id == request.ProductId);

        if (reviewAlreadyExists)
            return new BadRequestObjectResult(new ErrorResponse("There already is a review for this product by this user."));

        var dbUser = await _db.Users
            .AsTracking()
            .FirstOrDefaultAsyncEF(x => x.Id == request.UserId);

        var dbProduct = await _db.Products
            .AsTracking()
            .FirstOrDefaultAsyncEF(x => x.Id == request.ProductId);

        if (dbProduct is null)
            return new NotFoundObjectResult(new ErrorResponse($"No product with the Id {request.ProductId} was found"));

        // Create the database review
        var dbProductReview = new ProductReviewEntity()
        {
            User = dbUser,
            Product = dbProduct,
            Description = request.Description,
            Score = request.Score
        };

        _db.ProductReviews.Add(dbProductReview);
        await _db.SaveChangesAsync();

        return new CreatedResult(location, new CreatedProductReviewResponse(dbProductReview.Id));
    }

    /// <summary>
    /// Returns a list of Review.
    /// </summary>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<PagedResponse<ProductReviewObject[]>, None>>> GetProductReviewsAsync(ProductReviewParameters request)
    {
        var dbProductReviews = await _db.ProductReviews
            .Include(x => x.User)
            .Include(x => x.Product)
            .Where(x => x.Product.Id == request.ProductId)
            .OrderBy(x => x.Id)
            .Select(x => new ProductReviewObject(x.Id, x.Product.Id, (x.User == null) ? null : x.Id, x.Description, x.Score))
            .ToArrayAsyncEF();

        if (dbProductReviews.Length is 0)
            return new NotFoundResult();

        var remainingResultIds = await _db.ProductReviews
            .Include(x => x.User)
            .Include(x => x.Product)
            .Where(x => !dbProductReviews.Select(y => y.Id).Contains(x.Id) && x.Product.Id == request.ProductId)
            .OrderBy(x => x.Id)
            .Select(x => x.Id)
            .ToArrayAsyncEF();

        var previousAmount = remainingResultIds.Count(x => x < dbProductReviews[0].Id);
        var nextAmount = remainingResultIds.Count(x => x > dbProductReviews[^1].Id);

        return new OkObjectResult(PagedResponse.Create(request.PageNumber, request.PageSize, previousAmount, nextAmount, dbProductReviews.Length, dbProductReviews));
    }

    /// <summary>
    /// Return the product review with the given Id.
    /// </summary>
    /// <param name="productReview">The Id of the product review to be searched.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<ProductReviewObject, None>>> GetProductReviewByIdAsync(GetProductReviewRequest request)
    {
        var dbProductReview = await _db.ProductReviews
            .Include(x => x.User)
            .Include(x => x.Product)
            .Where(x => x.Product.Id == request.ProductId && ((x.User == null) ? null : x.User.Id) == request.UserId)
            .Select(x => new ProductReviewObject(x.Id, x.Product.Id, (x.User == null) ? null : x.Id, x.Description, x.Score))
            .FirstOrDefaultAsyncEF();

        return (dbProductReview is null)
            ? new NotFoundResult()
            : new OkObjectResult(dbProductReview);
    }

    /// <summary>
    /// Deletes the product review with the given Id.
    /// </summary>
    /// <param name="productReviewId">The Id of the product review to be deleted.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult> DeleteProductReviewByIdAsync(int productReviewId)
    {
        return ((await _db.ProductReviews.DeleteAsync(x => x.Id == productReviewId)) is 0)
            ? new NotFoundResult()
            : new OkResult();
    }

    /// <summary>
    /// Updates a product review.
    /// </summary>
    /// <param name="request">The controller request.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult> UpdateProductReviewByIdAsync(UpdateProductReviewRequest request)
    {
        var result = await _db.ProductReviews
            .Include(x => x.User)
            .Include(x => x.Product)
            .Where(x => x.Id == request.Id && x.User != null && x.User.Id == request.UserId && x.Product.Id == request.ProductId)
            .UpdateAsync(x => new ProductReviewEntity()
            {
                Description = request.NewDescription,
                Score = request.NewScore,
            });

        return (result is 0)
            ? new NotFoundResult()
            : new OkResult();
    }
}