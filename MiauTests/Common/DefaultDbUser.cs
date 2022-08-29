using MiauDatabase.Entities;
using Encrypt = BCrypt.Net.BCrypt;

namespace MiauTests.Common;

/// <summary>
/// Contains valid properties of a <see cref="UserEntity"/> object as constants,
/// so their values can be used on attributes.
/// </summary>
internal static class DefaultDbUser
{
    /// <summary>
    /// Id of the user.
    /// </summary>
    internal const int Id = 1;

    /// <summary>
    /// Cpf of the user.
    /// </summary>
    internal const string Cpf = "12345678920";

    /// <summary>
    /// E-mail of the user.
    /// </summary>
    internal const string Email = "user@email.com";

    /// <summary>
    /// Raw passwords of the user.
    /// </summary>
    internal const string Password = "avocado";

    /// <summary>
    /// Name of the user.
    /// </summary>
    internal const string Name = "Herp";

    /// <summary>
    /// Surname of the user.
    /// </summary>
    internal const string Surname = "Derp";

    /// <summary>
    /// Phone number of the user.
    /// </summary>
    internal const string Phone = "21111111111";

    /// <summary>
    /// The instance of the user.
    /// </summary>
    internal static UserEntity Instance = new()
    {
        Id = Id,
        Cpf = Cpf,
        Name = Name,
        Surname = Surname,
        Email = Email,
        Phone = Phone,
        HashedPassword = Encrypt.HashPassword(Password),
        DateAdded = DateTime.UtcNow
    };
}