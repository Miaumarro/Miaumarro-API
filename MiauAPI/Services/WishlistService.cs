using MiauAPI.Models.QueryParameters;
using MiauAPI.Models.Requests;
using MiauAPI.Models.Responses;
using MiauAPI.Pagination;
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


namespace MiauAPI.Services;

/// <summary>
/// Handles requests pertaining to wish ist.
/// </summary>
public sealed class WishListService
{
    private readonly MiauDbContext _db;

    public WishListService(MiauDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Creates a new wish product.
    /// </summary>
    /// <param name="request">The controller request.</param>
    /// <param name="location">The URL of the new resource or the content of the Location header.</param>
    /// <returns>The result of the operation.</returns>
    /// <exception cref="ArgumentException">Occurs when <paramref name="location"/> is <see langword="null"/> or empty.</exception>
    public async Task<ActionResult<OneOf<CreatedWishListResponse, ErrorResponse>>> CreateWishListAsync(CreatedWishListRequest request, string location)
    {
        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Location cannot be null or empty.", nameof(location));

        // Checks the UserId
        var dbUser = await _db.Users.FirstOrDefaultAsync(x => x.Id == request.UserId);
        if (dbUser == null)
        {
            return new NotFoundObjectResult(new ErrorResponse($"No user with the Id = {request.ProductId} was found"));
        }

        // Checks the ProductId
        var dbProduct = await _db.Products.FirstOrDefaultAsync(x => x.Id == request.ProductId);
        if (dbProduct == null)
        {
            return new NotFoundObjectResult(new ErrorResponse($"No product with the Id = {request.ProductId} was found"));
        }

        // Create the database wish product
        var dbWishList = new WishListEntity()
        {
            User = dbUser,
            Product = dbProduct
        };

        _db.WishLists.Update(dbWishList);
        await _db.SaveChangesAsync();

        return new CreatedResult(location, new CreatedWishListResponse(dbWishList.Id));
    }

    /// <summary>
    /// Returns a list of wish products.
    /// </summary>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<GetWishListResponse, ErrorResponse>>> GetWishListAsync(WishListParameters wishListParameters)
    {
        var errorMessages = Enumerable.Empty<string>();
        var dbWishLists = _db.WishLists.Select(p => new WishListObject
        {
            Id = p.Id,
            UserId = p.User.Id,
            ProductId = p.Product.Id

        });

        if (wishListParameters.UserId != 0)
        {
            dbWishLists = dbWishLists.Where(p => p.UserId == wishListParameters.UserId);
        }

        if (wishListParameters.ProductId != 0)
        {
            dbWishLists = dbWishLists.Where(p => p.ProductId == wishListParameters.ProductId);
        }

        var dbWishListsList = await dbWishLists.ToListAsync();

        if (dbWishListsList.Count == 0)
        {
            return new NotFoundObjectResult("No wish product with the given paramenters were found.");
        }

        var dbWishListsPaged = PagedList<WishListObject>.ToPagedList(
                        dbWishListsList,
                        wishListParameters.PageNumber,
                        wishListParameters.PageSize);

        return new OkObjectResult(new GetWishListResponse(dbWishListsPaged));
    }


    /// <summary>
    /// Deletes the wish product with the given Id.
    /// </summary>
    /// <param name="wishListId">The Id of the wish product to be deleted.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<DeleteResponse, ErrorResponse>>> DeleteWishListByIdAsync(int wishListId)
    {
        return ((await _db.WishLists.DeleteAsync(p => p.Id == wishListId)) is 0)
            ? new NotFoundObjectResult(new ErrorResponse($"No wish product with the Id = {wishListId} was found"))
            : new OkObjectResult(new DeleteResponse($"Successful delete wish product with the Id = {wishListId}"));
    }



}
