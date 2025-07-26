using Arcane.Carmen.Collector.Lexer.Tokens;

namespace Arcane.Carmen.Collector.AST
{
    public abstract record Statement : IVisitable
    {
        public static StatementHandler Handler { get; } = new StatementHandler();
        public static bool TryParse(Token[] tokens, out Statement? statement)
        {
            statement = null;
            if (tokens == null || tokens.Length == 0)
            {
                return false; // No tokens to parse
            }
            if (tokens[^1].Type == TokenType.EndOfStatement)
            {
                // Remove the EndOfStatement token for parsing
                tokens = tokens[0..^1];
            }
            return Handler.TryParse(tokens, out statement);
        }
        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
