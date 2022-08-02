namespace MiauAPI.Models.Requests;

/// <summary>
/// Represents a request for create or update a product iamge.
/// </summary>
/// <param name="ProductId">The id of pet's user.</param>
/// <param name="ImagePath">This Products's image location in the file system.</param>
public sealed record ProductImageRequest(int ProductId, IFormFile ImagePath);

