using MiauAPI.Pagination;
using MiauDatabase.Entities;

namespace MiauAPI.Models.Responses;

/// <summary>
/// Represents the response given when a product is successfully created.
/// </summary>
/// <param name="Id">The database ID of the product.</param>
public sealed record CreatedProductResponse(int Id);
public sealed record GetProductResponse(PagedList<ProductEntity> Products);
