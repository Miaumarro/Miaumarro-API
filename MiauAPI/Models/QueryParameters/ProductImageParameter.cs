namespace MiauAPI.Models.QueryParameters;

/// <summary>
/// Set relevant parameters for a product image search.
/// </summary>
/// <param name="ProductId">The product Id.</param>
/// <param name="Id">The product image Id.</param>
public sealed record ProductImageParameters
{
    public int ProductId { get; init; }
    public int Id { get; init; }

}

