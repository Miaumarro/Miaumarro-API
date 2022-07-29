
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace MiauAPI.Validators;

/// <summary>
/// Helper class to validate request parameters and generate standardized error
/// messages on validation failures.
/// </summary>
public static class Validate
{
    /// <summary>
    /// Regex to match a valid e-mail address.
    /// </summary>
    private static readonly Regex _emailRegex = new(@"(^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$)", RegexOptions.Compiled);

    /// <summary>
    /// Checks if <paramref name="text"/> is not empty and is not longer than <paramref name="maxLength"/>.
    /// </summary>
    /// <param name="text">The string to be checked.</param>
    /// <param name="maxLength">The maximum length the string should have.</param>
    /// <param name="paramName">The name of the string parameter.</param>
    /// <param name="errorMessage">
    /// The resulting error message if <paramref name="text"/> is not within the specified range,
    /// <see langword="null"/> otherwise.
    /// </param>
    /// <returns><see langword="true"/> if <paramref name="text"/> is <see langword="null"/> or within the specified range, <see langword="false"/> otherwise.</returns>
    /// <exception cref="ArgumentException">Occurs when <paramref name="maxLength"/> is less than 1.</exception>
    public static bool IsTextInRange(string? text, int maxLength, string paramName, [MaybeNullWhen(true)] out string errorMessage)
            => IsTextInRange(text, 1, maxLength, paramName, out errorMessage);

    /// <summary>
    /// Checks if <paramref name="text"/>'s length is between <paramref name="minLength"/> and <paramref name="maxLength"/>.
    /// </summary>
    /// <param name="text">The string to be checked.</param>
    /// <param name="minLength">The minimum length the string should have.</param>
    /// <param name="maxLength">The maximum length the string should have.</param>
    /// <param name="paramName">The name of the string parameter.</param>
    /// <param name="errorMessage">
    /// The resulting error message if <paramref name="text"/> is not within the specified range,
    /// <see langword="null"/> otherwise.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="text"/> is <see langword="null"/> or within the specified range,
    /// <see langword="false"/> otherwise.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Occurs when <paramref name="minLength"/> is greater than <paramref name="maxLength"/> or when either are
    /// less than zero.
    /// </exception>
    public static bool IsTextInRange(string? text, int minLength, int maxLength, string paramName, [MaybeNullWhen(true)] out string errorMessage)
    {
        if (minLength < 0 || maxLength < 0)
            throw new ArgumentException($"{nameof(minLength)} and {nameof(maxLength)} cannot be less than zero.");
        else if (minLength > maxLength)
            throw new ArgumentException($"{nameof(minLength)} cannot be greater than {nameof(maxLength)}", nameof(minLength));

        errorMessage = (text is null || text.Length < minLength || text.Length > maxLength)
            ? $"{paramName} length must be between {minLength} and {maxLength}. Value: {text?.Length}"
            : null;

        return errorMessage is null;
    }

    /// <summary>
    /// Checks if <paramref name="text"/> is a valid e-mail address.
    /// </summary>
    /// <param name="text">The string to be checked.</param>
    /// <param name="errorMessage">The resulting error message if the method returns <see langword="false"/>, <see langword="null"/> otherwise.</param>
    /// <returns><see langword="true"/> if <paramref name="text"/> is a valid e-mail address, <see langword="false"/> otherwise.</returns>
    /// <exception cref="ArgumentNullException">Occurs when <paramref name="text"/> is <see langword="null"/>.</exception>
    public static bool IsEmail(string text, [MaybeNullWhen(true)] out string errorMessage)
    {
        ArgumentNullException.ThrowIfNull(text);

        if (_emailRegex.IsMatch(text))
        {
            errorMessage = null;
            return true;
        }

        errorMessage = $"'{text}' is not a valid e-mail.";
        return false;
    }

    /// <summary>
    /// Checks if <paramref name="value"/> is <see langword="null"/>.
    /// </summary>
    /// <param name="value">The object to be checked.</param>
    /// <param name="paramName">The name of the object parameter.</param>
    /// <param name="errorMessage">The resulting error message if the method returns <see langword="true"/>, <see langword="null"/> otherwise.</param>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <returns><see langword="true"/> if <paramref name="value"/> is <see langword="null"/>, <see langword="false"/> otherwise.</returns>
    public static bool IsNull<T>(T value, string paramName, [MaybeNullWhen(false)] out string errorMessage)
    {
        errorMessage = (value is null)
            ? $"{paramName} cannot be null."
            : null;

        return value is null;
    }

    /// <summary>
    /// Checks if <paramref name="text"/> only contains digits.
    /// </summary>
    /// <param name="text">The string to be checked.</param>
    /// <param name="paramName">The name of the string parameter.</param>
    /// <param name="errorMessage">The resulting error message if the method returns <see langword="false"/>, <see langword="null"/> otherwise.</param>
    /// <returns><see langword="true"/> if <paramref name="text"/> only contains digits, <see langword="false"/> otherwise.</returns>
    /// <exception cref="ArgumentNullException">Occurs when <paramref name="text"/> is <see langword="null"/>.</exception>
    public static bool HasOnlyDigits(string text, string paramName, [MaybeNullWhen(true)] out string errorMessage)
    {
        ArgumentNullException.ThrowIfNull(text, paramName);

        errorMessage = (text.Length > 0 && text.All(x => char.IsDigit(x)))
            ? null
            : $"{paramName} must only contain digits.";

        return errorMessage is null;
    }

    /// <summary>
    /// Checks if <paramref name="text"/> only contains digits.
    /// </summary>
    /// <param name="date">The date to be checked.</param>
    /// <param name="paramName">The name of the string parameter.</param>
    /// <param name="errorMessage">The resulting error message if the method returns <see langword="false"/>, <see langword="null"/> otherwise.</param>
    /// <returns><see langword="true"/> if <paramref name="text"/> the date is valid, <see langword="false"/> otherwise.</returns>
    /// <exception cref="ArgumentNullException">Occurs when <paramref name="text"/> is <see langword="null"/>.</exception>
    public static bool IsDateValid(DateTime date, string paramName, [MaybeNullWhen(true)] out string errorMessage)
    {
        ArgumentNullException.ThrowIfNull(date, paramName);
        errorMessage = (date >= DateTime.UtcNow)
           ? $"{paramName} must be a valid date."
           : null;
        return errorMessage is null;
    }
}