using MiauAPI.Models.QueryParameters;
using MiauAPI.Models.Requests;
using MiauAPI.Models.Responses;
using MiauAPI.Validators.Abstractions;
using MiauDatabase;
using MiauDatabase.Entities;
using Microsoft.AspNetCore.Mvc;
using OneOf;
using MiauAPI.Validators;
using LinqToDB;
using MiauDatabase.Enums;
using MiauAPI.Enums;
using MiauAPI.Models.QueryObjects;
using MiauAPI.Extensions;
using Microsoft.EntityFrameworkCore;
using LinqToDB.EntityFrameworkCore;
using System.Linq;
using OneOf.Types;

namespace MiauAPI.Services;

/// <summary>
/// Handles requests pertaining to wish ist.
/// </summary>
public sealed class WishlistService
{
    private readonly MiauDbContext _db;

    public WishlistService(MiauDbContext db)
        => _db = db;

    /// <summary>
    /// Creates a new wish product.
    /// </summary>
    /// <param name="request">The controller's request.</param>
    /// <param name="location">The URL of the new resource or the content of the Location header.</param>
    /// <returns>The result of the operation.</returns>
    /// <exception cref="ArgumentException">Occurs when <paramref name="location"/> is <see langword="null"/> or empty.</exception>
    public async Task<ActionResult<OneOf<CreatedWishlistResponse, ErrorResponse>>> CreateWishlistAsync(CreatedWishlistRequest request, string location)
    {
        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Location cannot be null or empty.", nameof(location));

        var wishlistItemExists = await _db.Wishlists
            .Include(x => x.User)
            .Include(x => x.Product)
            .AnyAsyncEF(x => x.User.Id == request.UserId && x.Product.Id == request.ProductId);

        if (wishlistItemExists)
            return new BadRequestObjectResult(new ErrorResponse("This product has already been wishlisted."));

        // Checks the UserId
        var dbUser = await _db.Users
            .AsTracking()
            .FirstOrDefaultAsyncEF(x => x.Id == request.UserId);

        if (dbUser is null)
            return new NotFoundResult();

        // Checks the ProductId
        var dbProduct = await _db.Products
            .AsTracking()
            .FirstOrDefaultAsyncEF(x => x.Id == request.ProductId);

        if (dbProduct is null)
            return new NotFoundResult();

        // Create the database wish product
        var dbWishList = new WishlistEntity()
        {
            User = dbUser,
            Product = dbProduct
        };

        _db.Wishlists.Add(dbWishList);
        await _db.SaveChangesAsync();

        return new CreatedResult(location, new CreatedWishlistResponse(dbWishList.Id));
    }

    /// <summary>
    /// Returns a list of wish products.
    /// </summary>
    /// <param name="request">The controller's request.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<PagedResponse<ProductObject[]>, None>>> GetWishlistAsync(WishlistParameters request)
    {
        var dbProductsQuery = _db.Wishlists
            .Include(x => x.User)
            .Include(x => x.Product)
            .Where(x => x.User.Id == request.UserId)
            .OrderBy(x => x.Product.Id);

        var dbProducts = await dbProductsQuery
            .PageRange(request.PageNumber, request.PageSize)
            .Select(x => new ProductObject(
                    x.Product.Id,
                    x.Product.Name,
                    x.Product.Description,
                    x.Product.Brand,
                    x.Product.Price,
                    x.Product.IsActive,
                    x.Product.Amount,
                    x.Product.Tags,
                    x.Product.Discount
                )
            )
            .ToArrayAsyncEF();

        if (dbProducts.Length is 0)
            return new NotFoundResult();

        var remainingResultIds = await dbProductsQuery
            .Where(x => !dbProducts.Select(y => y.Id).Contains(x.Product.Id))
            .Select(x => x.Product.Id)
            .ToArrayAsyncEF();

        var previousAmount = remainingResultIds.Count(x => x < dbProducts[0].Id);
        var nextAmount = remainingResultIds.Count(x => x > dbProducts[^1].Id);

        return new OkObjectResult(PagedResponse.Create(request.PageNumber, request.PageSize, previousAmount, nextAmount, dbProducts.Length, dbProducts));
    }

    /// <summary>
    /// Deletes the wish product with the given Id.
    /// </summary>
    /// <param name="request">The controller's request.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult> DeleteWishlistAsync(DeleteWishlistRequest request)
    {
        var wishlistId = await _db.Wishlists
            .Include(x => x.User)
            .Include(x => x.Product)
            .Where(x => x.User.Id == request.UserId && x.Product.Id == request.ProductId)
            .Select(x => x.Id)
            .FirstOrDefaultAsyncEF();

        if (wishlistId is 0)
            return new NotFoundResult();

        await _db.Wishlists.DeleteAsync(x => x.Id == wishlistId);

        return new OkResult();
    }
}