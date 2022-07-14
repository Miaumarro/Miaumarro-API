using MiauDatabase.Abstractions;
using MiauDatabase.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MiauDatabase.Entities;

/// <summary>
/// Represents a database user.
/// </summary>
[Comment("Represents a user.")]
public sealed record UserEntity : MiauDbEntity
{
    /// <summary>
    /// The addresses associated with this user.
    /// </summary>
    public List<AddressEntity> Addresses { get; init; } = new();

    /// <summary>
    /// The pets associated with this user.
    /// </summary>
    public List<PetEntity> Pets { get; init; } = new();

    /// <summary>
    /// The purchases associated with this user.
    /// </summary>
    public List<PurchaseEntity> Purchases { get; init; } = new();

    /// <summary>
    /// The product reviews associated with this user.
    /// </summary>
    public List<ProductReviewEntity> ProductReviews { get; init; } = new();

    /// <summary>
    /// The wishlist items associated with this user.
    /// </summary>
    public List<WishlistEntity> Wishlist { get; init; } = new();

    /// <summary>
    /// This user's CPF.
    /// </summary>
    [Required]
    [StringLength(14)]
    public string Cpf { get; init; } = null!;

    /// <summary>
    /// This user's name.
    /// </summary>
    [Required]
    [MaxLength(30)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// This user's surname.
    /// </summary>
    [Required]
    [MaxLength(60)]
    public string Surname { get; set; } = null!;

    /// <summary>
    /// This user's e-mail.
    /// </summary>
    [Required]
    [MaxLength(60)]
    public string Email { get; set; } = null!;

    /// <summary>
    /// This user's phone.
    /// </summary>
    [MaxLength(14)]
    public string? Phone { get; set; }

    /// <summary>
    /// This user's password salt.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Salt { get; set; } = null!;

    /// <summary>
    /// This user's salted password.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string SaltedPassword { get; set; } = null!;

    /// <summary>
    /// This user's permissions in the store.
    /// </summary>
    public UserPermissions Permissions { get; set; } = UserPermissions.Customer;
}