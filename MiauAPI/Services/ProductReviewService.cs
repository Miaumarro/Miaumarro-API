//using MiauAPI.Models.QueryParameters;
//using MiauAPI.Models.Requests;
//using MiauAPI.Models.Responses;
//using MiauAPI.Pagination;
//using MiauAPI.Validators.Abstractions;
//using MiauDatabase;
//using MiauDatabase.Entities;
//using Microsoft.AspNetCore.Mvc;
//using OneOf;
//using MiauAPI.Validators;
//using LinqToDB;
//using MiauDatabase.Enums;
//using MiauAPI.Enums;
//using MiauAPI.Models.QueryObjects;

//namespace MiauAPI.Services;
using MiauAPI.Models.QueryParameters;
using MiauAPI.Models.Requests;
using MiauAPI.Models.Responses;
using MiauAPI.Pagination;
using MiauDatabase;
using MiauDatabase.Entities;
using Microsoft.AspNetCore.Mvc;
using OneOf;
using LinqToDB;
using MiauAPI.Models.QueryObjects;
using MiauAPI.Validators.Abstractions;

namespace MiauAPI.Services;

/// <summary>
/// Handles requests pertaining to reviews.
/// </summary>
public sealed class ProductReviewService
{
    private readonly MiauDbContext _db;
    private readonly IRequestValidator<CreatedProductReviewRequest> _validator;
    private readonly IRequestValidator<UpdateProductReviewRequest> _validatorUpdate;

    public ProductReviewService(MiauDbContext db, IRequestValidator<CreatedProductReviewRequest> validator, IRequestValidator<UpdateProductReviewRequest> validatorUpdate)
    {
        _db = db;
        _validator = validator;
        _validatorUpdate = validatorUpdate;
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

        // Checks the UserId
        if (request.UserId == 0)
            return new BadRequestObjectResult(new ErrorResponse($"The review must be related to a user. 'UserId = {request.UserId}'"));

        var dbUser = await _db.Users.FirstOrDefaultAsync(x => x.Id == request.UserId);
        if (dbUser == null)
        {
            return new NotFoundObjectResult(new ErrorResponse($"No User with the Id = {request.UserId} was found"));
        }

        // Checks the ProductId
        var dbProduct = await _db.Products.FirstOrDefaultAsync(x => x.Id == request.ProductId);
        if (dbProduct == null)
        {
            return new NotFoundObjectResult(new ErrorResponse($"No product with the Id = {request.ProductId} was found"));
        }


        // Create the database review
        var dbProductReview = new ProductReviewEntity()
        {
            User = dbUser,
            Product = dbProduct,
            Description = request.Description,
            Score = request.Score

        };

        _db.ProductReviews.Update(dbProductReview);
        await _db.SaveChangesAsync();

        return new CreatedResult(location, new CreatedProductReviewResponse(dbProductReview.Id));
    }

    /// <summary>
    /// Returns a list of Review.
    /// </summary>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<GetProductReviewResponse, ErrorResponse>>> GetProductReviewAsync(ProductReviewParameters productReviewParameters)
    {

        var dbProductReview = _db.ProductReviews.Select(p => new ProductReviewObject
        {

            Id = p.Id,
            UserId = p.User.Id,
            ProductId = p.Product.Id,
            Description = p.Description,
            Score = p.Score

        });

        if (productReviewParameters.UserId != 0)
        {
            dbProductReview = dbProductReview.Where(p => p.UserId == productReviewParameters.UserId);
        }

        var dbProductReviewList = await dbProductReview.ToListAsync();

        if (dbProductReviewList.Count == 0)
        {
            return new NotFoundObjectResult("No review with the given paramenters were found.");
        }

        var dbProductReviewPaged = PagedList<ProductReviewObject>.ToPagedList(
                        dbProductReviewList,
                        productReviewParameters.PageNumber,
                        productReviewParameters.PageSize);

        return new OkObjectResult(new GetProductReviewResponse(dbProductReviewPaged));
    }

    /// <summary>
    /// Return the product review with the given Id.
    /// </summary>
    /// <param name="productReview">The Id of the product review to be searched.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<GetProductReviewByIdResponse, ErrorResponse>>> GetProductReviewByIdAsync(int productReviewId)
    {

        var dbProductReview = await _db.ProductReviews.Where(p => p.Id == productReviewId)
                                            .Select(p => new ProductReviewObject
                                            {
                                                Id = p.Id,
                                                UserId = p.User.Id,
                                                ProductId = p.Product.Id,
                                                Description = p.Description,
                                                Score = p.Score
                                            })
                                            .FirstOrDefaultAsync();

        return dbProductReview == null
                ? new NotFoundObjectResult(new ErrorResponse($"No product review with the Id = {productReviewId} was found"))
                : new OkObjectResult(new GetProductReviewByIdResponse(dbProductReview));
    }

    /// <summary>
    /// Deletes the product review with the given Id.
    /// </summary>
    /// <param name="productReviewId">The Id of the product review to be deleted.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<DeleteResponse, ErrorResponse>>> DeleteProductReviewByIdAsync(int productReviewId)
    {
        return ((await _db.ProductReviews.DeleteAsync(p => p.Id == productReviewId)) is 0)
            ? new NotFoundObjectResult(new ErrorResponse($"No productr review with the Id = {productReviewId} was found"))
            : new OkObjectResult(new DeleteResponse($"Successful delete review with the Id = {productReviewId}"));
    }

    /// <summary>
    /// Updates a product review.
    /// </summary>
    /// <param name="request">The controller request.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<UpdateResponse, ErrorResponse>>> UpdateProductReviewByIdAsync(UpdateProductReviewRequest request)
    {
        // Check if request contains valid data
        if (!_validatorUpdate.IsRequestValid(request, out var errorMessages))
            return new BadRequestObjectResult(new ErrorResponse(errorMessages.ToArray()));

        var dbProductReview = await _db.ProductReviews.FindAsync(request.Id);

        if (dbProductReview == null)
        {
            return new NotFoundObjectResult(new ErrorResponse($"No review with the Id = {request.Id} was found"));
        }

        // Checks the UserId
        if (request.UserId == 0)
            return new BadRequestObjectResult(new ErrorResponse($"The product review must be related to a user. 'UserId = {request.UserId}'"));

        var dbUser = await _db.Users.FirstOrDefaultAsync(x => x.Id == request.UserId);
        if (dbUser == null)
        {
            return new NotFoundObjectResult(new ErrorResponse($"No User with the Id = {request.UserId} was found"));
        }

        // Checks the ProductId
        var dbProduct = await _db.Products.FirstOrDefaultAsync(x => x.Id == request.ProductId);
        if (dbProduct == null)
        {
            return new NotFoundObjectResult(new ErrorResponse($"No product with the Id = {request.ProductId} was found"));
        }

        dbProductReview = new ProductReviewEntity()
        {
            Id = request.Id,
            User = dbUser,
            Product = dbProduct,
            Description = request.Description,
            Score = request.Score

        };

        _db.ProductReviews.Update(dbProductReview);

        await _db.SaveChangesAsync();

        return new OkObjectResult(new UpdateResponse($"Successfull update product review with the Id = {request.Id}"));

    }
}
