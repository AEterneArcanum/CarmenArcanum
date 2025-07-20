using Arcane.Carmen.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST
{
    public abstract class StatementParser : Parser<Statement>
    {
        protected StatementParser(int priority)
            : base(priority) { }
    }
}
