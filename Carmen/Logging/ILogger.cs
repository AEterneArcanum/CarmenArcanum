using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.Logging
{
    public enum LogLevel
    {
        Debug,
        Info,
        Warn,
        Error,
        Fatal
    }

    public interface ILogEntry
    {
        public DateTime Created { get; }
        public string Message { get; }
        public LogLevel Level { get; }
    }

    public interface ILogger<T> where T : ILogEntry
    {
        public event Action<T>? OnEntry;

        public List<T> Entries { get; }

        public void Log(T entry);
    }
}
