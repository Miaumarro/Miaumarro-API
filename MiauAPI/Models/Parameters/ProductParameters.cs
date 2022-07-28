using MiauDatabase.Enums;

namespace MiauAPI.Models.Parameters;

public class ProductParameters : QueryStringParameters
{
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
    public string? SearchedTerm { get; set; }
    public string? Brand { get; set; }
    public bool ActiveDiscount { get; set; } = false;
    public ProductTag Tags { get; set; }
    public SortParameter SortParameter { get; set; }


}

public enum SortParameter
{
    PriceAsc,
    PriceDesc,
    DiscountAsc,
    DiscountDesc
}
