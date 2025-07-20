using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST
{
    public static class StatementPriorities
    {
        public const int Label = 0;
        public const int Goto = 0;
        public const int Return = 0;
        public const int Assert = 1;
        public const int Break = 1;
        public const int Continue = 1;
        public const int Expression = 2; // Function calls, increment/decrement, etc. Expressions that cas standalone.
        public const int Assignment = 5;
        public const int Using = 6;
        public const int Import = 6;
        public const int Alias = 6;
        public const int Conditional = 10;
        public const int Loop = 20; // For loops, while loops, etc.
        public const int Block = 30;
        public const int Definitions = 31;
    }
}
