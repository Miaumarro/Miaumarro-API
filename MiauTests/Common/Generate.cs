using System.Text;

namespace MiauTests.Common;

/// <summary>
/// Helper class for generating random data.
/// </summary>
internal static class Generate
{
    /// <summary>
    /// Generates a random CPF.
    /// </summary>
    /// <returns>A random CPF.</returns>
    internal static string RandomCpf()
        => RandomText('0'..'9', 11);

    /// <summary>
    /// Generates random text with characters from the specified <paramref name="range"/>.
    /// </summary>
    /// <param name="range">The range of the random characters.</param>
    /// <param name="length">The final length of the string.</param>
    /// <returns>A randomly generated string.</returns>
    internal static string RandomText(Range range, int length)
    {
        var counter = 0;
        var stringBuilder = new StringBuilder();

        while (counter++ < length)
            stringBuilder.Append(RandomCharacter(range));

        return stringBuilder.ToString();
    }

    /// <summary>
    /// Generates a random e-mail.
    /// </summary>
    /// <returns>A random e-mail.</returns>
    internal static string RandomEmail()
    {
        return RandomText('a'..'z', Random.Shared.Next(1, 10)) +
            "@" +
            RandomText('a'..'z', Random.Shared.Next(1, 10)) +
            ".com";
    }

    /// <summary>
    /// Generates a random character within the specified <paramref name="range"/>.
    /// </summary>
    /// <param name="range">The range of the random character.</param>
    /// <returns>A random character.</returns>
    internal static char RandomCharacter(Range range)
    {
        return (range.Start.Value < 0 || range.End.Value < 0 || range.Start.Value >= range.End.Value)
            ? (char)Random.Shared.Next(range.Start.Value, ushort.MaxValue)
            : (char)Random.Shared.Next(range.Start.Value, range.End.Value);
    }
}