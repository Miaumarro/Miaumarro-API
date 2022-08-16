using MiauAPI.Models.Requests;
using MiauAPI.Models.Responses;
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

    public WishlistService(MiauDbContext db)
        => _db = db;

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

        // Create the database wishlist item
        var dbWishlistItem = new WishlistEntity()
        {
            User = await _db.Users.FirstAsync(x => x.Id == request.UserId),
            Product = await _db.Products.FirstAsync(x => x.Id == request.ProductId)
        };

        _db.Wishlist.Add(dbWishlistItem);
        await _db.SaveChangesAsync();

        return new CreatedResult(location, new CreatedPetResponse(dbWishlistItem.Id));
    }
}