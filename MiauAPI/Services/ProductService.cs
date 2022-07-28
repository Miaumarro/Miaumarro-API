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
using System.Diagnostics.CodeAnalysis;

namespace MiauAPI.Services;

/// <summary>
/// Handles requests pertaining to users.
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
        var dbProducts = _db.Products
                    .ToList();

        static bool CheckQuery(List<ProductEntity> resultedQuery, [MaybeNullWhen(true)] out string errorMessage)
        {
            errorMessage = (!resultedQuery.Any())
                ? "No products were found."
                : null;

            return errorMessage is null;
        }

        // TODO: implement await in the correct way
        await Task.Delay(1000);


        // List products with an specific term in the description
        if (productParameters.SearchedTerm is not null)
        {
            dbProducts = dbProducts
                                .Where(p => p.Description.ToLower().Contains(productParameters.SearchedTerm.ToLower()))
                                .ToList();

            if (!CheckQuery(dbProducts, out var descriptionError))
            {
                return new NotFoundObjectResult(new ErrorResponse(descriptionError));
            }
        }


        // List products with an specific brand
        if (productParameters.Brand is not null)
        {
            dbProducts = dbProducts
                                .Where(p => p.Brand == productParameters.Brand.ToLower())
                                .ToList();

            if (!CheckQuery(dbProducts, out var descriptionError))
            {
                return new NotFoundObjectResult(new ErrorResponse(descriptionError));
            }
        }

        // List products within a price range
        if (productParameters.MaxPrice != 0 || productParameters.MinPrice != 0)
        {
            if (productParameters.MaxPrice == 0 && productParameters.MinPrice != 0)
            {
                var dbProductMostExpensive = dbProducts
                                            .Where(p => (p.IsActive))
                                            .OrderByDescending(p => p.Price * (1 - p.Discount))
                                            .First();
                productParameters.MaxPrice = dbProductMostExpensive.Price;
            }

            if (!Validate.IsPositive(productParameters.MinPrice, nameof(productParameters.MinPrice), out var descriptionError)
                || !Validate.IsPositive(productParameters.MaxPrice, nameof(productParameters.MaxPrice), out descriptionError)
                || !Validate.IsValidRange<Decimal>(productParameters.MinPrice, productParameters.MaxPrice, nameof(productParameters.MinPrice), nameof(productParameters.MaxPrice), out descriptionError))
            {
                errorMessages = errorMessages.Append(descriptionError);
                return new BadRequestObjectResult(new ErrorResponse(errorMessages.ToArray()));
            }
            else
            {
                dbProducts = dbProducts
                                .Where(p => ((p.Price * (1 - p.Discount)) >= productParameters.MinPrice)
                                         && ((p.Price * (1 - p.Discount)) <= productParameters.MaxPrice))
                                .ToList();
            }

            if (!CheckQuery(dbProducts, out descriptionError))
            {
                return new NotFoundObjectResult(new ErrorResponse(descriptionError));
            }
        }

        // List products with an active discount
        if (productParameters.ActiveDiscount)
        {
            dbProducts = dbProducts
                            .Where(p => p.Discount > 0)
                            .ToList();

            if (!CheckQuery(dbProducts, out var descriptionError))
            {
                return new NotFoundObjectResult(new ErrorResponse(descriptionError));
            }
        }

        // List products with specifics tags
        if (productParameters.Tags != 0)
        {
            dbProducts = dbProducts
                            .Where(p => p.Tags.HasFlag(productParameters.Tags))
                            .ToList();

            if (!CheckQuery(dbProducts, out var descriptionError))
            {
                return new NotFoundObjectResult(new ErrorResponse(descriptionError));
            }
        }

        // Sort products by Price Asc (default), PriceDesc,  DiscountAsc, DiscountDesc
        switch (productParameters.SortParameter)
        {
            case SortParameter.PriceAsc:
                dbProducts = dbProducts
                            .OrderBy(p => (p.Price * (1 - p.Discount)))
                            .ToList();
                break;
            case SortParameter.PriceDesc:
                dbProducts = dbProducts
                            .OrderByDescending(p => (p.Price * (1 - p.Discount)))
                            .ToList();
                break;
            case SortParameter.DiscountAsc:
                dbProducts = dbProducts
                            .OrderBy(p => p.Discount)
                            .ToList();
                break;
            case SortParameter.DiscountDesc:
                dbProducts = dbProducts
                            .OrderByDescending(p => p.Discount)
                            .ToList();
                break;
        }

        dbProducts = dbProducts
                            .OrderByDescending(p => p.IsActive)
                            .ToList();

        var dbProductsPaged = PagedList<ProductEntity>.ToPagedList(
                        dbProducts,
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
}
