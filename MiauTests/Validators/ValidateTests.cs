using MiauAPI.Validators;

namespace MiauTests.Validators;

public sealed class ValidateTests
{
    [Theory]
    [InlineData(true, "", 0, 0)]
    [InlineData(true, "", 0, 5)]
    [InlineData(true, "test", 0, 5)]
    [InlineData(true, "test", 3, 5)]
    [InlineData(true, "test", 4, 5)]
    [InlineData(true, "test", 4, 4)]
    [InlineData(false, null, 0, 0)]
    [InlineData(false, null, 0, 5)]
    [InlineData(false, null, 1, 5)]
    [InlineData(false, "", 1, 1)]
    [InlineData(false, "", 1, 5)]
    [InlineData(false, "test", 0, 0)]
    [InlineData(false, "test", 0, 3)]
    [InlineData(false, "test", 5, 10)]
    internal void IsTextInRangeTest(bool expected, string? input, int minLength, int maxLength)
    {
        Assert.Equal(expected, Validate.IsTextInRange(input, minLength, maxLength, nameof(input), out var errorMessage));
        CheckNullability(expected, errorMessage);
    }

    [Theory]
    [InlineData(true, "test", 5)]
    [InlineData(true, "test", 4)]
    [InlineData(false, "test", 3)]
    [InlineData(false, "test", 1)]
    [InlineData(false, "", 5)]
    [InlineData(false, null, 5)]
    internal void IsTextInRangeNoMinTest(bool expected, string? input, int maxLength)
    {
        Assert.Equal(expected, Validate.IsTextInRange(input, maxLength, nameof(input), out var errorMessage));
        CheckNullability(expected, errorMessage);
    }

    [Theory]
    [InlineData("test", -3, 5)]
    [InlineData("test", 3, -5)]
    [InlineData("test", -3, -5)]
    [InlineData("test", 1, 0)]
    [InlineData("test", 3, 1)]
    internal void IsTextInRangeFailTest(string? input, int minLength, int maxLength)
    {
        Assert.Throws<ArgumentException>(() => Validate.IsTextInRange(input, minLength, maxLength, nameof(input), out _));
        Assert.Throws<ArgumentException>(() => Validate.IsTextInRange(input, 0, nameof(input), out _));
    }

    [Theory]
    [InlineData(true, "test@email.com")]
    [InlineData(true, "123test-_@email.com")]
    [InlineData(true, "test@email.com.br")]
    [InlineData(true, "a@d.c.d")]
    [InlineData(false, "")]
    [InlineData(false, "@")]
    [InlineData(false, "testemail.com")]
    [InlineData(false, "test@emailcom")]
    [InlineData(false, "@email.com")]
    [InlineData(false, "test@com")]
    [InlineData(false, "test@")]
    [InlineData(false, "testemailcom")]
    internal void IsEmailTest(bool expected, string input)
    {
        Assert.Equal(expected, Validate.IsEmail(input, out var errorMessage));
        CheckNullability(expected, errorMessage);
    }

    [Fact]
    internal void IsEmailFailTest()
        => Assert.Throws<ArgumentNullException>(() => Validate.IsEmail(null!, out _));

    [Theory]
    [InlineData(true, "1234567890")]
    [InlineData(true, "0")]
    [InlineData(false, "")]
    [InlineData(false, " ")]
    [InlineData(false, "a")]
    [InlineData(false, "1234 5678")]
    [InlineData(false, " 1234")]
    [InlineData(false, "1234 ")]
    [InlineData(false, " 1234 ")]
    [InlineData(false, "12_34")]
    [InlineData(false, "12a34")]
    [InlineData(false, "a1234")]
    [InlineData(false, "1234a")]
    [InlineData(false, "a1234a")]

    internal void HasOnlyDigitsTest(bool expected, string input)
    {
        Assert.Equal(expected, Validate.HasOnlyDigits(input, nameof(input), out var errorMessage));
        CheckNullability(expected, errorMessage);
    }

    [Fact]
    internal void HasOnlyDigitsFailTest()
        => Assert.Throws<ArgumentNullException>(() => Validate.HasOnlyDigits(null!, "?", out _));

    [Theory]
    [InlineData(true, null)]
    [InlineData(false, 1)]
    [InlineData(false, "")]
    [InlineData(false, "blep")]
    internal void IsNullTest(bool expected, object? input)
    {
        Assert.Equal(expected, Validate.IsNull(input, nameof(input), out var errorMessage));
        CheckNullability(!expected, errorMessage);
    }

    [Theory]
    [InlineData(true, null)]
    [InlineData(true, "")]
    [InlineData(true, "   ")]
    [InlineData(false, "-")]
    internal void IsNullOrWhiteSpace(bool expected, string? input)
    {
        Assert.Equal(expected, Validate.IsNullOrWhiteSpace(input, nameof(input), out var errorMessage));
        CheckNullability(!expected, errorMessage);
    }

    /// <summary>
    /// Checks if <paramref name="errorMessage"/> is <see langword="null"/> if <paramref name="expected"/> is <see langword="true"/>
    /// or not <see langword="null"/> if <paramref name="expected"/> is false.
    /// </summary>
    /// <param name="expected">The output of the try method.</param>
    /// <param name="errorMessage">The output of the try method's 'out var'.</param>
    private static void CheckNullability(bool expected, string? errorMessage)
    {
        if (expected)
            Assert.Null(errorMessage);
        else
            Assert.NotNull(errorMessage);
    }
}