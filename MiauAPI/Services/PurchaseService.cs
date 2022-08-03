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
/// Handles requests pertaining to purchase.
/// </summary>
public sealed class PurchaseService
{
    private readonly MiauDbContext _db;
    private readonly IRequestValidator<CreatedPurchaseRequest> _validator;

    public PurchaseService(MiauDbContext db, IRequestValidator<CreatedPurchaseRequest> validator)
    {
        _db = db;
        _validator = validator;
    }

    /// <summary>
    /// Creates a new purchase.
    /// </summary>
    /// <param name="request">The controller request.</param>
    /// <param name="location">The URL of the new resource or the content of the Location header.</param>
    /// <returns>The result of the operation.</returns>
    /// <exception cref="ArgumentException">Occurs when <paramref name="location"/> is <see langword="null"/> or empty.</exception>
    public async Task<ActionResult<OneOf<CreatedPurchaseResponse, ErrorResponse>>> CreatePetAsync(CreatedPurchaseRequest request, string location)
    {
        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Location cannot be null or empty.", nameof(location));

        // Check if request contains valid data
        if (!_validator.IsRequestValid(request, out var errorMessages))
            return new BadRequestObjectResult(new ErrorResponse(errorMessages.ToArray()));

        //Create the list of purchased products
        var purchasedProduct = new List<PurchasedProductEntity>();
        foreach(var product in request.PurchasedProductsId)
        {
            purchasedProduct.Add(_db.PurchasedProducts.First(x => x.Id == product));
        }

        // Create the database purchase
        var dbPurchase = new PurchaseEntity()
        {
            User = _db.Users.First(x => x.Id == request.UserId),
            Coupon = _db.Coupons.First(x => x.Id == request.CouponId),
            PurchasedProduct = purchasedProduct,
            Status = request.Status
        };

        _db.Purchases.Add(dbPurchase);
        await _db.SaveChangesAsync();

        // TODO: handle authentication properly
        return new CreatedResult(location, new CreatedPurchaseResponse(dbPurchase.Id));
    }
}
