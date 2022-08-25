namespace MiauAPI.Models.QueryParameters;

/// <summary>
/// Set relevant parameters for a product image search.
/// </summary>
/// <param name="Id">The product image Id.</param>
/// <param name="ProductId">The product Id.</param>
public sealed record ProductImageParameters(int Id, int ProductId) : QueryStringParameters;