using Arcane.Carmen.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.AST.Statements
{
    public record StmtIncrement(Expressions.ExprIncrement Increment) : Statement
    {
    }
    public class StmtIncrementParser : StatementParser
    {
        public StmtIncrementParser(int priority = StatementPriorities.Expression) : base(priority)
        {
        }
        public override bool TryParse(Token[] tokens, out Statement? result)
        {
            if (!Expression.TryParse(tokens, out var increment)
                || increment is not Expressions.ExprIncrement)
            {
                result = null;
                return false;
            }
            result = new StmtIncrement((Expressions.ExprIncrement)increment!);
            return true;
        }
    }
}
