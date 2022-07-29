using MiauAPI.Pagination;
using MiauDatabase.Entities;

namespace MiauAPI.Models.Responses;

/// <summary>
/// Represents the response given when a product query is successfully executed.
/// </summary>
/// <param name="Products">The resulted list of products.</param>
public sealed record GetProductResponse(PagedList<ProductEntity> Products);
