using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.Logging
{
    public record LogEntry(DateTime Time, LogLevel Level, string Message)
    {
        public override string ToString()
        {
            return $"{Time} : {Level} : {Message}";
        }
    }
}
