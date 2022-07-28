using MiauAPI.Models.Parameters;
using MiauAPI.Models.Requests;
using MiauAPI.Models.Responses;
using MiauAPI.Pagination;
using MiauAPI.Validators.Abstractions;
using MiauDatabase;
using MiauDatabase.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneOf;
using Encrypt = BCrypt.Net.BCrypt;

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
                    .OrderByDescending(p => p.IsActive)
                    .ToList();

        // TODO: implement await in the correct way
        await Task.Delay(1000);
        if (dbProducts is null)
        {
            errorMessages = errorMessages.Append("No registered products.");
            return new NotFoundObjectResult(new ErrorResponse(errorMessages.ToArray()));
        }

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