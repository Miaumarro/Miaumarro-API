using MiauAPI.Models.QueryParameters;
using MiauAPI.Models.Requests;
using MiauAPI.Models.Responses;
using MiauAPI.Pagination;
using MiauDatabase;
using MiauDatabase.Entities;
using Microsoft.AspNetCore.Mvc;
using OneOf;
using LinqToDB;
using MiauAPI.Models.QueryObjects;
using MiauAPI.Extensions;
using Microsoft.EntityFrameworkCore;
using LinqToDB.EntityFrameworkCore;
using OneOf.Types;

namespace MiauAPI.Services;

/// <summary>
/// Handles requests pertaining to products images.
/// </summary>
public sealed class ProductImageService
{
    private const string _imageDirName = "product_images";
    private readonly MiauDbContext _db;
    private readonly FileService _fileService;

    public ProductImageService(MiauDbContext db, FileService fileService)
    {
        _db = db;
        _fileService = fileService;
    }

    /// <summary>
    /// Creates a new product image.
    /// </summary>
    /// <param name="request">The controller request.</param>
    /// <param name="location">The URL of the new resource or the content of the Location header.</param>
    /// <returns>The result of the operation.</returns>
    /// <exception cref="ArgumentException">Occurs when <paramref name="location"/> is <see langword="null"/> or empty.</exception>
    public async Task<ActionResult<OneOf<CreatedProductImageResponse, None>>> CreatedProductImageAsync(CreatedProductImageRequest request, string location)
    {
        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Location cannot be null or empty.", nameof(location));

        // Checks the ProductId
        var dbProduct = await _db.Products
            .AsTracking()
            .Where(x => x.Id == request.ProductId)
            .Select(x => new ProductEntity() { Id = request.ProductId })
            .FirstOrDefaultAsyncEF();

        if (dbProduct is null)
            return new NotFoundResult();

        //Creates the path for the Product Image
        var imagePath = await _fileService.SaveFileAsync(request.Image, Path.Combine(_imageDirName, request.ProductId.ToString()), request.ProductId.ToString());

        // Create the database product image
        var dbProductImage = new ProductImageEntity()
        {
            Product = dbProduct,
            FileUrl = imagePath
        };

        _db.ProductImages.Add(dbProductImage);
        await _db.SaveChangesAsync();

        return new CreatedResult(location, new CreatedProductImageResponse(dbProductImage.Id));
    }

    /// <summary>
    /// Returns a list of product images.
    /// </summary>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult<OneOf<GetProductImageResponse, None>>> GetProductImagesAsync(ProductImageParameters productImageParameters)
    {
        var dbProductImages = await _db.ProductImages
            .Include(x => x.Product)
            .Where(x => x.Product.Id == productImageParameters.ProductId)
            .Select(x => new ProductImageObject(x.Id, x.Product.Id, x.FileUrl))
            .ToArrayAsyncEF();

        return (dbProductImages.Length is 0)
            ? new NotFoundResult()
            : new OkObjectResult(new GetProductImageResponse(dbProductImages));
    }

    /// <summary>
    /// Deletes the product image with the given Id.
    /// </summary>
    /// <param name="productImageId">The id of the product image to be deleted.</param>
    /// <returns>The result of the operation.</returns>
    public async Task<ActionResult> DeleteProductImageByIdAsync(int productImageId)
    {
        return ((await _db.ProductImages.DeleteAsync(p => p.Id == productImageId)) is 0)
            ? new NotFoundResult()
            : new OkResult();
    }
}
