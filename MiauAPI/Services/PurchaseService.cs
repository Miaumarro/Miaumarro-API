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
/// Handles requests pertaining to purchase.
/// </summary>
public sealed class PurchaseService
{
    private readonly MiauDbContext _db;

    public PurchaseService(MiauDbContext db)
    {
        _db = db;
    }

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
        var dbPurchaseRegiser = new PurchaseEntity()
        {
            User = await _db.Users.FirstAsync(x => x.Id == request.UserId),
            Coupon = await _db.Coupons.FirstOrDefaultAsync(x => x.Id == request.CouponId),
            Status = request.Status
        };
        var purchasedProducts = await CreatePurchasedProductsListAsync(request.PurchasedProductsId, dbPurchaseRegiser.Id);
        var dbPurchase = new PurchaseEntity()
        {
            Id = dbPurchaseRegiser.Id,
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



    /// <summary>
    /// Returns a list of purchased product.
    /// </summary>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<GetPurchaseResponse, ErrorResponse>>> GetPurchaseAsync(PurchaseParameters purchasesParameters)
    {
        if (purchasesParameters.UserId == 0)
        {
            return new NotFoundObjectResult("User NotFound!");
        }

        var dbPurchasesList = await _db.Purchases
            .Where(x => purchasesParameters.UserId == x.Id)
            .Select(x => new PurchaseObject
            {
                Id = x.Id,
                UserId = x.User.Id,
                CouponId = x.Coupon != null ? x.Coupon!.Id : null,
                Status = x.Status,
                PurchasedProduct = SetPurchasedProductObject(x.PurchasedProduct)
            }).ToListAsync();


        if (dbPurchasesList.Count == 0)
        {
            return new NotFoundObjectResult("No purchased product with the given paramenters were found.");
        }

        var dbPurchasedProductPaged = PagedList<PurchaseObject>.ToPagedList(
                        dbPurchasesList,
                        purchasesParameters.PageNumber,
                        purchasesParameters.PageSize);

        return new OkObjectResult(new GetPurchaseResponse(dbPurchasedProductPaged));
    }

    private List<PurchasedProductObject> SetPurchasedProductObject(List<PurchasedProductEntity> purchasedProductEntities)
    {
        var purchasedProductObject = new List<PurchasedProductObject>();
        purchasedProductEntities.ForEach(x => purchasedProductObject.Add(new PurchasedProductObject()
        {
            Id = x.Id,
            ProductId = x.Product.Id,
            PurchaseId = x.Purchase.Id,
            SalePrice = x.SalePrice.ToString()
        }));
        return purchasedProductObject;
    }

    /// <summary>
    /// Returns the purchased product with the given Id.
    /// </summary>
    /// <param name="purchasedProductId">The Id of the purchased product to be searched.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<GetPurchasedProductByIdResponse, ErrorResponse>>> GetPurchasedProductByIdAsync(int purchasedProductId)
    {

        var dbPurchasedProduct = await _db.PurchasedProducts.Where(p => p.Id == purchasedProductId)
                                            .Select(p => new PurchasedProductObject
                                            {
                                                Id = p.Id,
                                                PurchaseId = p.Purchase.User.Id,
                                                ProductId = p.Product.Id

                                            })
                                            .FirstOrDefaultAsync();



        return dbPurchasedProduct == null
                ? new NotFoundObjectResult(new ErrorResponse($"No purchasd product with the Id = {purchasedProductId} was found"))
                : new OkObjectResult(new GetPurchasedProductByIdResponse(dbPurchasedProduct));
    }

    /// <summary>
    /// Updates a purchase.
    /// </summary>
    /// <param name="request">The controller request.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<UpdateResponse, ErrorResponse>>> UpdatePurchaseByIdAsync(UpdatePurchaseRequest request)
    {

        var dbPurchase = await _db.Purchases.FindAsync(request.Id);

        if (dbPurchase == null)
        {
            return new NotFoundObjectResult(new ErrorResponse($"No pet with the Id = {request.Id} was found"));
        }

        // Checks the UserId
        if (request.UserId == 0)
        {
            return new BadRequestObjectResult(new ErrorResponse($"The pet must be related to a user. 'UserId = {request.UserId}'"));
        }

        var dbUser = await _db.Users.FirstOrDefaultAsync(x => x.Id == request.UserId);
        if (dbUser == null)
        {
            return new NotFoundObjectResult(new ErrorResponse($"No User with the Id = {request.UserId} was found"));
        }

        dbPurchase = new PurchaseEntity()
        {
            Id = request.Id,
            User = dbUser,
            Status = request.Status,

        };

        _db.Purchases.Update(dbPurchase);

        await _db.SaveChangesAsync();

        return new OkObjectResult(new UpdateResponse($"Successful update pet with the Id = {request.Id}"));

    }

}
