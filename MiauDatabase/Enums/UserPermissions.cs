namespace MiauDatabase.Enums;

/// <summary>
/// Represents the level of permissions a user can have.
/// </summary>
[Flags]
public enum UserPermissions
{
    /// <summary>
    /// The user is not allowed to make purchases or schedule appointments in the store.
    /// </summary>
    Blocked = 0,

    /// <summary>
    /// The user is allowed to use the store as normal.
    /// </summary>
    Customer = 1 << 0,

    /// <summary>
    /// The user is allowed to update listings that are not critical to a product's sale.
    /// </summary>
    Clerk = 1 << 1,

    /// <summary>
    /// The user is allowed to update all listings, as well as add and remove them.
    /// </summary>
    Administrator = 1 << 2
}