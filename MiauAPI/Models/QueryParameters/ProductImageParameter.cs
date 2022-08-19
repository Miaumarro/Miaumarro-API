namespace MiauAPI.Models.QueryParameters;

/// <summary>
/// Set relevant parameters for a product image search.
/// </summary>
/// <param name="ProductId">The product Id.</param>
/// <param name="Id">The product image Id.</param>
public sealed record ProductImageParameters(int ProductId, int Id) : QueryStringParameters;