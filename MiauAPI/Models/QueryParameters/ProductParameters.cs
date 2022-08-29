using MiauAPI.Enums;
using MiauDatabase.Enums;

namespace MiauAPI.Models.QueryParameters;

/// <summary>
/// Set relevant parameters for a product search.
/// </summary>
public sealed record ProductParameters : QueryStringParameters
{
    private readonly string? _brand;

    /// <summary>
    /// The minimum price for a product.
    /// </summary>
    public decimal MinPrice { get; init; }

    /// <summary>
    /// The maximum price for a product.
    /// </summary>
    public decimal MaxPrice { get; init; }

    /// <summary>
    /// The term to be searched in the product's description.
    /// </summary>
    public string? SearchedTerm { get; init; }

    /// <summary>
    /// The brand to search for.
    /// </summary>
    /// <value></value>
    public string? Brand
    {
        get => _brand;
        init => _brand = value?.ToLowerInvariant();
    }

    /// <summary>
    /// Determines if the searched items should have an discount.
    /// </summary>
    public bool ActiveDiscount { get; init; }

    /// <summary>
    /// The product's tags to be searched.
    /// </summary>
    public ProductTag Tags { get; init; }

    /// <summary>
    /// How the query should be sorted.
    /// </summary>
    public SortParameter SortParameter { get; init; }

    public ProductParameters()
    {
    }

    public ProductParameters(decimal minPrice, decimal maxPrice, string? searchedTerm, string? brand, bool activeDiscount, ProductTag tags, SortParameter sortParameter)
    {
        MinPrice = minPrice;
        MaxPrice = maxPrice;
        SearchedTerm = searchedTerm;
        Brand = brand;
        ActiveDiscount = activeDiscount;
        Tags = tags;
        SortParameter = sortParameter;
    }
}