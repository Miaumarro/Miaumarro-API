using MiauDatabase.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MiauDatabase.Entities;

/// <summary>
/// Represents a database discount coupon.
/// </summary>
[Comment("Represents a discount coupon.")]
public sealed record CouponEntity : MiauDbEntity
{
    private decimal _discount;

    /// <summary>
    /// The purchases that have used this coupon.
    /// </summary>
    public List<PurchaseEntity> Purchases { get; init; } = new();

    /// <summary>
    /// The string the user is supposed to enter to activate the coupon for their purchase.
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string Coupon { get; init; } = null!;

    /// <summary>
    /// Determines whether this coupon can be used.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// The discount to be applied to the purchase.
    /// </summary>
    /// <remarks>Must be a value between 0 and 1.</remarks>
    public decimal Discount
    {
        get => _discount;
        init => _discount = (value is < 0 or > 1)
            ? throw new ArgumentOutOfRangeException(nameof(value), "Discount must be between 0 and 1.")
            : value;
    }
}