using Arcane.Carmen.Collector.AST;
using Arcane.Carmen.Collector.AST.Expressions;
using Arcane.Carmen.Collector.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen.Collector.AST.Statements
{
    public record StmtDecrement(ExprDecrement Decrement) : Statement;

    public class StmtDecrementParser : StatementParser
    {
        public StmtDecrementParser() :base(StatementPriorities.Expression) { }
        public StmtDecrementParser(int priority = StatementPriorities.Expression) : base(priority)
        {
        }
        public override bool TryParse(Token[] tokens, out Statement? result)
        {
            if (!Expression.TryParse(tokens, out var decrement)
                || decrement is not ExprDecrement)
            {
                result = null;
                return false;
            }
            result = new StmtDecrement((ExprDecrement)decrement!);
            return true;
        }
    }
}
