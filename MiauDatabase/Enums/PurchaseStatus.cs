namespace MiauDatabase.Enums;

/// <summary>
/// Represents the status of a purchase.
/// </summary>
public enum PurchaseStatus
{
    /// <summary>
    /// The purchase was cancelled.
    /// </summary>
    Canceled,

    /// <summary>
    /// The purchase is awaiting pending payment.
    /// </summary>
    Pending,

    /// <summary>
    /// The purchase has been completed and is pending delivery.
    /// </summary>
    Processed,

    /// <summary>
    /// The purchased product is in transit to its destination.
    /// </summary>
    InTransit,

    /// <summary>
    /// The purchased product has reached its destination.
    /// </summary>
    Completed
}