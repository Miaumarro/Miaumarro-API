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
/// Handles requests pertaining to products images.
/// </summary>
public sealed class ProductImageService
{
    private readonly MiauDbContext _db;
    private readonly IRequestValidator<ProductImageRequest> _validator;

    public ProductImageService(MiauDbContext db, IRequestValidator<ProductImageRequest> validator)
    {
        _db = db;
        _validator = validator;
    }

    /// <summary>
    /// Creates a new product image.
    /// </summary>
    /// <param name="request">The controller request.</param>
    /// <param name="location">The URL of the new resource or the content of the Location header.</param>
    /// <returns>The result of the operation.</returns>
    /// <exception cref="ArgumentException">Occurs when <paramref name="location"/> is <see langword="null"/> or empty.</exception>
    public async Task<ActionResult<OneOf<ProductImageResponse, ErrorResponse>>> CreatedProductImageAsync(ProductImageRequest request, string location)
    {
        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Location cannot be null or empty.", nameof(location));

        // Checks if request contains valid data
        if (!_validator.IsRequestValid(request, out var errorMessages))
            return new BadRequestObjectResult(new ErrorResponse(errorMessages.ToArray()));

        // Checks the ProductId
        var dbProduct = await _db.Products.FirstAsync(x => x.Id == request.ProductId);
        if (dbProduct == null)
        {
            new NotFoundObjectResult(new ErrorResponse($"No product with the Id = {request.ProductId} was found"));
        }

        //Creates the path for the Product Image
        var path = $"Data/{request.ProductId}/images";
        if ((!Directory.Exists(path)))
        {
            Directory.CreateDirectory(path);
        }
        var filename = request.ImagePath.FileName;
        using (var fileStream = new FileStream(Path.Combine(path, filename), FileMode.Create))
        {
            await request.ImagePath.CopyToAsync(fileStream);
        }

        // Create the database product image
        var dbProductImage = new ProductImageEntity()
        {
            Product = dbProduct!,
            FileUrl = $"images/{request.ProductId}/" + filename
        };

        await _db.ProductImages.AddAsync(dbProductImage);
        await _db.SaveChangesAsync();

        // TODO: handle authentication properly
        return new CreatedResult(location, new ProductImageResponse(dbProductImage.Id));
    }

}
