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
/// Handles requests pertaining to products.
/// </summary>
public sealed class ProductService
{
    private readonly MiauDbContext _db;
    private readonly IRequestValidator<CreatedProductRequest> _validator;

    public ProductService(MiauDbContext db, IRequestValidator<CreatedProductRequest> validator)
    {
        _db = db;
        _validator = validator;
    }

    /// <summary>
    /// Returns a list of products.
    /// </summary>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<GetProductResponse, ErrorResponse>>> GetProductAsync(ProductParameters productParameters)
    {

        var errorMessages = Enumerable.Empty<string>();
        var dbProducts = _db.Products.Select(p => new ProductObject{
                                                        Id = p.Id,
                                                        Name = p.Name,
                                                        Description = p.Description,
                                                        Brand = p.Brand,
                                                        Price = p.Price,
                                                        IsActive = p.IsActive,
                                                        Amount = p.Amount,
                                                        Tags = p.Tags,
                                                        Discount = p.Discount
                                                    });

        if (productParameters.SearchedTerm is not null)
        {
            dbProducts = SearchByTerm(dbProducts, productParameters.SearchedTerm);
        }

        if (productParameters.Brand is not null)
        {
            dbProducts = SearchByBrand(dbProducts, productParameters.Brand);
        }

        if (productParameters.MaxPrice != 0 || productParameters.MinPrice != 0)
        {
            dbProducts = SearchByPriceRange(dbProducts, productParameters.MinPrice, productParameters.MaxPrice, out var descriptionError);
            if (descriptionError != string.Empty)
                return new BadRequestObjectResult(descriptionError);
        }

        if (productParameters.ActiveDiscount)
        {
            dbProducts = SearchByActiveDiscount(dbProducts);
        }

        if (productParameters.Tags != 0)
        {
            dbProducts = SearchByTags(dbProducts, productParameters.Tags);
        }

        dbProducts = Sort(dbProducts, productParameters.SortParameter);

        var dbProductsList = await dbProducts
                            .ToListAsync();

        if (dbProductsList.Count==0)
        {
            return new NotFoundObjectResult("No products with the given paramenters were found.");
        }

        var dbProductsPaged = PagedList<ProductObject>.ToPagedList(
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

        // TODO: handle authentication properly
        return new CreatedResult(location, new CreatedProductResponse(dbProduct.Id));
    }

    /// <summary>
    /// Lists products with an specific term in the description
    /// </summary>
    /// <returns>The result of the operation and a description error.</returns>
    public IQueryable<ProductObject> SearchByTerm(IQueryable<ProductObject> products, string term)
    {
        return products.Where(p => p.Name.ToLower().Contains(term.ToLower()));
    }

    /// <summary>
    /// Lists products with an specific brand
    /// </summary>
    /// <returns>The result of the operation and a description error.</returns>
    public IQueryable<ProductObject> SearchByBrand(IQueryable<ProductObject> products, string brand)
    {
        return products.Where(p => p.Brand == brand.ToLower());
    }

    /// <summary>
    /// Lists products within an specific price range
    /// </summary>
    /// <returns>The result of the operation and a description error.</returns>
    public IQueryable<ProductObject> SearchByPriceRange(IQueryable<ProductObject> products, decimal minPrice, decimal maxPrice, out string errorMessages)
    {
        errorMessages = string.Empty;
        if (maxPrice == 0 && minPrice != 0)
        {
            var mostExpensiveProduct = products
                                        .Where(p => (p.IsActive))
                                        .OrderByDescending(p => (double)p.Price * (1 - (double)p.Discount))
                                        .Select(p => p.Price)
                                        .First();
            if(mostExpensiveProduct > minPrice)
            {
                maxPrice = mostExpensiveProduct;
            }
            else
            {
                maxPrice = minPrice;
            }
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
            return products
                            .Where(p => ((p.Price * (1 - p.Discount)) >= minPrice)
                                     && ((p.Price * (1 - p.Discount)) <= maxPrice));
        }
    }

    /// <summary>
    /// Lists products with an active discount
    /// </summary>
    /// <returns>The result of the operation and a description error.</returns>
    public IQueryable<ProductObject> SearchByActiveDiscount(IQueryable<ProductObject> products)
    {
        return products.Where(p => p.Discount > 0);
    }

    /// <summary>
    /// Lists products with specifics tags
    /// </summary>
    /// <returns>The result of the operation and a description error.</returns>
    public IQueryable<ProductObject> SearchByTags(IQueryable<ProductObject> products, ProductTag tags)
    {
        return products.Where(p => p.Tags.HasFlag(tags));
    }

    /// <summary>
    /// Sorts products by a given parameter (Ascending Price as default, Descending Price, Ascending Discount, Descending Discount)
    /// </summary>
    /// <returns>The sorted product list.</returns>
    public IQueryable<ProductObject> Sort(IQueryable<ProductObject> products, Enum sortOrder)
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
