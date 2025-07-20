using Arcane.Carmen.Lexer.Tokens;

namespace Arcane.Carmen.AST.Expressions
{
    public record ExprNullLiteral : Expression
    {
        public override string ToString() => "null";
    }

    public class ExprNullLiteralParser : ExpressionParser
    {
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
