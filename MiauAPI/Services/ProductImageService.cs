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

    public ProductImageService(MiauDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Creates a new product image.
    /// </summary>
    /// <param name="request">The controller request.</param>
    /// <param name="location">The URL of the new resource or the content of the Location header.</param>
    /// <returns>The result of the operation.</returns>
    /// <exception cref="ArgumentException">Occurs when <paramref name="location"/> is <see langword="null"/> or empty.</exception>
    public async Task<ActionResult<OneOf<CreatedProductImageResponse, ErrorResponse>>> CreatedProductImageAsync(CreatedProductImageRequest request, string location)
    {
        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Location cannot be null or empty.", nameof(location));

        // Checks the ProductId
        var dbProduct = await _db.Products.FirstOrDefaultAsync(x => x.Id == request.ProductId);
        if (dbProduct == null)
        {
            return new NotFoundObjectResult(new ErrorResponse($"No product with the Id = {request.ProductId} was found"));
        }

        //Creates the path for the Product Image
        var path = $"Data/{request.ProductId}/images";
        if ((!Directory.Exists(path)))
        {
            Directory.CreateDirectory(path);
        }
        var filename = request.ImageFile.FileName;
        using (var fileStream = new FileStream(Path.Combine(path, filename), FileMode.Create))
        {
            await request.ImageFile.CopyToAsync(fileStream);
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
        return new CreatedResult(location, new CreatedProductImageResponse(dbProductImage.Id));
    }

    /// <summary>
    /// Returns a list of product images.
    /// </summary>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<GetProductImageResponse, ErrorResponse>>> GetProductImageAsync(ProductImageParameters productImageParameters)
    {

        var errorMessages = Enumerable.Empty<string>();
        var dbProductImages = _db.ProductImages.Select(p => new ProductImageObject
        {
            Id = p.Id,
            ProductId = p.Product.Id,
            ImagePath = p.FileUrl
        });

        if (productImageParameters.ProductId != 0)
        {
            dbProductImages = dbProductImages.Where(p => p.ProductId == productImageParameters.ProductId);
        }

        if (productImageParameters.Id != 0)
        {
            dbProductImages = dbProductImages.Where(p => p.Id == productImageParameters.Id);
        }

        var dbProductImagesList = await dbProductImages
                            .ToListAsync();

        if (dbProductImagesList.Count == 0)
        {
            return new NotFoundObjectResult("No product images with the given paramenters were found.");
        }

        return new OkObjectResult(new GetProductImageResponse(dbProductImagesList));

    }

    /// <summary>
    /// Deletes the product image with the given Id.
    /// </summary>
    /// <param name="id">The id of the product image to be deleted.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<DeleteResponse, ErrorResponse>>> DeleteProductImageByIdAsync(int id)
    {

        var dbProductImage = await _db.ProductImages.FindAsync(id);
        if (dbProductImage == null)
        {
            return new NotFoundObjectResult(new ErrorResponse($"No product image with the Id = {id} was found"));
        }

        await _db.ProductImages.DeleteAsync(x => x.Id == id);
        await _db.SaveChangesAsync();

        return new OkObjectResult(new DeleteResponse($"Successfull delete product image with the Id = {id}"));
    }

}
