using Arcane.Carmen.Logging;

namespace Arcane.Carmen.Writer
{
    public record WriterLogEntry(DateTime Created, string Message, LogLevel Level) : ILogEntry;

    public class WriterLog : ILogger<WriterLogEntry>
    {
        public List<WriterLogEntry> Entries { get; init; } = [];

        public event Action<WriterLogEntry>? OnEntry;

        public void Log(WriterLogEntry entry)
        {
            Entries?.Add(entry);
            OnEntry?.Invoke(entry);
        }

        public void Log(string message, LogLevel level)
        {
            Log(new(DateTime.Now, message, level));
        }
    }
}
