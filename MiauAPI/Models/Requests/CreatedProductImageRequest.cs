namespace MiauAPI.Models.Requests;

/// <summary>
/// Represents a request for create or update a product iamge.
/// </summary>
/// <param name="ProductId">The id of pet's user.</param>
/// <param name="Image">This Products's image file.</param>
public sealed record CreatedProductImageRequest(int ProductId, byte[] Image);

