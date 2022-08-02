namespace MiauAPI.Models.QueryObjects;

/// <summary>
/// Represents the object in a product image query.
/// </summary>
/// <param name="ProductId">The Id of the product image.</param>
/// <param name="ProductId">The Id of the product related to the image.</param>
/// <param name="ImagePath">The Path of the image.</param>
public sealed record ProductImageObject
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ImagePath { get; set; } = null!;

} 