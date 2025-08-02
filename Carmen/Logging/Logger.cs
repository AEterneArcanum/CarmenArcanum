using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.Logging
{
    public class Logger
    {
        public event Action<LogEntry>? OnLog;

        public DateTime Start { get; init; } = DateTime.Now;
        public readonly List<LogEntry> LogEntries = [];

        public void Log(LogEntry entry)
        {
            LogEntries.Add(entry);
            OnLog?.Invoke(entry);
        }

        public void Log(string Message, LogLevel Level = LogLevel.Debug)
        {
            Log(new(DateTime.Now, Level, Message));
        }

        public void Save(string filepath)
        {
            File.WriteAllLines(
                $"{filepath}/{Start.ToString("yy MMM dd HH mm ss")}.txt", 
                LogEntries.Select(x => x.ToString()));
        }
    }
}
