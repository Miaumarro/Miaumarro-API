namespace MiauAPI.Common;

/// <summary>
/// Defines this API's global constants.
/// </summary>
public static class ApiConstants
{
    /// <summary>
    /// Represents the version of the API.
    /// </summary>
    public const string Version = "v1";

    /// <summary>
    /// Represents the main path of the API endpoints.
    /// </summary>
    public const string MainEndpoint = $"api/{Version}/[controller]";

    /// <summary>
    /// Represents the location of the security key in the "appsettings.json" file.
    /// </summary>
    public const string JwtAppSetting = "Jwt:Key";
}