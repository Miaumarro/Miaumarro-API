using MiauAPI.Models.QueryParameters;
using MiauAPI.Models.Requests;
using MiauAPI.Models.Responses;
using MiauAPI.Validators.Abstractions;
using MiauDatabase;
using MiauDatabase.Entities;
using Microsoft.AspNetCore.Mvc;
using OneOf;
using LinqToDB;
using MiauAPI.Enums;
using MiauAPI.Models.QueryObjects;
using MiauAPI.Extensions;
using OneOf.Types;
using Kotz.Extensions;

namespace MiauAPI.Services;

/// <summary>
/// Handles requests pertaining to products.
/// </summary>
public sealed class ProductService
{
    private readonly MiauDbContext _db;
    private readonly IRequestValidator<CreatedProductRequest> _validator;
    private readonly IRequestValidator<ProductParameters> _productParamValidator;
    private readonly IRequestValidator<UpdateProductRequest> _validatorUpdate;

    public ProductService(MiauDbContext db, IRequestValidator<CreatedProductRequest> validator, IRequestValidator<ProductParameters> productParamValidator, IRequestValidator<UpdateProductRequest> validatorUpdate)
    {
        _db = db;
        _validator = validator;
        _productParamValidator = productParamValidator;
        _validatorUpdate = validatorUpdate;
    }

    /// <summary>
    /// Returns a list of products.
    /// </summary>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<PagedResponse<ProductObject[]>, ErrorResponse, None>>> GetProductsAsync(ProductParameters request)
    {
        if (!_productParamValidator.IsRequestValid(request, out var errorMessages))
            return new BadRequestObjectResult(new ErrorResponse(errorMessages.ToArray()));

        // Build the query accoding to the request parameters
        var productsQuery = _db.Products.AsQueryable();

        if (request.SearchedTerm is not null)
            productsQuery = productsQuery.Where(x => x.Name.Contains(request.SearchedTerm));

        if (request.Brand is not null)
            productsQuery = productsQuery.Where(x => request.Brand == x.Brand);

        if (request.MaxPrice is not 0 || request.MinPrice is not 0)
            productsQuery = productsQuery.Where(x => ((x.Price * (1 - x.Discount)) >= request.MinPrice) && ((x.Price * (1 - x.Discount)) <= request.MaxPrice));

        if (request.ActiveDiscount)
            productsQuery = productsQuery.Where(x => x.Discount > 0);

        if (request.Tags is not 0)
            productsQuery = productsQuery.Where(x => x.Tags.HasFlag(request.Tags));

        productsQuery = Sort(productsQuery, request.SortParameter);

        // Get the products from the database
        var dbProducts = await productsQuery
            .Select(x => new ProductObject(x.Id, x.Name, x.Description, x.Brand, x.Price, x.IsActive, x.Amount, x.Tags, x.Discount))
            .PageRange(request.PageNumber, request.PageSize)
            .ToArrayAsync();

        if (dbProducts.Length is 0)
            return new NotFoundResult();

        // Get all eligible product ids
        var excludedProductIds = await productsQuery
            .Select(x => x.Id)
            .ToArrayAsync();

        // Get how many products exist before and after the page selection
        var previousAmount = excludedProductIds.AsSpan()[..excludedProductIds.IndexOf(dbProducts[0].Id)].Length;
        var nextAmount = excludedProductIds.AsSpan()[(excludedProductIds.IndexOf(dbProducts[^1].Id) + 1)..].Length;

        return new OkObjectResult(PagedResponse.Create(request.PageNumber, request.PageSize, previousAmount, nextAmount, dbProducts.Length, dbProducts));
    }

