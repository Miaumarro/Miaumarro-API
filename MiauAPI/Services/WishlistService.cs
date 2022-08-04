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
/// Handles requests pertaining to wishlist.
/// </summary>
public sealed class WishlistService
{
    private readonly MiauDbContext _db;
    private readonly IRequestValidator<CreatedWishlistItemRequest> _validator;

    public WishlistService(MiauDbContext db, IRequestValidator<CreatedWishlistItemRequest> validator)
    {
        _db = db;
        _validator = validator;
    }

    /// <summary>
    /// Creates a new wishlist item.
    /// </summary>
    /// <param name="request">The controller request.</param>
    /// <param name="location">The URL of the new resource or the content of the Location header.</param>
    /// <returns>The result of the operation.</returns>
    /// <exception cref="ArgumentException">Occurs when <paramref name="location"/> is <see langword="null"/> or empty.</exception>
    public async Task<ActionResult<OneOf<CreatedWishlistItemResponse, ErrorResponse>>> CreateWishlistItemAsync(CreatedWishlistItemRequest request, string location)
    {
        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Location cannot be null or empty.", nameof(location));

        // Check if request contains valid data
        if (!_validator.IsRequestValid(request, out var errorMessages))
            return new BadRequestObjectResult(new ErrorResponse(errorMessages.ToArray()));

        // Create the database wishlist item
        var dbWishlistItem = new WishlistEntity()
        {
            User = _db.Users.First(x => x.Id == request.UserId),
            Product = _db.Products.First(x => x.Id == request.ProductId)
        };

        _db.Wishlist.Add(dbWishlistItem);
        await _db.SaveChangesAsync();

        // TODO: handle authentication properly
        return new CreatedResult(location, new CreatedPetResponse(dbWishlistItem.Id));
    }
}