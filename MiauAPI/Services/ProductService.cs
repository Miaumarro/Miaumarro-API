using MiauAPI.Models.Parameters;
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

namespace MiauAPI.Services;

/// <summary>
/// Handles requests pertaining to products.
/// </summary>
public sealed class ProductService
{
    private readonly MiauDbContext _db;
    private readonly IRequestValidator<CreatedProductRequest> _validatorCreatedProduct;

    public ProductService(MiauDbContext db, IRequestValidator<CreatedProductRequest> validatorCreatedProduct)
    {
        _db = db;
        _validatorCreatedProduct = validatorCreatedProduct;
    }

    /// <summary>
    /// Returns a list of products.
    /// </summary>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<GetProductResponse, ErrorResponse>>> GetProductAsync(ProductParameters productParameters)
    {

        var errorMessages = Enumerable.Empty<string>();
        var dbProducts = _db.Products.Select(p => p);

        if (productParameters.SearchedTerm is not null)
        {
            dbProducts = SearchByTerm(dbProducts, productParameters.SearchedTerm, out var descriptionError);
            if (descriptionError != string.Empty)
                return new NotFoundObjectResult(descriptionError);
        }

        if (productParameters.Brand is not null)
        {
            dbProducts = SearchByBrand(dbProducts, productParameters.Brand, out var descriptionError);
            if (descriptionError != string.Empty)
                return new NotFoundObjectResult(descriptionError);
        }

        if (productParameters.MaxPrice != 0 || productParameters.MinPrice != 0)
        {
            dbProducts = SearchByPriceRange(dbProducts, productParameters.MinPrice, productParameters.MaxPrice, out var descriptionError);
            if (descriptionError != string.Empty)
                return new BadRequestObjectResult(descriptionError);
        }

        if (productParameters.ActiveDiscount)
        {
            dbProducts = SearchByActiveDiscount(dbProducts, out var descriptionError);
            if (descriptionError != string.Empty)
                return new NotFoundObjectResult(descriptionError);
        }

        if (productParameters.Tags != 0)
        {
            dbProducts = SearchByTags(dbProducts, productParameters.Tags, out var descriptionError);
            if (descriptionError != string.Empty)
                return new NotFoundObjectResult(descriptionError);
        }

        dbProducts = Sort(dbProducts, productParameters.SortParameter);

        var dbProductsList = await dbProducts
                            .ToListAsync();

        var dbProductsPaged = PagedList<ProductEntity>.ToPagedList(
                        dbProductsList,
                        productParameters.PageNumber,
                        productParameters.PageSize);

        return new OkObjectResult(new GetProductResponse(dbProductsPaged));

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
        if (!_validatorCreatedProduct.IsRequestValid(request, out var errorMessages))
            return new BadRequestObjectResult(new ErrorResponse(errorMessages.ToArray()));

        // Create the database user
        var dbProduct = new ProductEntity()
        {
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

        // TODO: handle authentication properly
        return new CreatedResult(location, new CreatedProductResponse(dbProduct.Id));
    }

    /// <summary>
    /// Lists products with an specific term in the description
    /// </summary>
    /// <returns>The result of the operation and a description error.</returns>
    public IQueryable<ProductEntity> SearchByTerm(IQueryable<ProductEntity> products, string description, out string errorMessages)
    {
        products = products.Where(p => p.Description.ToLower().Contains(description.ToLower()));
        errorMessages = !products.Any()
                    ? $"No products with the term '{description}' were found."
                    : String.Empty;
        return products;
    }

    /// <summary>
    /// Lists products with an specific brand
    /// </summary>
    /// <returns>The result of the operation and a description error.</returns>
    public IQueryable<ProductEntity> SearchByBrand(IQueryable<ProductEntity> products, string brand, out string errorMessages)
    {
        products = products.Where(p => p.Brand == brand.ToLower());
        errorMessages = !products.Any()
                    ? $"No products with the brand '{brand}' were found."
                    : String.Empty;
        return products;
    }

    /// <summary>
    /// Lists products within an specific price range
    /// </summary>
    /// <returns>The result of the operation and a description error.</returns>
    public IQueryable<ProductEntity> SearchByPriceRange(IQueryable<ProductEntity> products, decimal minPrice, decimal maxPrice, out string errorMessages)
    {
        if (maxPrice == 0 && minPrice != 0)
        {
            var mostExpensiveProduct = products
                                        .Where(p => (p.IsActive))
                                        .OrderByDescending(p => (double)p.Price * (1 - (double)p.Discount))
                                        .First();
            maxPrice = mostExpensiveProduct.Price;
        }

        if (!Validate.IsPositive(minPrice, nameof(minPrice), out var messageError)
            || !Validate.IsPositive(maxPrice, nameof(maxPrice), out messageError)
            || !Validate.IsValidRange<decimal>(minPrice, maxPrice, nameof(minPrice), nameof(maxPrice), out messageError))
        {
            errorMessages = messageError;
            return products;
        }
        else
        {
            products = products
                            .Where(p => ((p.Price * (1 - p.Discount)) >= minPrice)
                                     && ((p.Price * (1 - p.Discount)) <= maxPrice));
            errorMessages = !products.Any()
                        ? "No products were found."
                        : String.Empty;
            return products;
        }
    }

    /// <summary>
    /// Lists products with an active discount
    /// </summary>
    /// <returns>The result of the operation and a description error.</returns>
    public IQueryable<ProductEntity> SearchByActiveDiscount(IQueryable<ProductEntity> products, out string errorMessages)
    {
        products = products.Where(p => p.Discount > 0);
        errorMessages = !products.Any()
                    ? "No products were found."
                    : String.Empty;
        return products;
    }

    /// <summary>
    /// Lists products with specifics tags
    /// </summary>
    /// <returns>The result of the operation and a description error.</returns>
    public IQueryable<ProductEntity> SearchByTags(IQueryable<ProductEntity> products, ProductTag tags, out string errorMessages)
    {
        products = products.Where(p => p.Tags.HasFlag(tags));
        errorMessages = !products.Any()
                    ? "No products were found."
                    : String.Empty;
        return products;
    }

    /// <summary>
    /// Sorts products by a given parameter (Ascending Price as default, Descending Price, Ascending Discount, Descending Discount)
    /// </summary>
    /// <returns>The sorted product list.</returns>
    public IQueryable<ProductEntity> Sort(IQueryable<ProductEntity> products, Enum sortOrder)
    {
        switch (sortOrder)
        {
            case SortParameter.PriceAsc:
                products = products
                        .OrderByDescending(p => p.IsActive)
                        .ThenBy(p => ((double)p.Price * (1 - (double)p.Discount)));
                break;
            case SortParameter.PriceDesc:
                products = products
                            .OrderByDescending(p => p.IsActive)
                            .ThenByDescending(p => ((double)p.Price * (1 - (double)p.Discount)));
                break;
            case SortParameter.DiscountAsc:
                products = products
                            .OrderByDescending(p => p.IsActive)
                            .ThenBy(p => (double)p.Discount);
                break;
            case SortParameter.DiscountDesc:
                products = products
                            .OrderByDescending(p => p.IsActive)
                            .ThenByDescending(p => (double)p.Discount);
                break;
        }
        return products;
    }
}