    /// <summary>
    /// Returns the product with the given Id.
    /// </summary>
    /// <param name="productId">The Id of the product to be searched.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<ProductObject, ErrorResponse>>> GetProductByIdAsync(int productId)
    {
        var dbProduct = await _db.Products
            .Where(x => x.Id == productId)
            .Select(x => new ProductObject(x.Id, x.Name, x.Description, x.Brand, x.Price, x.IsActive, x.Amount, x.Tags, x.Discount))
            .FirstOrDefaultAsync();

        return (dbProduct == null)
            ? new NotFoundObjectResult(new ErrorResponse($"No product with the Id = {productId} was found"))
            : new OkObjectResult(dbProduct);
    }

    /// <summary>
    /// Creates a new product.
    /// </summary>
    /// <param name="request">The controller request.</param>
    /// <param name="location">The URL of the new resource or the content of the Location header.</param>
    /// <returns>The result of the operation.</returns>
    /// <exception cref="ArgumentException">Occurs when <paramref name="location"/> is <see langword="null"/> or empty.</exception>
    public async Task<ActionResult<OneOf<CreatedProductResponse, ErrorResponse>>> CreateProductAsync(CreatedProductRequest request, string location)
    {
        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Location cannot be null or empty.", nameof(location));

        // Check if request contains valid data
        if (!_validator.IsRequestValid(request, out var errorMessages))
            return new BadRequestObjectResult(new ErrorResponse(errorMessages.ToArray()));

        // Create the database user
        var dbProduct = new ProductEntity()
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            IsActive = request.IsActive,
            Amount = request.Amount,
            Tags = request.Tags,
            Brand = request.Brand,
            Discount = request.Discount
        };

        _db.Products.Add(dbProduct);
        await _db.SaveChangesAsync();

        return new CreatedResult(location, new CreatedProductResponse(dbProduct.Id));
    }

    /// <summary>
    /// Deletes the product with the given Id.
    /// </summary>
    /// <param name="productId">The Id of the product to be deleted.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult> DeleteProductByIdAsync(int productId)
    {
        return ((await _db.Products.DeleteAsync(p => p.Id == productId)) is 0)
            ? new NotFoundResult()
            : new OkResult();
    }

    /// <summary>
    /// Updates the product with the given Id.
    /// </summary>
    /// <param name="id">The Id of the product to be updated.</param>
    /// <param name="product">The product object with the parameters to be updated.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult> UpdateProductByIdAsync(UpdateProductRequest request)
    {
        // Check if request contains valid data
        if (!_validatorUpdate.IsRequestValid(request, out var errorMessages))
            return new BadRequestObjectResult(new ErrorResponse(errorMessages.ToArray()));

        var result = await _db.Products
            .Where(x => x.Id == request.Id)
            .UpdateAsync(x => new ProductEntity()
            {
                Name = request.Name,
                Description = request.Description,
                Brand = request.Brand,
                Price = request.Price,
                IsActive = request.IsActive,
                Amount = request.Amount,
                Tags = request.Tags,
                Discount = request.Discount
            });

        return (result is 0)
            ? new NotFoundResult()
            : new OkResult();
    }

    /// <summary>
    /// Sorts products by a given parameter.
    /// </summary>
    /// <returns>The query of the sorted product list.</returns>
    /// <exception cref="NotSupportedException">Occurs when <paramref name="sortOrder"/> is not implemented.</exception>
    public IQueryable<ProductEntity> Sort(IQueryable<ProductEntity> products, SortParameter sortOrder)
    {
        return sortOrder switch
        {
            SortParameter.PriceAsc => products.OrderByDescending(p => p.IsActive).ThenBy(p => (double)p.Price * (1 - (double)p.Discount)),
            SortParameter.PriceDesc => products.OrderByDescending(p => p.IsActive).ThenByDescending(p => (double)p.Price * (1 - (double)p.Discount)),
            SortParameter.DiscountAsc => products.OrderByDescending(p => p.IsActive).ThenBy(p => (double)p.Discount),
            SortParameter.DiscountDesc => products.OrderByDescending(p => p.IsActive).ThenByDescending(p => (double)p.Discount),
            _ => throw new NotSupportedException($"Sort parameter of type {sortOrder} is not supported.")
        };
    }
}
