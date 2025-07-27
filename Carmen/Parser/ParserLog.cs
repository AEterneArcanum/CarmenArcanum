using Arcane.Carmen.Logging;

namespace Arcane.Carmen.Parser;

public class ParserLog : ILogger<ParserLogEntry>
{
    public List<ParserLogEntry> Entries { get; init; } = [];

    public event Action<ParserLogEntry>? OnEntry;

    public void Log(ParserLogEntry entry)
    {
        Entries?.Add(entry);
        OnEntry?.Invoke(entry);
    }

    public void Log(string Message, LogLevel Level)
    {
        Log(new(DateTime.Now, Level, Message));
    }
}
