using System.Reflection;
using System;

namespace MiauAPI.Services;

/// <summary>
/// Contains methods to easily read and write images to the file system.
/// </summary>
public sealed class ImageService
{
    /// <summary>
    /// The absolute path of the API's Data folder.
    /// </summary>
    private static readonly string _appDataPath = Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location)?.FullName ?? string.Empty, "Data");

    /// <summary>
    /// Saves an image to the API's Data folder.
    /// </summary>
    /// <param name="imageData">The content of the image.</param>
    /// <param name="subdirectoryName">The name of the subdirectory.</param>
    /// <param name="imageName">The name of the image.</param>
    /// <param name="cToken">The cancellation token.</param>
    /// <returns>The absolute path to the new image file.</returns>
    /// <exception cref="ArgumentException">Occurs when <paramref name="imageData"/> is empty.</exception>
    public async Task<string> SaveImageAsync(ReadOnlyMemory<byte> imageData, string subdirectoryName, string imageName, CancellationToken cToken = default)
    {
        if (imageData.Length is 0)
            throw new ArgumentException($"Image's content cannot be empty.", nameof(imageData));

        var directoryPath = Path.Combine(_appDataPath, subdirectoryName);

        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);

        var imagePath = Path.Combine(directoryPath, imageName);

        await using var fileStream = new FileStream(imagePath, FileMode.Create, FileAccess.Write);
        fileStream.Position = 0;

        await fileStream.WriteAsync(imageData, cToken);

        return imagePath;
    }

    /// <summary>
    /// Reads and returns the content of the specified file image.
    /// </summary>
    /// <param name="imagePath">The location of the image in the file system.</param>
    /// <returns>The content of the image.</returns>
    /// <exception cref="FileNotFoundException">Occurs when the file does not exist at the <paramref name="imagePath"/> location.</exception>
    public async Task<byte[]> ReadImageAsync(string imagePath)
    {
        return (File.Exists(imagePath))
            ? await File.ReadAllBytesAsync(imagePath)
            : throw new FileNotFoundException($"Could not find the specified file at {imagePath}");
    }
}
