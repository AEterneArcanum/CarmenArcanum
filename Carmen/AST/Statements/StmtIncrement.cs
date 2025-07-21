using Arcane.Carmen.AST.Expressions;
using Arcane.Carmen.Lexer.Tokens;

namespace Arcane.Carmen.AST.Statements
{
    public record StmtIncrement(ExprIncrement Increment) : Statement;
    public class StmtIncrementParser : StatementParser
    {
        public StmtIncrementParser(int priority = StatementPriorities.Expression) : base(priority)
        {
        }
        public override bool TryParse(Token[] tokens, out Statement? result)
        {
            if (!Expression.TryParse(tokens, out var increment)
                || increment is not ExprIncrement)
            {
                result = null;
                return false;
            }
            result = new StmtIncrement((ExprIncrement)increment!);
            return true;
        }
    }
}
