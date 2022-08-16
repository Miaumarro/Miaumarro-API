using MiauAPI.Models.Requests;
using MiauAPI.Models.Responses;
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

    public PurchaseService(MiauDbContext db)
        => _db = db;

    /// <summary>
    /// Creates a new purchase.
    /// </summary>
    /// <param name="request">The controller request.</param>
    /// <param name="location">The URL of the new resource or the content of the Location header.</param>
    /// <returns>The result of the operation.</returns>
    /// <exception cref="ArgumentException">Occurs when <paramref name="location"/> is <see langword="null"/> or empty.</exception>
    public async Task<ActionResult<OneOf<CreatedPurchaseResponse, ErrorResponse>>> CreatePurchaseAsync(CreatedPurchaseRequest request, string location)
    {
        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Location cannot be null or empty.", nameof(location));

        // Create the database purchase
        var dbPurchaseTemp = new PurchaseEntity()
        {
            User = await _db.Users.FirstAsync(x => x.Id == request.UserId),
            Coupon = await _db.Coupons.FirstOrDefaultAsync(x => x.Id == request.CouponId),
            Status = request.Status
        };
        var purchasedProducts = await CreatePurchasedProductsListAsync(request.ProductsId, dbPurchaseTemp.Id);
        var dbPurchase = new PurchaseEntity()
        {
            Id = dbPurchaseTemp.Id,
            PurchasedProduct = purchasedProducts
        };

        _db.Purchases.Add(dbPurchase);
        await _db.SaveChangesAsync();

        return new CreatedResult(location, new CreatedPurchaseResponse(dbPurchase.Id));
    }

    private async Task<List<PurchasedProductEntity>> CreatePurchasedProductsListAsync(List<int> productsId, int purchaseId)
    {
        var purchasedProducts = await _db.Products
            .Where(x => productsId.Contains(x.Id))
            .Select(x => new PurchasedProductEntity()
            {
                Product = x,
                Purchase = _db.Purchases.First(x => x.Id == purchaseId),
                SalePrice = x.Price - (x.Price * x.Discount)
            })
            .ToListAsync();

        _db.PurchasedProducts.AddRange(purchasedProducts);
        await _db.SaveChangesAsync();

        return purchasedProducts;
    }
}
