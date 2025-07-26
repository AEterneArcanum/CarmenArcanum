using Arcane.Carmen.Collector.AST;
using Arcane.Carmen.Collector.Lexer.Tokens;

namespace Arcane.Carmen.Collector.AST.Expressions
{
    public record ExprNullLiteral : Expression;

    public class ExprNullLiteralParser : ExpressionParser
    {
        public ExprNullLiteralParser() : base(ExpressionPriorities.NullLiteral) { }
        public ExprNullLiteralParser(int priority = ExpressionPriorities.NullLiteral) : base(priority) // Assuming lowest priority for null literals
        {
        }
        public override bool TryParse(Token[] tokens, out Expression? expression)
        {
            expression = null;
            if (tokens.Length != 1) return false;
            var token = tokens[0];
            if (token.Type != TokenType.LiteralNull)
                return false;
            expression = new ExprNullLiteral();
            return true;
        }
    }
}
