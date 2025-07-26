using Arcane.Carmen.Collector.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.Collector.AST
{
    public abstract class Parser<T> where T : class
    {
        public int Priority { get; }
        protected Parser(int priority)
        {
            Priority = priority;
        }
        public abstract bool TryParse(Token[] tokens, out T? result);
    }
}
