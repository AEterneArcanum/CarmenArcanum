using Arcane.Carmen.Collector.AST;
using Arcane.Carmen.Collector.Lexer.Tokens;

namespace Arcane.Carmen.Collector.AST.Expressions
{
    public record ExprCharLiteral(string Value) : Expression;

    public class ExprCharLiteralParser : ExpressionParser
    {
        public ExprCharLiteralParser()
            : base(ExpressionPriorities.CharacterLiteral) { }
        public ExprCharLiteralParser(int priority = ExpressionPriorities.CharacterLiteral) : base(priority) { }
        public override bool TryParse(Token[] tokens, out Expression? expression)
        {
            expression = null;
            if (tokens.Length != 1) return false;
            var token = tokens[0];
            if (token.Type != TokenType.LiteralCharacter)
                return false;
            if (token.Raw.Length < 2 || token.Raw[0] != '\'' || token.Raw[^1] != '\'')
                return false; // Must be a character literal with single quotes.
            string raw = token.Raw[1..^1]; // remove only the containing single quotes, Trim would remove a contained single quote.
            expression = new ExprCharLiteral(raw);
            return true;
        }
    }
}