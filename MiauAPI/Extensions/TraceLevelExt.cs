using System.Diagnostics;

namespace MiauAPI.Extensions;

public static class TraceLevelExt
{
    /// <summary>
    /// Converts this <see cref="TraceLevel"/> to its equivalent value in <see cref="LogLevel"/>.
    /// </summary>
    /// <param name="traceLevel">This trace level.</param>
    /// <returns>The corresponding <see cref="LogLevel"/>.</returns>
    /// <exception cref="NotSupportedException">Occurs when <paramref name="traceLevel"/> is not implemented.</exception>
    public static LogLevel ToLogLevel(this TraceLevel traceLevel)
    {
        return traceLevel switch
        {
            TraceLevel.Off => LogLevel.None,
            TraceLevel.Error => LogLevel.Error,
            TraceLevel.Warning => LogLevel.Warning,
            TraceLevel.Info => LogLevel.Information,
            TraceLevel.Verbose => LogLevel.Trace,
            _ => throw new NotSupportedException($"{nameof(TraceLevel)} of type {traceLevel} is unknown.")
        };
    }
}