using Arcane.Carmen.Lexer.Tokens;

namespace Arcane.Carmen.AST.Expressions
{
    public record ExprBooleanLiteral(bool Value) : Expression
    {
        public override string ToString() => Value ? "true" : "false";
    };

    public class ExprBooleanLiteralParser : ExpressionParser
    {
        public ExprBooleanLiteralParser(int priority = ExpressionPriorities.BooleanLiteral) : base(priority) { }
        public override bool TryParse(Token[] tokens, out Expression? expression)
        {
            expression = null;
            if (tokens.Length != 1) return false;
            var token = tokens[0];
            if (token.Type != TokenType.LiteralTrue && token.Type != TokenType.LiteralFalse)
                return false;
            expression = new ExprBooleanLiteral(token.Type == TokenType.LiteralTrue);
            return true;
        }
    }
}
