using MiauAPI.Enums;
using MiauDatabase.Enums;

namespace MiauAPI.Models.QueryParameters;

/// <summary>
/// Set relevant parameters for a product search.
/// </summary>
/// <param name="MinPrice">The minimum price for a product.</param>
/// <param name="MaxPrice">The maximum price for a product.</param>
/// <param name="SearchedTerm">The term to be searched in the product's description.</param>
/// <param name="Brand">The brand to be search.</param>
/// <param name="ActiveDiscount">Determines if the searched items should have an discount.</param>
/// <param name="Tags">The product's tags to be searched.</param>
/// <param name="SortParameter">How the query should be sorted.</param>
public sealed record ProductParameters(
    decimal MinPrice,
    decimal MaxPrice,
    string? SearchedTerm,
    string? Brand,
    bool ActiveDiscount,
    ProductTag Tags,
    SortParameter SortParameter
) : QueryStringParameters;