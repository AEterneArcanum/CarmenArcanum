using Arcane.Carmen.AST.Expressions;
using Arcane.Carmen.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST
{
    public abstract class ExpressionParser : Parser<Expression>
    {
        protected ExpressionParser(int priority)
            : base(priority)
        { }
    }
}
