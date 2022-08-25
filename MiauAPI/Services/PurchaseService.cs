using LinqToDB;
using LinqToDB.EntityFrameworkCore;
using MiauAPI.Extensions;
using MiauAPI.Models.QueryObjects;
using MiauAPI.Models.QueryParameters;
using MiauAPI.Models.Requests;
using MiauAPI.Models.Responses;
using MiauDatabase;
using MiauDatabase.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

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
            User = await _db.Users.FirstAsyncEF(x => x.Id == request.UserId),
            Coupon = await _db.Coupons.FirstOrDefaultAsyncEF(x => x.Id == request.CouponId),
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

    /// <summary>
    /// Gets all purchases made by the specified user.
    /// </summary>
    /// <param name="request">The controller request.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<PagedResponse<PurchaseObject[]>, None>>> GetPurchasesAsync(PurchaseParameters request)
    {
        var dbPurchases = await _db.Purchases
            .Include(x => x.User)
            .Include(x => x.Coupon)
            .Include(x => x.PurchasedProduct)
            .Where(x => x.User.Id == request.UserId)
            .OrderBy(x => x.Id)
            .PageRange(request.PageNumber, request.PageSize)
            .Select(x => new PurchaseObject(
                    x.Id,
                    x.Status,
                    x.DateAdded,
                    x.PurchasedProduct.Select(x => new ProductObject(x.Product.Id, x.Product.Name, x.Product.Description, x.Product.Brand, x.Product.Price, x.Product.IsActive, x.Product.Amount, x.Product.Tags, x.Product.Discount)).ToArray(),
                    (x.Coupon == null) ? null : new CouponObject(x.Coupon.Id, x.Coupon.Coupon, x.Coupon.IsActive, x.Coupon.Discount)
                )
            )
            .ToArrayAsyncEF();

        if (dbPurchases.Length is 0)
            return new NotFoundResult();

        var remainingResultIds = await _db.Purchases
            .Where(x => !dbPurchases.Select(y => y.Id).Contains(x.Id))
            .Select(x => x.Id)
            .ToArrayAsyncEF();

        var previousAmount = remainingResultIds.Count(x => x < dbPurchases[0].Id);
        var nextAmount = remainingResultIds.Count(x => x > dbPurchases[^1].Id);

        return new OkObjectResult(PagedResponse.Create(request.PageNumber, request.PageSize, previousAmount, nextAmount, dbPurchases.Length, dbPurchases));
    }

    /// <summary>
    /// Updates a purchase under the specified user.
    /// </summary>
    /// <param name="request">The controller request.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult> UpdatePurchaseAsync(UpdatePurchaseRequest request)
    {
        var result = await _db.Purchases
            .Include(x => x.User)
            .Where(x => x.Id == request.Id && x.User.Id == request.UserId)
            .UpdateAsync(x => new PurchaseEntity() { Status = request.Status });

        return (result is 0)
            ? new NotFoundResult()
            : new OkResult();
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
            .ToListAsyncEF();

        _db.PurchasedProducts.AddRange(purchasedProducts);
        await _db.SaveChangesAsync();

        return purchasedProducts;
    }
}
