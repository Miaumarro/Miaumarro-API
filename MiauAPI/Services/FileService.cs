using System.Reflection;
using System;

namespace MiauAPI.Services;

/// <summary>
/// Contains methods to easily read and write images to the file system.
/// </summary>
public sealed class FileService
{
    /// <summary>
    /// The absolute path of the API's Data folder.
    /// </summary>
    private static readonly string _appDataPath = Path.Combine(AppContext.BaseDirectory, "Data");

    /// <summary>
    /// Checks if a file exists in the API's Data folder.
    /// </summary>
    /// <param name="subdirectoryName">The name of the subdirectory.</param>
    /// <param name="fileName">The name of the file.</param>
    /// <returns><see langword="true"/> if the file exists, <see langword="false"/> otherwise.</returns>
    public bool FileExists(string subdirectoryName, string fileName)
        => File.Exists(Path.Combine(_appDataPath, subdirectoryName, fileName));

    /// <summary>
    /// Saves an image to the API's Data folder.
    /// </summary>
    /// <param name="fileData">The content of the file.</param>
    /// <param name="subdirectoryName">The name of the subdirectory.</param>
    /// <param name="fileName">The name of the file.</param>
    /// <param name="cToken">The cancellation token.</param>
    /// <returns>The absolute path to the new image file.</returns>
    /// <exception cref="ArgumentException">Occurs when <paramref name="fileData"/> is empty.</exception>
    public async Task<string> SaveFileAsync(ReadOnlyMemory<byte> fileData, string subdirectoryName, string fileName, CancellationToken cToken = default)
    {
        if (fileData.Length is 0)
            throw new ArgumentException($"Image's content cannot be empty.", nameof(fileData));

        var directoryPath = Path.Combine(_appDataPath, subdirectoryName);

        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);

        var imagePath = Path.Combine(directoryPath, fileName);

        await using var fileStream = new FileStream(imagePath, FileMode.Create, FileAccess.Write);
        fileStream.Position = 0;

        await fileStream.WriteAsync(fileData, cToken);

        return imagePath;
    }

    /// <summary>
    /// Reads and returns the content of the specified file.
    /// </summary>
    /// <param name="filePath">The location of the file in the file system.</param>
    /// <param name="cToken">The cancellation token.</param>
    /// <returns>The content of the file.</returns>
    /// <exception cref="FileNotFoundException">Occurs when the file does not exist at the <paramref name="filePath"/> location.</exception>
    public async Task<byte[]> ReadFileAsync(string filePath, CancellationToken cToken = default)
    {
        return (File.Exists(filePath))
            ? await File.ReadAllBytesAsync(filePath, cToken)
            : throw new FileNotFoundException($"Could not find the specified file at {filePath}");
    }

    /// <summary>
    /// Deletes a file from the file system.
    /// </summary>
    /// <param name="subdirectoryName">The name of the subdirectory.</param>
    /// <param name="fileName">The name of the file.</param>
    /// <returns><see langword="true"/> if the file was successfully deleted, <see langword="false"/> otherwise.</returns>
    public bool DeleteFile(string subdirectoryName, string fileName)
    {
        var path = Path.Combine(_appDataPath, subdirectoryName, fileName);
        var exists = File.Exists(path);

        if (exists)
            File.Delete(path);

        return exists;
    }
}
