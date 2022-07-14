using MiauDatabase.Abstractions;
using MiauDatabase.Enums;
using Microsoft.EntityFrameworkCore;

namespace MiauDatabase.Entities;

/// <summary>
/// Represents a database purchase.
/// </summary>
/// <value></value>
[Comment("Represents a purchase.")]
public sealed record PurchaseEntity : MiauDbEntity
{
    /// <summary>
    /// The user this purchase is associated with.
    /// </summary>
    public UserEntity User { get; init; } = null!;

    /// <summary>
    /// The purchased products this purchase is associated with.
    /// </summary>
    public List<PurchasedProductEntity> PurchasedProduct { get; init; } = new();

    /// <summary>
    /// The discount coupon used for this purchase.
    /// </summary>
    public string? Coupon { get; init; }

    /// <summary>
    /// The status of this purchase.
    /// </summary>
    public PurchaseStatus Status { get; init; } = PurchaseStatus.Pending;
}