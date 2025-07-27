using Arcane.Carmen.Logging;

namespace Arcane.Carmen.Parser;

public record ParserLogEntry(DateTime Created, LogLevel Level, string Message) : ILogEntry;
