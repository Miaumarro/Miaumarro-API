using MiauAPI.Models.QueryObjects;

namespace MiauAPI.Models.Responses;

/// <summary>
/// Represents the response when a product is requested by its Id.
/// </summary>
/// <param name="Product">The resulted product.</param>
public sealed record GetProductByIdResponse(ProductObject Product);
