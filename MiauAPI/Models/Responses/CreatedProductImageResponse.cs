namespace MiauAPI.Models.Responses;

/// <summary>
/// Represents the response given when a product image is successfully created or updated.
/// </summary>
/// <param name="Id">The database ID of the product image.</param>
public sealed record CreatedProductImageResponse(int Id);
