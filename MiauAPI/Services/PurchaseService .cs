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
public sealed class PurchaseService
{
    private readonly MiauDbContext _db;
    private readonly IRequestValidator<CreatedPurchaseRequest> _validator;
    private readonly IRequestValidator<UpdatePurchaseRequest> _validatorUpdate;

    public PurchaseService(MiauDbContext db, IRequestValidator<CreatedPurchaseRequest> validator, IRequestValidator<UpdatePurchaseRequest> validatorUpdate)
    {
        _db = db;
        _validator = validator;
        _validatorUpdate = validatorUpdate; ///////////////////////////////////////////////////////////////////
    }

    /// <summary>
    /// Creates a new wish product.
    /// </summary>
    /// <param name="request">The controller request.</param>
    /// <param name="location">The URL of the new resource or the content of the Location header.</param>
    /// <returns>The result of the operation.</returns>
    /// <exception cref="ArgumentException">Occurs when <paramref name="location"/> is <see langword="null"/> or empty.</exception>
    public async Task<ActionResult<OneOf<CreatedPurchaseResponse, ErrorResponse>>> CreatePurchaseAsync(CreatedPurchaseRequest request, string location)
    {
        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Location cannot be null or empty.", nameof(location));

        // Check if request contains valid data
        if (!_validator.IsRequestValid(request, out var errorMessages))
            return new BadRequestObjectResult(new ErrorResponse(errorMessages.ToArray()));

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
        var dbPurchase = new PurchaseEntity()
        {
            User = dbUser,
            Product = dbProduct
        };

        _db.Purchases.Update(dbPurchase);
        await _db.SaveChangesAsync();

        return new CreatedResult(location, new CreatedPurchaseResponse(dbPurchase.Id));
    }

    /// <summary>
    /// Returns a list of wish products.
    /// </summary>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<GetPurchaseResponse, ErrorResponse>>> GetPurchaseAsync(PurchaseParameters purchaseParameters)
    {

        var dbPurchases = _db.Purchases.Select(p => new PurchaseObject
        {
            Id = p.Id,
            UserId = p.User.Id,
            ProductId = p.Product.Id

        });

        if (purchaseParameters.UserId != 0)
        {
            dbPurchases = dbPurchases.Where(p => p.UserId == purchaseParameters.UserId);
        }

        var dbPurchasesList = await dbPurchases.ToListAsync();

        if (dbPurchasesList.Count == 0)
        {
            return new NotFoundObjectResult("No wish product with the given paramenters were found.");
        }

        var dbPurchasesPaged = PagedList<PurchaseObject>.ToPagedList(
                        dbPurchasesList,
                        purchaseParameters.PageNumber,
                        purchaseParameters.PageSize);

        return new OkObjectResult(new GetPurchaseResponse(dbPurchasesPaged));
    }

    /// <summary>
    /// Returns the wish product with the given Id.
    /// </summary>
    /// <param name="wishProductId">The Id of the product to be searched.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<GetPurchaseByIdResponse, ErrorResponse>>> GetPurchaseByIdAsync(int purchaseId)
    {

        var dbPurchase = await _db.Purchases.Where(p => p.Id == purchaseId)
                                            .Select(p => new PurchaseObject
                                            {
                                                Id = p.Id,
                                                UserId = p.User.Id,
                                                ProductId = p.Product.Id

                                            })
                                            .FirstOrDefaultAsync();

        return dbPurchase == null
                ? new NotFoundObjectResult(new ErrorResponse($"No product with the Id = {purchaseId} was found"))
                : new OkObjectResult(new GetPurchaseByIdResponse(dbPurchase));
    }
    /// <summary>
    /// Deletes the wish product with the given Id.
    /// </summary>
    /// <param name="purchaseId">The Id of the wish product to be deleted.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<DeleteResponse, ErrorResponse>>> DeletePurchaseByIdAsync(int purchaseId)
    {
        return ((await _db.Purchases.DeleteAsync(p => p.Id == purchaseId)) is 0)
            ? new NotFoundObjectResult(new ErrorResponse($"No wish product with the Id = {purchaseId} was found"))
            : new OkObjectResult(new DeleteResponse($"Successful delete wish product with the Id = {purchaseId}"));
    }

    

}
